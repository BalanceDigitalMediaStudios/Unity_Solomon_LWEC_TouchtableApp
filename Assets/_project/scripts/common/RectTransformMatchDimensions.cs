using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RectTransformMatchDimensions : MonoBehaviour{

    [SerializeField] RectTransform  match;
    [SerializeField] bool           invertXY;

    RectTransform _rect;
    RectTransform rect{ get { if (_rect == null) _rect = GetComponent<RectTransform>(); return _rect; } }

    Vector2 previous;


    void Update(){

        if(previous != match.rect.size)
            UpdateDimensions();
    }


    void UpdateDimensions(){

        previous = match.rect.size;

        if (match && rect)
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,   invertXY ? previous.y : previous.x);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,     invertXY ? previous.x : previous.y);
        }
    }


    void OnValidate(){

        UpdateDimensions();
    }
}