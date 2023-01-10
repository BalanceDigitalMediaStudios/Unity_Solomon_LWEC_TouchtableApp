using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitatMatch : MonoBehaviour{

    [SerializeField, ReadOnly] StickerSpawner   spawner;  //sticker to unlock
    [SerializeField] UnlockScreen               unlockScreen;
    [SerializeField] UITransitionFade           mainFade;

    [Header("Choices")]
    [SerializeField] Button                     backButton;
    [SerializeField] UITransitionFade           instructionsFade;
    [SerializeField] Button                     closeInstructionsButton;
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
    HabitatMatchData match;    


    void Awake(){

        startPos    = sticker.transform.position;
        startScale  = sticker.transform.localScale;

        backButton.             onClick.AddListener(Close);
        closeInstructionsButton.onClick.AddListener(CloseInstructionsButton);
        continueButton.         onClick.AddListener(CloseAndUnlockSticker);        
    }


    void OnEnable(){

        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;

        sticker.onEndDrag += OnEndDrag;
        for (int i = 0; i < dropZones.Length; i++)
            dropZones[i].onDrop += OnDrop;

        ResetSticker();
        madeChoice = false;
    }
    void OnDisable(){

        sticker.onEndDrag -= OnEndDrag;
        for (int i = 0; i < dropZones.Length; i++)
            dropZones[i].onDrop -= OnDrop;
    }




    public void Open(StickerSpawner spawner){

        //open menu and help, disable results screen
        gameObject.SetActive(true);
        instructionsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        instructionsFade.gameObject.SetActive(true);
        resultsFade.gameObject.SetActive(false);


        this.spawner = spawner;
        match = spawner.data.habitatMatch;
        
        sticker.Initialize(spawner.data.sticker.sprite);

        //randomize drop zones
        List<HabitatMatch_DropZone> dropZoneList = new List<HabitatMatch_DropZone>(dropZones);
        for (int i = 0; i < dropZones.Length && i < match.choices.Length; i++)
        {
            int rand = Random.Range(0, dropZoneList.Count);

            HabitatMatch_DropZone temp = dropZoneList[rand];
            dropZoneList.RemoveAt(rand);

            temp.Initialize(match.choices[i]);
        }
    }

    void CloseInstructionsButton(){

        instructionsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        instructionsFade.TransitionToStart(true, instructionsFade.transitionTime, 0f);
    }

    void CloseAndUnlockSticker(){

        unlockScreen.OpenAndUnlock(spawner);
        Close();        
    }
    void Close(){

        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        mainFade.TransitionToStart(true, mainFade.transitionTime, 0f);
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
        resultText.text = match.choices[match.correctIndex].name == habitat.name ? "CORRECT!" : "ACTUALLY...";
        flavorText.text = match.flavorText;

        //open results
        resultsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        resultsFade.gameObject.SetActive(true);
    }
}