using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attract : ZonedMonobehaviour{

    [Header("Settings")]
    [SerializeField] float openDuration     = 1;
    [SerializeField] float closeDuration    = 1;

    [Header("Elements")]
    [SerializeField] CanvasGroup    attractCG;
    [SerializeField] CanvasGroup    experienceCG;
    [SerializeField] PostcardMaker  postcardMaker;   



    void Awake(){ CloseExperience(0); }


    void OnEnable() { 

        HabitatButton.onSelectHabitat += OnSelectHabitat;
        AttractTimer.onTriggerAttract += OnTriggerAttract;
    }
    void OnDisable(){ 
        
        HabitatButton.onSelectHabitat -= OnSelectHabitat;
        AttractTimer.onTriggerAttract -= OnTriggerAttract;
    }


    void OnSelectHabitat(string zoneId, HabitatButton button){

        if(IsThisZone(zoneId))
            OpenExperience(button, openDuration);
    }
    public void OpenExperience(HabitatButton button, float duration = 0){

        StopAllCoroutines();
        StartCoroutine(OpenExperienceRoutine(button, duration));
    }
    IEnumerator OpenExperienceRoutine(HabitatButton button, float duration){

        //disable interaction
        attractCG.      blocksRaycasts = false;
        experienceCG.   blocksRaycasts = false;


        //enable experience with data
        experienceCG.gameObject.SetActive(true);
        postcardMaker.Initialize(button.data);
        experienceCG.alpha = 1;

        //transition
        if (duration > 0)
        {
            experienceCG.transform.position = button.transform.position; //set experience to button location
            Vector3 localPosStart = experienceCG.transform.localPosition;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                float lerp = Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, t));

                //scale in experience from button location
                experienceCG.transform.localPosition    = Vector3.Lerp(localPosStart, Vector3.zero, lerp);
                experienceCG.transform.localScale       = Vector3.Lerp(Vector3.zero, Vector3.one, lerp);

                yield return null;
            }
        }

        //set final values
        attractCG.gameObject.SetActive(false);
        experienceCG.transform.localPosition    = Vector3.zero;
        experienceCG.transform.localScale       = Vector3.one;
        experienceCG.blocksRaycasts             = true;
    }




    void OnTriggerAttract(string zoneId){

        if(IsThisZone(zoneId))
            CloseExperience(closeDuration);
    }
    public void CloseExperience(float duration = 0){

        StopAllCoroutines();
        StartCoroutine(CloseExperienceRoutine(duration));
    }
    IEnumerator CloseExperienceRoutine(float duration){

        //disable interaction during transition
        attractCG.blocksRaycasts    = false;
        experienceCG.blocksRaycasts = false;
        attractCG.gameObject.SetActive(true);


        //transition
        if (duration > 0)
        {
            float alphaStart = experienceCG.alpha;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                //fade out experience
                experienceCG.alpha = Mathf.Lerp(1, 0, t);
                yield return null;
            }
        }        

        //enable interaction
        experienceCG.gameObject.SetActive(false);        
        attractCG.blocksRaycasts = true;     
    }
}