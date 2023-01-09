using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{

    public delegate void DragEvent();
    public event DragEvent onPointerDown, onPointerUp, onBeginDrag, onDrag, onEndDrag;


    Vector3 offset;
    bool    _isDragging = false;
    public bool isDragging{ get { return _isDragging; } }


    public virtual void OnPointerDown(PointerEventData eventData){

        //Debug.LogFormat("OnPointerDown: {0}", name);
        offset = transform.position - eventData.pointerCurrentRaycast.worldPosition;

        if(onPointerDown != null)
            onPointerDown();
    }

    public virtual void OnPointerUp(PointerEventData eventData){

        //Debug.LogFormat("OnPointerUp: {0}", name);

        if(onPointerUp != null)
            onPointerUp();
    }

    public virtual void OnBeginDrag(PointerEventData eventData){

        //Debug.LogFormat("OnBeginDrag: {0}", name);
        _isDragging = true;

        if(onBeginDrag != null)
            onBeginDrag();
    }

    public virtual void OnDrag(PointerEventData eventData){
        
        if(eventData.pointerCurrentRaycast.gameObject == null)  //ignore if pointer is off screen
            return;

        transform.position = eventData.pointerCurrentRaycast.worldPosition + offset;

        if(onDrag != null)
            onDrag();
    }

    public virtual void OnEndDrag(PointerEventData eventData){
        
        //Debug.LogFormat("OnEndDrag {0}", name);
        _isDragging = false;

        if(onEndDrag != null)
            onEndDrag();
    }
}