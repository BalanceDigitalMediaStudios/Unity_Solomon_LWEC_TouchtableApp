using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnlockMethod{
    unlocked,
    pollQuestion,
    habitatMatch
}

[CreateAssetMenu(fileName = "habitatData", menuName = "Data/Habitat Data")]
public class HabitatData : ScriptableObject{

    [SerializeField] string _longName;
    public string longName{get { return _longName; } }

    [SerializeField, SpritePreview(200)] Sprite _sprite;
    public Sprite sprite{get { return _sprite; } }

    

    [System.Serializable]
    public class StickerSettings{

        #if UNITY_EDITOR
        [HideInInspector]public string name; //used purely for nicely naming inspector array elements
        #endif

        [SerializeField] StickerData _sticker;
        public StickerData sticker{ get { return _sticker; } }

        [SerializeField] UnlockMethod _unlockMethod;
        public UnlockMethod unlockMethod{get { return _unlockMethod; } }

        [SerializeField] PollQuestionData _pollQuestion;
        public PollQuestionData pollQuestion{get { return _pollQuestion; } }

        [SerializeField] HabitatMatchData _habitatMatch;
        public HabitatMatchData habitatMatch{get { return _habitatMatch; } }
    }
    [SerializeField] StickerSettings[] _stickers;
    public StickerSettings[] stickers{get { return _stickers; } }




    void OnValidate(){

        for (int i = 0; i < stickers.Length; i++)
        {
            if (stickers[i].sticker)
                stickers[i].name = stickers[i].sticker.longName;
            else
                stickers[i].name = string.Format("Element {0}", i);
        }
    }
}