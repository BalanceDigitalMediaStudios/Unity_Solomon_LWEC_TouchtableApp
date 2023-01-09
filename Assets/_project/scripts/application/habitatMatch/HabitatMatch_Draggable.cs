using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HabitatMatch_Draggable : Draggable{

    [SerializeField] Image          image;
    [SerializeField] float          draggingScaleMultiplier = 1.2f;
    [SerializeField] RectTransform  boundaryRect;


    Vector3 originalScale;
    Vector2 margin;
    Bounds _bounds;  //boundaryRect converted to bounds, prevents issue with scale and orientation
    public Bounds bounds{
        get{ if (_bounds.size == Vector3.zero) _bounds = RectTransformHelper.RectTransformToBounds(boundaryRect); return _bounds; } }


    CanvasGroup _canvasGroup;
    CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }


    
    


    void OnEnable(){ canvasGroup.blocksRaycasts = true; }


    void Awake(){

        originalScale   = transform.localScale;
        margin          = .5f * draggingScaleMultiplier * RectTransformHelper.GetWorldSize(transform as RectTransform);
    }

    public void Initialize(Sprite sprite){ image.sprite = sprite; }


    public override void OnPointerDown(PointerEventData eventData){

        Resize(originalScale * draggingScaleMultiplier);
        base.OnPointerDown(eventData);        
    }

    public override void OnPointerUp(PointerEventData eventData){

        Resize(originalScale);
        base.OnPointerUp(eventData);        
    }

    public override void OnBeginDrag(PointerEventData eventData){

        canvasGroup.blocksRaycasts = false;
        base.OnBeginDrag(eventData);        
    }

    public override void OnDrag(PointerEventData eventData){

        base.OnDrag(eventData);
        transform.position = ClampPosition(transform.position, margin);
    }
    public override void OnEndDrag(PointerEventData eventData){

        canvasGroup.blocksRaycasts = true;
        base.OnEndDrag(eventData);        
    }




    void Resize(Vector3 scale){ transform.localScale = scale; }

    Vector3 ClampPosition(Vector3 position, Vector2 margin = default(Vector2)){

        //can't go beyond bounds including margin
        position.x = Mathf.Clamp(position.x, bounds.min.x + margin.x, bounds.max.x - margin.x);
        position.y = Mathf.Clamp(position.y, bounds.min.y + margin.y, bounds.max.y - margin.y);

        return position;
    }
}