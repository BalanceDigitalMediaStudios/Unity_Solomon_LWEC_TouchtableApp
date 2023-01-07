using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Help : ZonedMonobehaviour{

    [SerializeField] UITransitionFade   fade;
    [SerializeField] Button             continueButton;
    [SerializeField] PostcardMaker      postcardMaker;

    [SerializeField] int unlockedIndex;
    [SerializeField] int lockedIndex;
    [SerializeField] int sortingOrder;

    List<StickerSpawner> spawners = new List<StickerSpawner>(0);  //spawners that will show up in the help screen

    void Awake(){

        continueButton.onClick.AddListener(Close);
    }

    void OnEnable() { PostcardMaker.onHasInitialized += OnPostcardMakerInitialized; }
    void OnDisable(){ PostcardMaker.onHasInitialized -= OnPostcardMakerInitialized; }
    void OnPostcardMakerInitialized(string zoneId){

        if (IsThisZone(zoneId))
            Open();
    }



    void AddCanvas(GameObject go){

        Canvas canvas           = go.AddComponent<Canvas>();
        canvas.overrideSorting  = true;
        canvas.sortingOrder     = sortingOrder;
    }
    void RemoveCanvas(GameObject go){

        Canvas canvas = go.GetComponent<Canvas>();
        if(canvas)
            Destroy(canvas);
    }




    void Open(){

        /* StopAllCoroutines();
        StartCoroutine(OpenRoutine()); */

        spawners.Clear();

        //get specific spawners
        StickerSpawner[] unlockedSpawners   = postcardMaker.unlockedGroup.  GetComponentsInChildren<StickerSpawner>(true);
        StickerSpawner[] lockedSpawners     = postcardMaker.lockedGroup.    GetComponentsInChildren<StickerSpawner>(true);
        Debug.LogFormat("Unlocked: {0} Locked: {1}", unlockedSpawners.Length, lockedSpawners.Length);


        if(unlockedIndex < unlockedSpawners.Length)
            spawners.Add(unlockedSpawners[unlockedIndex]);
        if(lockedIndex < lockedSpawners.Length)
            spawners.Add(lockedSpawners[lockedIndex]);
        
        //add canvases to modify sorting layer of these spawners
        foreach(StickerSpawner s in spawners)
            AddCanvas(s.gameObject);

        //fade in
        fade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        fade.TransitionToEnd(false, fade.transitionTime, fade.delayTime);
    }
    /* IEnumerator OpenRoutine(){

        yield return new WaitForEndOfFrame();
        
    } */
    


    void Close(){

        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }
    IEnumerator CloseRoutine(){

        //fade out menu
        fade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        fade.TransitionToStart(true, fade.transitionTime, 0f);
        yield return new WaitForSeconds(fade.transitionTime);

        //remove canvases of modified spawners
        foreach(StickerSpawner s in spawners)
            RemoveCanvas(s.gameObject);

        spawners.Clear();

        //turn off help
        gameObject.SetActive(false);
    }
}