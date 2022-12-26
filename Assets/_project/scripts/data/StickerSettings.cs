using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class StickerSettings{

    #if UNITY_EDITOR
    [HideInInspector]public string name; //used purely for nicely naming inspector array elements
    #endif

    public enum UnlockMethod
    {
        unlocked,
        pollQuestion,
        habitatMatch
    }



    [SerializeField] StickerData _sticker;
    public StickerData sticker{ get { return _sticker; } }

    [SerializeField] UnlockMethod _unlockMethod;
    public UnlockMethod unlockMethod{get { return _unlockMethod; } }

    [SerializeField] PollQuestionData _pollQuestion;
    public PollQuestionData pollQuestion{get { return _pollQuestion; } }

    [SerializeField] HabitatMatchData _habitatMatch;
    public HabitatMatchData habitatMatch{get { return _habitatMatch; } }
}