using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class StickerElement : MonoBehaviour {

    //Debuging

    [Header("Sticker Properties")]
    public Image stickerImage;
    public RectTransform rectTransform;

    [Header("Transform Data")]
    public Vector2 landmarkPosition;
    public Vector2 positionOffset = Vector2.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector2 scaleOffset = Vector3.zero;


    public float stickerSizeRatio = 0.5f;
    Vector2 baseResize = Vector2.one * 10;
    float resizeRate = 0.5f;
    public float translateRate = 1;
    public Vector2 contentResetSize;

    // Touch Controls
    [Header("Touch Controls")]
    public float minLongPressDistance;
    public float minLongPressTime;
    public bool isLongPress;

    protected List<int> actingTouchId = new List<int>();
    protected List<Touch> actingTouches = new List<Touch>();
    protected List<Vector2> actingTouchOffsets = new List<Vector2>();
    protected EventTrigger eventTrigger;

    //Transform
    public Vector2 dragStartPosition = Vector2.zero;
    protected Vector2 inputPrevPos;
    protected bool isAdvTransform = false;
    protected float transformScale;
    protected float transformRot;

    //Debug
    protected bool isMouseClick = false;
    protected bool isSpaceDown = false;

    [Header("Earned Settings")]
    public bool isStickerEarned;

    // Use this for initialization
    protected void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();

        //Set Event Trigger Delegates
        eventTrigger = this.GetComponent<EventTrigger>();

        EventTrigger.Entry downEntry = new EventTrigger.Entry();
        EventTrigger.Entry upEntry = new EventTrigger.Entry();
        downEntry.eventID = EventTriggerType.PointerDown;
        upEntry.eventID = EventTriggerType.PointerUp;
        downEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        upEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });

        eventTrigger.triggers.Add(downEntry);
        eventTrigger.triggers.Add(upEntry);

        InitStickerTransform(dragStartPosition);

        contentResetSize = GetComponent<Image>().sprite.rect.size * stickerSizeRatio;

        Debug.Log("Sticker's Position is: " + rectTransform.anchoredPosition);
    }

    // void OnEnable()
    // {
    //     if(rectTransform)
    //         InitStickerTransform();
    // }

    // Update is called once per frame
    protected void Update()
    {

        //Update touch input according to finger id
        for (int i = 0; i < actingTouchId.Count; i++)
        {
            if (i == actingTouches.Count)
            {
                actingTouches.Add(GetTouchFromFingerId(actingTouchId[i]));
                actingTouchOffsets.Add(GetTouchFromFingerId(actingTouchId[i]).position - (Vector2)rectTransform.position);
            }
            else
                actingTouches[i] = GetTouchFromFingerId(actingTouchId[i]);
        }

        if (isMouseClick) //Mouse Input
        {
            if (Input.GetKey(KeyCode.Space)) // Keyboard Debug
            {
                if (!isSpaceDown)
                {
                    actingTouchOffsets.Add((Vector2)Input.mousePosition - new Vector2(300, 300));
                    isSpaceDown = true;
                }
                ResizeWindow(isAdvTransform, true);
                RotateWindow(isAdvTransform, true);

                if (isAdvTransform == false)
                    isAdvTransform = true;
            }
            else
            {
                if (isSpaceDown)
                {
                    isSpaceDown = false;
                    isAdvTransform = false;
                    actingTouchOffsets[0] = Input.mousePosition - rectTransform.position;
                    actingTouchOffsets.RemoveAt(1);
                }
                MoveWindow(actingTouchOffsets[0], true);

                inputPrevPos = (Vector2)Input.mousePosition;
            }
        }
        else //Touch Input
        {
            switch (actingTouches.Count)
            {
                case 1:
                    MoveWindow(actingTouchOffsets[0], false);
                    break;
                case 2:
                    MoveWindow(actingTouchOffsets[0], false);
                    // Disabled advanced editing
                    /*
                    ResizeWindow(isAdvTransform, false);
                    RotateWindow(isAdvTransform, false);
                    
                    if (isAdvTransform == false)
                        isAdvTransform = true;
                    */
                    break;
                default:
                    break;

            }
        }


    }

    #region TOUCH_CONTROLS
    protected void MoveWindow(Vector2 offset, bool useMouse)
    {

        if(useMouse)
        {
            // # Add the delta
            rectTransform.position = (Vector2)Input.mousePosition - actingTouchOffsets[0];
        }
        else
        {
            rectTransform.anchoredPosition += (actingTouches[0].deltaPosition * translateRate);
        }
    }

    protected void ResizeWindow(bool resizeBegin, bool useMouse)
    {

        if(useMouse)
        {
            // # Disabled this for editor

            // //Debug.Log("Resize");
            // float newDistance;

            // //Use Debuging values in editor
            // if (useMouse)
            // {
            //     newDistance = Vector2.Distance(Input.mousePosition, actingTouchOffsets[1]);
            // }
            // else
            // {
            //     newDistance = Vector2.Distance(actingTouches[0].position, actingTouches[1].position);
            //     //Debug.Log(actingTouches[1].position.ToString());
            // }

            // newDistance /= rectTransform.rect.width;

            // if (!resizeBegin)
            // {
            //     transformScale = newDistance;
            // }

            // //Calculation
            // float deltaDistance = newDistance - transformScale;

            // float newDimensions = rectTransform.localScale.x + deltaDistance;

            // //Cap the max/min dimensions
            // if (newDimensions > 3f)
            // {
            //     newDimensions = 3f;
            // }
            // if (newDimensions < 0.1f)
            // {
            //     newDimensions = 0.1f;
            // }
            // rectTransform.localScale = new Vector3(newDimensions, newDimensions, newDimensions);

            // //Set new distance value
            // transformScale = newDistance;
        }
        else
        {
            // # Assign Pivot to midpoint of Touches
            Vector2 pivotStart = rectTransform.pivot;

            Touch touch1 = actingTouches[0];
            Touch touch2 = actingTouches[1];

            Vector2 touchMidpoint = (touch1.position + touch2.position) / 2;
            Vector2 positionOffset = touchMidpoint - (Vector2)rectTransform.position;

            rectTransform.pivot += new Vector2(positionOffset.x / rectTransform.sizeDelta.x, positionOffset.y / rectTransform.sizeDelta.y);

            // # Adjust the Anchored Position of the image
            rectTransform.anchoredPosition += positionOffset;

            // # Take the deltas of the Touches, combine them by direction
            // # Do this by comparing the distance of the current positions to the distance of positions plus deltas
            float scaleFactor = Vector2.Distance(touch1.position + touch1.deltaPosition, touch2.position + touch2.deltaPosition) - Vector2.Distance(touch1.position, touch2.position);


            // # Resize
            //Debug.Log(contentResetSize);
            rectTransform.sizeDelta += baseResize * scaleFactor * resizeRate;
            if (rectTransform.sizeDelta.x < contentResetSize.x/*/2*/ || rectTransform.sizeDelta.y < contentResetSize.y /*/2*/)
                rectTransform.sizeDelta = contentResetSize/*/2*/;
            if (rectTransform.sizeDelta.x > contentResetSize.x * 5 || rectTransform.sizeDelta.y > contentResetSize.y * 5)
                rectTransform.sizeDelta = contentResetSize * 5;

            stickerImage.rectTransform.sizeDelta = rectTransform.sizeDelta;
        }        



    }

    protected void RotateWindow(bool rotateBegin, bool useMouse)
    {
        float newAngleVector;
        int rotationDirection; //Vector2.Angle doesn't work around full 360 degress, this gives the rotation direction

        if (useMouse)
        {
            Vector2 vec2MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //Use Touch positions
            newAngleVector = Vector2.Angle(Vector2.up, (vec2MousePosition - actingTouchOffsets[1]).normalized);
            rotationDirection = Mathf.FloorToInt(Mathf.Sign(vec2MousePosition.x - actingTouchOffsets[1].x));
        }
        else
        {
            //Use Debuging values in editor
            newAngleVector = Vector2.Angle(Vector2.up, (actingTouches[0].position - actingTouches[1].position).normalized);
            rotationDirection = Mathf.FloorToInt(Mathf.Sign(actingTouches[0].position.x - actingTouches[1].position.x));
        }

        if (!rotateBegin)
            transformRot = newAngleVector;

        // # This must be applied to a seperate object to keep rotations consistant
        stickerImage.GetComponent<RectTransform>().Rotate(Vector3.forward, (transformRot - newAngleVector) * rotationDirection, Space.Self);
        transformRot = newAngleVector;
 
    }


    //Window Utilities

    protected void SendWindowToFront()
    {
        rectTransform.SetAsLastSibling();
    }

    public void OnPointerDown(PointerEventData data)
    {
        //Debug.Log("Pointer is down");

        if (actingTouches.Count < 2)
        {
            if (data.pointerId < 0)
            {
                isMouseClick = true;
                actingTouchOffsets.Add(Input.mousePosition - rectTransform.position);

                Debug.Log("Record Mouse Click");

            }
            else
            {
                actingTouchId.Add(data.pointerId);

                //Debug.Log("Record Touch " + data.pointerId);
                Debug.Log(actingTouches.Count);
                Debug.Log(data.position);
            }

        }

        SendWindowToFront();

        StickerManager.Instance.SetStickerTrashActive(true);

    }

    public void OnPointerUp(PointerEventData data)
    {
        //Debug.Log("Pointer is Up");

        if (!Input.GetMouseButton(0) && isMouseClick)
        {
            if (isSpaceDown)
                isSpaceDown = false;

            isMouseClick = false;
            actingTouchOffsets.Clear();
            actingTouchOffsets = new List<Vector2>();

            Debug.Log("Clear Mouse Offset");

            StickerManager.Instance.CheckTrash(this);
        }


        for (int i = 0; i < actingTouches.Count; i++)
        {
            Touch tch = actingTouches[i];

            if (tch.fingerId == data.pointerId)
            {
                actingTouches.Remove(tch);
                Debug.Log("Clearing Touch " + tch);
                actingTouchOffsets.RemoveAt(i);
                actingTouchId.RemoveAt(i);
            }
            else
            {
                actingTouchOffsets[0] = GetTouchFromFingerId(actingTouchId[0]).position - (Vector2)rectTransform.position;
            }
        }
        if (isAdvTransform)
            isAdvTransform = false;

        if(actingTouches.Count == 0)
            StickerManager.Instance.CheckTrash(this);

        positionOffset = rectTransform.anchoredPosition - landmarkPosition;
        rotationOffset = rectTransform.rotation.eulerAngles;
        scaleOffset = rectTransform.sizeDelta - new Vector2(stickerSizeRatio, stickerSizeRatio);
    }

    public Touch GetTouchFromFingerId(int fingerId)
    {
        //Look for touch according to id
        for (int j = 0; j < Input.touches.Length; j++)
        {
            if (Input.touches[j].fingerId == fingerId)
            {
                return Input.touches[j];
            }
        }
        //default to touch by touch number
        return Input.GetTouch(fingerId);

    }

    #endregion //TOUCH_CONTROLS

    void InitStickerTransform(Vector2 dragPositionNormalized)
    {
        
        // The following positioning system assumes stickers are anchored to the bottom left of the camera rect
        landmarkPosition = (dragPositionNormalized != Vector2.zero ? 
            new Vector2(Mathf.Lerp(0, StickerManager.Instance.StickerManipulationRect.rect.width, dragPositionNormalized.x), 
                        Mathf.Lerp(0, StickerManager.Instance.StickerManipulationRect.rect.height, dragPositionNormalized.y)) : 
            GetDefaultPosition());


        rectTransform.anchoredPosition = landmarkPosition + (positionOffset);

        rectTransform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);


        // The following size calculations assume that the DragAndDrop and StickerElement Prefabs have the same base size.
        //stickerImage.SetNativeSize();
        rectTransform.sizeDelta = (transform as RectTransform).sizeDelta;// * StickerManager.Instance.CameraToManipRatio;
        rectTransform.sizeDelta *= stickerSizeRatio;


        /*
        stickerImage.rectTransform.sizeDelta *= stickerSizeRatio;
        rectTransform.sizeDelta *= stickerSizeRatio;
        rectTransform.sizeDelta = rectTransform.sizeDelta + (scaleOffset);
        //stickerImage.rectTransform.sizeDelta = rectTransform.sizeDelta;
        */

        this.GetComponent<AspectRatioFitter>().aspectRatio = ((float)stickerImage.sprite.texture.width / (float)stickerImage.sprite.texture.height);


    }

    

    Vector2 GetDefaultPosition()
    {
        //return new Vector2(StickerManager.Instance.stickerParent.rect.size.x / 2, -StickerManager.Instance.stickerParent.rect.size.y / 2);
        Vector2 rectSize = new Vector2(StickerManager.Instance.StickerManipulationRect.rect.width, StickerManager.Instance.StickerManipulationRect.rect.height);
        Debug.Log(new Vector2(rectSize.x / 2, rectSize.y / 2));
        return new Vector2(rectSize.x / 2, rectSize.y / 2);
    }

    

    public void GetLandmarkPostition()
    {
        // if(TakePhotoMenu.Instance.isFace)
        //     InitStickerTransform();
    }

    public void DeleteSticker()
    {
        StickerManager.Instance.RemoveSticker(this);
        Destroy(this.gameObject);
    }
}
