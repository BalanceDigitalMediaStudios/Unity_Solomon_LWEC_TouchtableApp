using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitatMatch : MonoBehaviour{

    [SerializeField, ReadOnly] StickerSpawner   spawner;  //sticker to unlock
    [SerializeField, ReadOnly] HabitatMatchData data;
    [SerializeField] UITransitionFade           mainFade;

    [Header("Choices")]
    [SerializeField] Button                     backButton;
    [SerializeField] UITransitionFade           instructionsFade;
    [SerializeField] HabitatMatch_Draggable     sticker;
    [SerializeField] HabitatMatch_DropZone[]    dropZones;
    [SerializeField, ReadOnly] bool             madeChoice = false;

    [Header("Results")]
    [SerializeField] UITransitionFade   resultsFade;
    [SerializeField] TextMeshProUGUI    resultText;
    [SerializeField] TextMeshProUGUI    flavorText;
    [SerializeField] Button             continueButton;


    Vector3 startScale;
    Vector3 startPos;


    public void Initialize(StickerSpawner spawner, HabitatMatchData data){

        this.spawner    = spawner;
        this.data       = data;

        sticker.Initialize(spawner.data.sticker.sprite);
        for (int i = 0; i < dropZones.Length && i < data.choices.Length; i++)
            dropZones[i].Initialize(data.choices[i]);
    }



    void Awake(){

        startPos    = sticker.transform.position;
        startScale  = sticker.transform.localScale;

        backButton.     onClick.AddListener(Close);
        continueButton. onClick.AddListener(CloseAndUnlockSticker);
    }

    void OnEnable(){

        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;

        sticker.onEndDrag += OnEndDrag;
        for (int i = 0; i < dropZones.Length; i++)
            dropZones[i].onDrop += OnDrop;

        ResetSticker();
    }

    void OnDisable(){

        sticker.onEndDrag -= OnEndDrag;
        for (int i = 0; i < dropZones.Length; i++)
            dropZones[i].onDrop -= OnDrop;
    }


    void ResetSticker(){ 
        
        sticker.transform.position      = startPos;
        sticker.transform.localScale    = startScale;
    }



    void OnEndDrag(){

        //put sticker back if a choice was not made
        if(!madeChoice)
            ResetSticker();
    }



    void OnDrop(HabitatData habitat){

        madeChoice = true;

        //set result data
        resultText.text = data.choices[data.correctIndex].name == habitat.name ? "CORRECT!" : "ACTUALLY...";
        flavorText.text = data.flavorText;

        //open results
        resultsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        resultsFade.gameObject.SetActive(true);
    }




    void CloseAndUnlockSticker(){

        //TODO - add unlock screen here

        spawner.UnlockSticker();
        Close();        
    }

    void Close(){

        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        mainFade.TransitionToStart(true, mainFade.transitionTime, 0f);
    }
}