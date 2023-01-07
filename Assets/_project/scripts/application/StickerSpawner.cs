using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StickerSpawner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{

    [SerializeField] Image                          image;
    [SerializeField] DraggableSticker               prefab;
    [SerializeField, ReadOnly] DraggableSticker     instance;
    [SerializeField, ReadOnly] StickerSettings      data;
    

    bool isDragging = false;

    PostcardMaker _postcardMaker;
    PostcardMaker postcardMaker { get { if (_postcardMaker == null) _postcardMaker = GetComponentInParent<PostcardMaker>(true); return _postcardMaker; } }



    public void Initialize(StickerSettings data){

        this.data = data;
        if (data != null && data.sticker != null && data.sticker.sprite != null)
        {
            image.sprite = data.sticker.sprite;

            if(data.unlockMethod == StickerSettings.UnlockMethod.unlocked)
                UnlockSticker();
            else
                LockSticker();
        }
    }


    public void OnPointerDown(PointerEventData eventData){

        //spawn new sticker using same sprite and send along all event data
        instance = Instantiate(prefab, transform.position, transform.rotation, postcardMaker.draggableArea.transform);
        instance.transform.localScale = Vector3.one;
        instance.Initialize(image.sprite);

        instance.OnPointerDown(eventData);
    }
    public void OnPointerUp (PointerEventData eventData) { 
        
        //remove the new sticker if it was never dragged
        if(!isDragging && instance != null)
            instance.Destroy();

        instance.OnPointerUp(eventData); 
    }
    public void OnBeginDrag (PointerEventData eventData) {
        
        postcardMaker.OnAddSticker();  //only add sticker to count once we've officially dragged it
        isDragging = true;
        instance.OnBeginDrag(eventData); 
    }
    public void OnDrag      (PointerEventData eventData) { instance.OnDrag(eventData); }
    public void OnEndDrag   (PointerEventData eventData) {

        isDragging = false;
        instance.OnEndDrag(eventData); 
    }



    public void UnlockSticker(){

        transform.SetParent(postcardMaker.unlockedGroup);
        transform.SetAsLastSibling();
    }

    public void LockSticker(){

        transform.SetParent(postcardMaker.lockedGroup);
        transform.SetAsLastSibling();
    }
}