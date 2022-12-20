using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


/// <summary>
/// Adds a contextual menu item to rect transforms.
/// Updates rect transform's anchors so that relative position remains the same, while anchored position is zeroed.
/// Essentially sets the rect transform's pivot to the normalized local position of the parent object
/// </summary>
public class SetRectAnchorsToPosition : EditorWindow{

    [MenuItem("CONTEXT/RectTransform/Set Anchors to Position")]
    static void SetAnchorsToPosition(MenuCommand command){

        //allow this command to be undoable
        Undo.RegisterCompleteObjectUndo(command.context, "Set Rect Transform's Anchors to Position");


        RectTransform rect      = (RectTransform)command.context;
        RectTransform container = (RectTransform)rect.parent;


        //determine what type of canvas we are working with
        Canvas  canvas      = rect.GetComponentInParent<Canvas>(true);
        Camera  canvasCam   = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;


        //get screen position of rect and convert to parent's local position
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvasCam, rect.position);        
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, canvasCam, out localPos);
        //Debug.LogFormat("Screen Pos: {0}", screenPoint);
        //Debug.LogFormat("Local Pos: {0}", localPos);


        //get local position of bottom left corner of container (index = 0) and apply as an offset
        Vector3[] containerCorners = new Vector3[4];
        container.GetLocalCorners(containerCorners);
        Vector2 cornerPos = new Vector2(containerCorners[0].x, containerCorners[0].y);        
        localPos -= cornerPos;
        //Debug.LogFormat("Corner Pos: {0}", cornerPos);


        //set normalized position as anchors and zero out anchored position
        rect.anchorMin = rect.anchorMax = new Vector2( localPos.x / container.rect.width, localPos.y / container.rect.height );
        rect.anchoredPosition = Vector2.zero;
        //Debug.LogFormat("Normal Pos: {0}", normalPos);
    }
}