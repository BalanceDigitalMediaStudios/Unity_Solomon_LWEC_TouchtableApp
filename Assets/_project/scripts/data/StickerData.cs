using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "stickerData", menuName = "Data/Sticker Data")]
public class StickerData : ScriptableObject{

    [SerializeField] string _longName;
    public string longName{get { return _longName; } }    

    [SerializeField, SpritePreview(150)] Sprite _sprite;
    public Sprite sprite{get { return _sprite; } }
}