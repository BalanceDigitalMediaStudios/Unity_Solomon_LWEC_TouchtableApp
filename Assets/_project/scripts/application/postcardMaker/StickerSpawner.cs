using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StickerSpawner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{

    [Header("Locking")]
    [SerializeField] CanvasGroup    stickerCG;

    [SerializeField] CanvasGroup    _lockedCG;
    public CanvasGroup lockedCG{get { return _lockedCG; } }

    [SerializeField] Image          _questionMarkImage;
    public Image questionMarkImage{get { return _questionMarkImage; } }

    [SerializeField] Image          _circleImage;
    public Image circleImage{get { return _circleImage; } }

    [SerializeField] Button         unlockButton;    
    [SerializeField, ReadOnly] bool isUnlocked;


    [Header("Other")]
    [SerializeField] Image                          image;
    [SerializeField] DraggableSticker               prefab;
    [SerializeField, ReadOnly] DraggableSticker     instance;
    [SerializeField, ReadOnly] StickerSettings      _data;
    public StickerSettings data { get { return _data; } }
    

    bool isDragging = false;

    PostcardMaker _postcardMaker;
    public PostcardMaker postcardMaker { get { if (_postcardMaker == null) _postcardMaker = GetComponentInParent<PostcardMaker>(true); return _postcardMaker; } }



    void Awake(){

        unlockButton.onClick.AddListener(UnlockButton);
    }



    public void Initialize(StickerSettings data, string name){

        this._data  = data;
        this.name   = name;
        if (data != null && data.sticker != null)
        {
            if(data.sticker.sprite != null)
                image.sprite = data.sticker.sprite;

            if(data.unlockMethod == StickerSettings.UnlockMethod.unlocked)
                UnlockSticker();
            else
                LockSticker();
        }
    }


    public void OnPointerDown(PointerEventData eventData){

        if(!isUnlocked)
            return;

        //spawn new sticker using same sprite and send along all event data
        instance                        = Instantiate(prefab, transform.position, transform.rotation, postcardMaker.draggableArea.transform);
        instance.transform.localScale   = Vector3.one;
        string newName                  = string.Format("{0}_{1}", prefab.name, data.sticker.longName);

        instance.Initialize(image.sprite, newName);

        instance.OnPointerDown(eventData);
    }
    public void OnPointerUp (PointerEventData eventData) {

        if(!isUnlocked)
            return;
        
        //remove the new sticker if it was never dragged
        if(!isDragging && instance != null)
            instance.Destroy();

        instance.OnPointerUp(eventData); 
    }
    public void OnBeginDrag (PointerEventData eventData) {

        if(!isUnlocked)
            return;
        
        postcardMaker.OnAddSticker();  //only add sticker to count once we've officially dragged it
        isDragging = true;
        instance.OnBeginDrag(eventData); 
    }
    public void OnDrag (PointerEventData eventData) {

        if(!isUnlocked)
            return;
        
        instance.OnDrag(eventData);
    }
    public void OnEndDrag (PointerEventData eventData) {

        if(!isUnlocked)
            return;

        isDragging = false;
        instance.OnEndDrag(eventData);
    }



    public void UnlockSticker(){

        isUnlocked = true;

        transform.SetParent(postcardMaker.unlockedGroup);
        transform.SetAsLastSibling();

        stickerCG.alpha = 1;
        unlockButton.gameObject.SetActive(false);

        postcardMaker.OnUnlockSticker();
    }

    public void LockSticker(){

        isUnlocked = false;

        transform.SetParent(postcardMaker.lockedGroup);
        transform.SetAsLastSibling();

        stickerCG.alpha = .3f;
        unlockButton.gameObject.SetActive(true);

        postcardMaker.OnLockSticker();
    }


    void UnlockButton(){

        if(data.unlockMethod == StickerSettings.UnlockMethod.habitatMatch && data.habitatMatch != null)
            postcardMaker.habitatMatch.Open(this);
        
        if(data.unlockMethod == StickerSettings.UnlockMethod.pollQuestion && data.pollQuestion != null)
            postcardMaker.poll.Open(this);
    }
}