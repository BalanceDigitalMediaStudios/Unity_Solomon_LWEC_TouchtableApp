using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitatMatch : UnlockActivity{

    [Header("Instructions")]
    [SerializeField] UITransitionFade   instructionsFade;
    [SerializeField] Button             closeInstructionsButton;

    [Header("Activity")]
    [SerializeField] HabitatMatch_Draggable     sticker;
    [SerializeField] HabitatMatch_DropZone[]    dropZones;
    [SerializeField, ReadOnly] bool             madeChoice = false;

    [Header("Results")]
    [SerializeField] UITransitionFade   resultsFade;
    [SerializeField] TextMeshProUGUI    resultText;
    [SerializeField] TextMeshProUGUI    flavorText;


    Vector3 startScale;
    Vector3 startPos;
    HabitatMatchData matchData;    


    protected override void Awake(){

        base.Awake();

        startPos    = sticker.transform.position;
        startScale  = sticker.transform.localScale;

        closeInstructionsButton.onClick.AddListener(CloseInstructionsButton);      
    }


    void OnEnable(){

        sticker.onEndDrag += OnEndDrag;
        for (int i = 0; i < dropZones.Length; i++)
            dropZones[i].onDrop += OnDrop;
    }
    void OnDisable(){

        sticker.onEndDrag -= OnEndDrag;
        for (int i = 0; i < dropZones.Length; i++)
            dropZones[i].onDrop -= OnDrop;
    }




    public override void Open(StickerSpawner spawner){

        base.Open(spawner);        

        //enable instructions and disable results
        instructionsFade.gameObject.SetActive(true);
        instructionsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        instructionsFade.TransitionToEnd(true, instructionsFade.transitionTime, instructionsFade.delayTime);
        resultsFade.gameObject.SetActive(false);


        //assign data        
        matchData = spawner.data.habitatMatch;
        sticker.Initialize(spawner.data.sticker.sprite);
        ResetSticker();
        madeChoice = false;


        //randomize drop zones
        List<HabitatMatch_DropZone> dropZoneList = new List<HabitatMatch_DropZone>(dropZones);
        for (int i = 0; i < dropZones.Length && i < matchData.choices.Length; i++)
        {
            int rand = Random.Range(0, dropZoneList.Count);

            HabitatMatch_DropZone temp = dropZoneList[rand];
            dropZoneList.RemoveAt(rand);

            temp.Initialize(matchData.choices[i]);
        }
    }

    void CloseInstructionsButton(){

        instructionsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        instructionsFade.TransitionToStart(true, instructionsFade.transitionTime, 0f);
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

        //set result text
        resultText.text = matchData.correctHabitat.name == habitat.name ? "CORRECT!" : "ACTUALLY...";
        flavorText.text = matchData.flavorText;

        //open results
        resultsFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        resultsFade.gameObject.SetActive(true);
    }
}