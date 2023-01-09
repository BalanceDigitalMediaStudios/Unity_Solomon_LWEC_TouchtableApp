using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActivation : MonoBehaviour{

    [System.Serializable]
    protected struct TimingGroup{

        public GameObject   gameObject;
        public float        delay;
    }

    [SerializeField] float          globalDelay = 0;
    [SerializeField] TimingGroup[]  timing;



    void OnEnable(){

        ActivateTimedObjects();
    }

    public void ActivateTimedObjects(){

        StopAllCoroutines();
        foreach(TimingGroup g in timing)
            StartCoroutine(ActivateRoutine(g));
    }


    IEnumerator ActivateRoutine(TimingGroup group){

        float delay = group.delay + globalDelay;
        if (delay > 0)
        {
            group.gameObject.SetActive(false);
            yield return new WaitForSeconds(delay);
        }

        group.gameObject.SetActive(true);
    }
}