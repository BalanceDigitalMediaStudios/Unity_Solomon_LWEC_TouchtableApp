using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttractTimer : ZonedMonobehaviour{

    public delegate void  AttractEvent (string zoneId);
    public static   event AttractEvent onTriggerAttract;

    [SerializeField] float maxIdleTime = 60;
    [SerializeField, ReadOnly] float idleTime = 0;

    [SerializeField] GameObject[] ignoreObjects;
    [SerializeField] RectTransform interactionZone;


    void Start(){ ResetTimer(); }
    void Update(){

        if(IsInteracting())
            ResetTimer();

        if(!IsIgnore() && idleTime > 0)
        {
            idleTime = Mathf.Max(0, idleTime - Time.deltaTime);
            if(idleTime <= 0)
                TriggerAttract();
        }
    }

    public void TriggerAttract(){

        ResetTimer();
        if(onTriggerAttract != null)
            onTriggerAttract(zoneId);
    }


    public void ResetTimer(){ idleTime = maxIdleTime; }



    bool IsInteracting(){

        //mouse
        if(Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(interactionZone, Input.mousePosition, Camera.main))
            return true;

        //touch
        for (int i = 0; i < Input.touchCount; i++)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(interactionZone, Input.GetTouch(i).position, Camera.main))
                return true;
        }

        return false;
    }

    bool IsIgnore(){

        for (int i = 0; i < ignoreObjects.Length; i++)
        {
            if (ignoreObjects[i].activeInHierarchy)
                return true;
        }        
        return false;
    }
}