using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StickerSpawner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{

    [SerializeField] Image              image;
    [SerializeField] DraggableSticker   prefab;
    [SerializeField] StickerData        data;
    [SerializeField, ReadOnly] DraggableSticker   instance;

    PostcardMaker _postcardMaker;
    PostcardMaker postcardMaker { get { if (_postcardMaker == null) _postcardMaker = GetComponentInParent<PostcardMaker>(true); return _postcardMaker; } }


    void OnValidate(){

        if(data != null && data.sprite != null)
            Initialize(data);
    }

    void Initialize(StickerData data){

        this.data = data;
        if(data != null && data.sprite != null) image.sprite = data.sprite;
    }


    public void OnPointerDown(PointerEventData eventData){

        //spawn new sticker and send along drag data
        instance = Instantiate(prefab, transform.position, transform.rotation, postcardMaker.draggableArea.transform);
        instance.transform.localScale = Vector3.one;
        if(data != null && data.sprite != null)
            instance.Initialize(data.sprite);
        
        instance.OnPointerDown(eventData);
    }
    public void OnPointerUp(PointerEventData eventData){

        instance.OnPointerUp(eventData);
    }



    public void OnBeginDrag(PointerEventData eventData){        

        instance.OnBeginDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData){

        instance.OnDrag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData){

        instance.OnEndDrag(eventData);
    }
}