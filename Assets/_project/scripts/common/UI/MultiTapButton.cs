using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class MultiTapButton : MonoBehaviour{

    public int minTaps = 5;
    [SerializeField][ReadOnly]int currentTaps;

    public float maxInterval = .5f;
    [SerializeField][ReadOnly]float currentInterval;

    Button button;


    [System.Serializable]
    public class MultiTapEvent : UnityEvent{};

    [Header("Events")]
    public MultiTapEvent onMultiTapSuccess;
    public MultiTapEvent onMultiTapFailure;


    void Awake(){

        button = GetComponent<Button>();

        button.onClick.AddListener(ButtonAction);
    }


    void Update(){

        if(currentTaps > 0 && currentInterval > 0)
        {
            currentInterval = Mathf.Clamp(currentInterval-Time.deltaTime, 0, maxInterval);

            if(currentInterval <= 0)
            {
                ResetTaps();
                ResetInterval();
                Failure();
            }
        }
    }

    void ButtonAction(){

        currentTaps++;

        if(currentTaps >= minTaps)
        {
            ResetTaps();
            Success();
        }
        ResetInterval();
    }

    void ResetTaps(){

        currentTaps = 0;
    }
    void ResetInterval(){

        currentInterval = maxInterval;
    }

    void Success(){

        Debug.LogFormat("Multitap <color=yellow>[{0}]</color> Success", name);
        onMultiTapSuccess.Invoke();
    }

    void Failure(){

        Debug.LogFormat("Multitap <color=yellow>[{0}]</color> Failure", name);
        onMultiTapFailure.Invoke();
    }
}