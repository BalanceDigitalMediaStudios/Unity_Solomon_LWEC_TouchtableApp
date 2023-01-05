using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSticker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{

    [SerializeField] Image              image;
    [SerializeField] float              draggingScaleMultiplier = 1.2f;   
    [SerializeField, ReadOnly] Vector2  worldSize               = Vector2.zero;

    [Header("Removal")]
    [Tooltip("Percentage of sticker's rect that must be inside the habitat image to not be destroyed.")]
    [SerializeField, Range(0,1)] float  removalTolerance            = .8f;

    [SerializeField] Vector2            removalDistance;  //range of distance from trash bin to determine transition duration
    [SerializeField] Vector2            removalDuration;
    [SerializeField] AnimationCurve     removalTimingCurve;

    [SerializeField] float              removalMaxVerticalOffset    = -1f;
    [SerializeField] AnimationCurve     removalVerticalOffsetCurve;

    Vector3 originalScale;
    Vector3 dragStart, offset;

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
    PostcardMaker _postcardMaker;
    PostcardMaker postcardMaker { get { if (_postcardMaker == null) _postcardMaker = GetComponentInParent<PostcardMaker>(true); return _postcardMaker; } }


    public void Initialize(Sprite sprite){

        image.sprite = sprite;
    }

    void Start(){

        originalScale = transform.localScale;
        canvasGroup.blocksRaycasts = true;        
    }

    public void OnPointerDown(PointerEventData eventData){

        Debug.LogFormat("OnPointerDown: {0}", name);        
        transform.localScale    = originalScale * draggingScaleMultiplier;
        offset                  = transform.position - eventData.pointerCurrentRaycast.worldPosition;        
        transform.SetParent(postcardMaker.draggableContainer.transform);        
        UpdateWorldSize();
    }

    public void OnPointerUp(PointerEventData eventData){

        Debug.LogFormat("OnPointerUp: {0}", name);
        transform.localScale = originalScale;
        transform.SetParent(postcardMaker.habitatImage.transform);
        UpdateWorldSize();
    }


    public void OnBeginDrag(PointerEventData eventData){

        Debug.LogFormat("OnBeginDrag: {0}", name);
        canvasGroup.blocksRaycasts = false;

        dragStart = eventData.pointerCurrentRaycast.worldPosition;        
    }

    public void OnDrag(PointerEventData eventData){

        //ignore if pointer is off screen
        if(eventData.pointerCurrentRaycast.gameObject == null)
            return;

        Vector3 delta       = eventData.pointerCurrentRaycast.worldPosition - dragStart;
        transform.position  = ClampStickerPosition(dragStart + delta + offset, .5f * worldSize);
    }

    Vector3 ClampStickerPosition(Vector3 position, Vector2 margin = new Vector2()){

        //sticker can't go beyond the bounds of the container including margin
        position.x = Mathf.Clamp(position.x, postcardMaker.draggableContainerBounds.min.x + margin.x, postcardMaker.draggableContainerBounds.max.x - margin.x);
        position.y = Mathf.Clamp(position.y, postcardMaker.draggableContainerBounds.min.y + margin.y, postcardMaker.draggableContainerBounds.max.y - margin.y);

        return position;
    }

    public void OnEndDrag(PointerEventData eventData){
        
        Debug.LogFormat("OnEndDrag {0}", name);

        //remove sticker if not inside habitat image (within margins)
        if (!IsInsideRect(transform as RectTransform, postcardMaker.habitatImage.rectTransform, removalTolerance * worldSize))
        {
            RemoveSticker(1);
            return;
        }

        canvasGroup.blocksRaycasts = true;
    }

    void UpdateWorldSize(){

        Vector3[] corners = new Vector3[4];
        (transform as RectTransform).GetWorldCorners(corners);
        worldSize.x = Mathf.Abs(corners[0].x - corners[2].x);
        worldSize.y = Mathf.Abs(corners[0].y - corners[2].y);
    }

    


    //returns true if of rectA is inside rectB including margin
    bool IsInsideRect(RectTransform rectA, RectTransform rectB, Vector2 margin = new Vector2()){

        //calculate bounds of rectA and rectB
        Bounds boundsA = GetBounds(rectA);
        Bounds boundsB = GetBounds(rectB);

        //decrease boundsB by margin
        boundsB.extents -= (Vector3)margin;

        //test if the bounds intersect
        return boundsA.Intersects(boundsB);
    }

    Bounds GetBounds(RectTransform rect){

        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < 4; i++)
            bounds.Encapsulate(corners[i]);

        return bounds;
    }


    void RemoveSticker(float durationMultiplier = 0){

        StartCoroutine(RemoveStickerRoutine(durationMultiplier));        
    }

    IEnumerator RemoveStickerRoutine(float durationMultiplier){

        Debug.LogFormat("Removed Sticker: {0}", name);
    
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(postcardMaker.draggableContainer.transform);


        //get distance from sticker to trash bin
        Vector3 posStart    = transform.position;
        Vector3 posEnd      = postcardMaker.trashBinRect.TransformPoint(postcardMaker.trashBinRect.rect.center);
        float distance      = Vector3.Distance(posStart, posEnd);

        //determine duration based on distance and multiplier
        Vector2 durationRange   = durationMultiplier * removalDuration;
        float distanceNormal    = (distance - removalDistance.x) / (removalDistance.y - removalDistance.x);
        float duration          = Mathf.Lerp(durationRange.x, durationRange.y, distanceNormal);

        if (duration > 0)
        {
            Vector3 scaleStart  = transform.localScale;
            Vector3 scaleEnd    = transform.localScale * .5f;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                float interp = removalTimingCurve.Evaluate(t);

                //calculate vertical offset
                Vector3 offset = Vector3.zero;
                offset.y = removalMaxVerticalOffset * removalVerticalOffsetCurve.Evaluate(t);

                //update position and scale
                transform.position      = Vector3.LerpUnclamped(posStart, posEnd, interp) + offset;
                transform.localScale    = Vector3.LerpUnclamped(scaleStart, scaleEnd, interp);

                yield return null;
            }
        }

        postcardMaker.OnRemoveSticker();
        Destroy(gameObject);
    }
}