using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UnlockActivity : MonoBehaviour{
    
    [SerializeField] protected UnlockScreen               unlockScreen;
    [SerializeField] protected UITransitionFade           mainFade;
    [SerializeField] protected Button                     backButton;
    [SerializeField] protected Button                     continueButton;
    [SerializeField, ReadOnly] protected StickerSpawner   spawner;        //spawner of the sticker to unlock



    protected virtual void Awake(){

        backButton.     onClick.AddListener(Close);
        continueButton. onClick.AddListener(CloseAndUnlockSticker);
    }


    public virtual void Open(StickerSpawner spawner){

        //store data
        this.spawner = spawner;

        //fade in menu
        mainFade.gameObject.SetActive(true);
        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        mainFade.TransitionToEnd(true, mainFade.transitionTime, mainFade.delayTime);
    }

    protected void CloseAndUnlockSticker(){

        //unlock sticker
        unlockScreen.OpenAndUnlock(spawner);
        Close();        
    }
    protected void Close(){

        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        mainFade.TransitionToStart(true, mainFade.transitionTime, 0f);
    }
}