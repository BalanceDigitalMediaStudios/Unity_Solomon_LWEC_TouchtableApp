using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitWarning : MonoBehaviour{

    [SerializeField] UITransitionFade mainFade;
    [SerializeField] Button cancelButton;
    [SerializeField] Button exitButton;

    [SerializeField] AttractTimer attractTimer;


    void Awake(){

        cancelButton.   onClick.AddListener(Close);
        exitButton.     onClick.AddListener(Exit);
    }

    public void Open(){

        gameObject.SetActive(true);
        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        mainFade.TransitionToEnd(true);
    }

    void Close(){

        if(gameObject.activeInHierarchy)
        {
            mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
            mainFade.TransitionToStart(true);
        }
    }

    void Exit(){

        attractTimer.TriggerAttract();
    }
}