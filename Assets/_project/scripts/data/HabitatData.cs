using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "habitat", menuName = "Data/Habitat Data")]
public class HabitatData : ScriptableObject{

    [SerializeField] string _longName;
    public string longName{get { return _longName; } }

    [SerializeField, SpritePreview(200)] Sprite _sprite;
    public Sprite sprite{get { return _sprite; } }

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