using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IdleManager : MonoBehaviour{

    [SerializeField]float           maxIdleTime = 60f;
    [SerializeField]float           currentIdleTime;
    [SerializeField]GameObject[]    ignoreObjects;


    void Awake(){

        ResetIdleTime();
    }


    void Update(){

        if(Input.GetMouseButton(0) || Input.touchCount > 0)
            ResetIdleTime();
        
        if(!IsIgnore() && currentIdleTime > 0)
        {
            currentIdleTime -= Time.deltaTime;
            if(currentIdleTime <= 0)
                Reload();
        }
    }

    bool IsIgnore(){

        foreach(GameObject go in ignoreObjects)
        {
            if(go.activeInHierarchy)
                return true;
        }
        return false;
    }

    void ResetIdleTime(){

        currentIdleTime = maxIdleTime;
    }

    public void Reload(){

        ReloadScene.instance.Reload();
    }
}