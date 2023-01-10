using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockScreen : MonoBehaviour{

    [SerializeField] UITransitionFade   fade;
    [SerializeField] Image              stickerImage;
    [SerializeField] Button             closeButton;


    void Awake(){

        closeButton.onClick.AddListener(Close);
    }


    public void OpenAndUnlock(StickerSpawner spawner){

        fade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        fade.gameObject.SetActive(true);
        stickerImage.sprite = spawner.data.sticker.sprite;

        spawner.UnlockSticker();
    }

    void Close(){

        fade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        fade.TransitionToStart(true, fade.transitionTime, 0f);
    }
}