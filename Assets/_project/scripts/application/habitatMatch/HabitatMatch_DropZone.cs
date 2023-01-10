using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HabitatMatch_DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler{

    public delegate void DropEvent (HabitatData habitat);
    public event DropEvent onDrop;

    [SerializeField] TextMeshProUGUI    title;
    [SerializeField] Image              image;
    [SerializeField] UITransitionScale  titleScaler;
    [SerializeField] UITransitionScale  scaler;
    [SerializeField] UITransitionFade   fader;

    [SerializeField, ReadOnly] HabitatData data;



    public void Initialize(HabitatData data){

        this.data = data;

        if(data != null)
        {
            title.text      = data.longName;
            image.sprite    = data.sprite;
        }
    }

    void OnEnable(){

        titleScaler.loop = false;
        titleScaler.    TransitionToStart(true);
        scaler.         TransitionToStart(true);
        fader.          TransitionToStart(true);
    }


    public void OnDrop(PointerEventData eventData){

        Debug.LogFormat("OnDrop: {0} Dragging: {1}", name, eventData.pointerDrag);
        if(onDrop != null)
            onDrop(data);
    }

    public void OnPointerEnter(PointerEventData eventData){

        if (eventData.pointerDrag != null)
        {
            titleScaler.loop = true;
            titleScaler.    TransitionToEnd(true);
            scaler.         TransitionToEnd(true);
            fader.          TransitionToEnd(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData){

        if (eventData.pointerDrag != null)
        {
            titleScaler.loop = false;
            titleScaler.    TransitionToStart(true);
            scaler.         TransitionToStart(true);
            fader.          TransitionToStart(true);
        }
    }
}