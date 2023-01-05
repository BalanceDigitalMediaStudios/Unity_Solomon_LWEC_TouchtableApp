using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostcardMaker : MonoBehaviour{

    //holds the rect that holds all created draggable stickers
    [SerializeField] RectTransform _draggableContainer;
    public RectTransform draggableContainer{get { return _draggableContainer; } }

    //World space bound of the draggable stickers container
    Bounds _draggableContainerBounds;
    public Bounds draggableContainerBounds{
        
        get{

            if (_draggableContainerBounds.size == Vector3.zero)
            {
                //get world corners of container
                Vector3[] corners = new Vector3[4];
                draggableContainer.GetWorldCorners(corners);

                //encapsulate corners into bounds to negate any rotation, scaling, etc due to zone placement
                _draggableContainerBounds = new Bounds(corners[0], Vector3.zero);
                for (int i = 1; i < 4; i++)
                    _draggableContainerBounds.Encapsulate(corners[i]);
            }
            return _draggableContainerBounds;
        }
    }

    //the image that stickers will be dragged onto
    [SerializeField] Image _habitatImage;
    public Image habitatImage{get { return _habitatImage; } }

    [SerializeField] Animator _trashBinAnimator;
    public Animator         transBinAnimator{get { return _trashBinAnimator; } }
    public RectTransform    trashBinRect{ get { return transBinAnimator.transform as RectTransform; } }

    [Header("Sticker Area")]
    [SerializeField] Transform  unlockedGroup;
    [SerializeField] Transform  lockedGroup;
    [SerializeField] GameObject maxStickersMessage;

    [Header("Data")]
    [SerializeField, ReadOnly] HabitatData data;

    public void OnRemoveSticker(){

        transBinAnimator.CrossFadeInFixedTime("pulse", .25f, -1, 0f);
    }
}