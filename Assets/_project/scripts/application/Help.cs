using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Help : ZonedMonobehaviour{

    [SerializeField] float              transitionDuration  = 1;
    [SerializeField] float              transitionOpenDelay = 1;
    [SerializeField] CanvasGroup        fadeCG;
    [SerializeField] Button             continueButton;
    [SerializeField] PostcardMaker      postcardMaker;

    [Header("Sorting Overrides")]
    [SerializeField] int    sortingOrder;
    [SerializeField] Image  sendPostcardImage;

    [Space(10)]
    [SerializeField] int    unlockedIndex;
    [SerializeField, ReadOnly] StickerSpawner unlockedSpawner;

    [Space(10)]
    [SerializeField] int    lockedIndex;
    [SerializeField, ReadOnly] StickerSpawner lockedSpawner;


    //original values for overriden UI elements
    Color sendPostcardColor_original;
    float lockedAlpha_original;
    Color questionMarkColor_original;
    Color circleColor_original;



    void Awake(){

        continueButton.onClick.AddListener(() => Close(transitionDuration));
    }

    void OnEnable() { PostcardMaker.onHasInitialized += OnPostcardMakerInitialized; }
    void OnDisable(){ PostcardMaker.onHasInitialized -= OnPostcardMakerInitialized; }
    void OnPostcardMakerInitialized(string zoneId){

        if (IsThisZone(zoneId))
            Open(transitionDuration, transitionOpenDelay);
    }



   




    void Open(float duration = 0, float delay = 0){ StopAllCoroutines(); StartCoroutine(OpenRoutine(duration, delay)); }
    IEnumerator OpenRoutine(float duration, float delay){

        fadeCG.blocksRaycasts   = true;
        fadeCG.alpha            = 0;

        //get specific spawners 
        StickerSpawner[] unlockedSpawners   = postcardMaker.unlockedGroup.  GetComponentsInChildren<StickerSpawner>(true);
        StickerSpawner[] lockedSpawners     = postcardMaker.lockedGroup.    GetComponentsInChildren<StickerSpawner>(true);

        //add canvases to modify sorting layers
        AddCanvas(sendPostcardImage.gameObject);
        if (unlockedIndex < unlockedSpawners.Length)
        {
            unlockedSpawner = unlockedSpawners[unlockedIndex];
            AddCanvas(unlockedSpawner.gameObject);
        }
        if (lockedIndex < lockedSpawners.Length)
        {
            lockedSpawner = lockedSpawners[lockedIndex];
            AddCanvas(lockedSpawner.gameObject);            
        }
        


        if(delay > 0)
            yield return new WaitForSeconds(delay);


        //transition colors for modified ui and also the main screen alpha
        if (lockedSpawner)
        {
            //store original colors for later use
            lockedAlpha_original        = lockedSpawner.lockedCG.alpha;
            questionMarkColor_original  = lockedSpawner.questionMarkImage.color;
            circleColor_original        = lockedSpawner.circleImage.color;            
            StartCoroutine(FadeLockedStickerSpawner(lockedSpawner, duration, 1, Color.white, ReplaceColorChannels(circleColor_original, "a", .5f)));
        }
        sendPostcardColor_original = sendPostcardImage.color;
        if(duration > 0)
        {
            float t = 0;
            while(t < 1)
            {
                t += Time.deltaTime / duration;

                fadeCG.alpha            = Mathf.Lerp(0, 1, t);
                sendPostcardImage.color = Color.Lerp(sendPostcardColor_original, Color.white, t);
                yield return null;
            }            
        }

        //set final values
        fadeCG.alpha            = 1;
        sendPostcardImage.color = Color.white;
    }    
    


    void Close(float duration = 0){ StopAllCoroutines(); StartCoroutine(CloseRoutine(duration)); }
    IEnumerator CloseRoutine(float duration){

        fadeCG.blocksRaycasts = false;

        if (duration > 0)
        {
            //get starting values
            float alphaStart                = fadeCG.alpha;
            Color sendPostcardColorStart    = sendPostcardImage.color;

            //fade colors back to original and fade out menu
            if(lockedSpawner)
                StartCoroutine(FadeLockedStickerSpawner(lockedSpawner, duration, lockedAlpha_original, questionMarkColor_original, circleColor_original));
            
            float t = 0;
            while(t < 1)
            {
                t += Time.deltaTime / duration;

                fadeCG.alpha            = Mathf.Lerp(alphaStart, 0, t);
                sendPostcardImage.color = Color.Lerp(sendPostcardColorStart, sendPostcardColor_original, t);
                yield return null;
            }   
        }

        //set final values
        fadeCG.alpha            = 0;
        sendPostcardImage.color = sendPostcardColor_original;

        //remove canvases of modified ui
        RemoveCanvas(sendPostcardImage.gameObject);
        if(unlockedSpawner)
            RemoveCanvas(unlockedSpawner.gameObject);
        if(lockedSpawner)
            RemoveCanvas(lockedSpawner.gameObject);
        

        //turn off help
        gameObject.SetActive(false);
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
    Color ReplaceColorChannels(Color input, string channels, float value){

        Color output = input;

        if(channels.ToLower().Contains("r"))
            output.r = value;
        if(channels.ToLower().Contains("g"))
            output.g = value;
        if(channels.ToLower().Contains("b"))
            output.b = value;
        if(channels.ToLower().Contains("a"))
            output.a = value;

        return output;
    }

    IEnumerator FadeLockedStickerSpawner(StickerSpawner spawner, float duration, float alpha, Color questionMarkColor, Color circleColor){

        //get start and end values
        float alphaStart                = spawner.lockedCG.alpha;
        float alphaEnd                  = alpha;

        Color questionMarkColorStart    = spawner.questionMarkImage.color;
        Color questionMarkColorEnd      = questionMarkColor;

        Color circleColorStart          = spawner.circleImage.color;
        Color circleColorEnd            = circleColor;
        

        //transition
        if(duration > 0)
        {
            float t = 0;
            while(t < 1)
            {
                t += Time.deltaTime / duration;

                spawner.lockedCG.alpha            = Mathf.Lerp(alphaStart, alphaEnd, t);
                spawner.questionMarkImage.color   = Color.Lerp(questionMarkColorStart, questionMarkColorEnd, t);
                spawner.circleImage.color         = Color.Lerp(circleColorStart, circleColorEnd, t);

                yield return null;
            }            
        }

        //set final values
        spawner.lockedCG.alpha            = alphaEnd;
        spawner.questionMarkImage.color   = questionMarkColorEnd;
        spawner.circleImage.color         = circleColorEnd; 
    }
}