using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "pollQuestionData", menuName = "Data/Poll Question Data")]
public class PollQuestionData : ScriptableObject{

    [SerializeField] string _question;
    public string question{get { return _question; } }

    const int ANSWERCOUNT = 4;
    [SerializeField] string[] _answers = new string[ANSWERCOUNT];
    public string[] answers{get { return _answers; } }    


    void OnValidate(){

        if(_answers.Length != ANSWERCOUNT)
            System.Array.Resize(ref _answers, ANSWERCOUNT);
    }
}