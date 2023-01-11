using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Poll_Answer : MonoBehaviour{

    public delegate void    PollAnswerEvent (Poll_Answer answer);
    public event            PollAnswerEvent onSelectAnswer;


    [SerializeField, ReadOnly] string _answerId;
    public string answerId{get { return _answerId; } }

    [SerializeField] UITransitionSlide _slide;
    public UITransitionSlide slide{get { return _slide; } }


    [SerializeField] Button             button;
    [SerializeField] TextMeshProUGUI    answerText;
    [SerializeField] GameObject         highlight;

    

    void Awake(){

        button.onClick.AddListener(ButtonAction);
    }


    public void Initialize(string answerId, string answer){

        _answerId       = answerId;
        answerText.text = answer;
        highlight.SetActive(false);
    }


    void ButtonAction(){

        highlight.SetActive(true);

        if(onSelectAnswer != null)
            onSelectAnswer(this);
    }


    public void Exit(float xPos, float duration, float delay){

        Vector3 endPos = (transform as RectTransform).anchoredPosition3D;
        endPos.x = xPos;

        slide.TransitionToPosition(endPos, duration, delay);
    }
}