using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformHelper : MonoBehaviour{

    //returns the world space bounds of a RectTransform
    public static Bounds RectTransformToBounds(RectTransform rect){

        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < 4; i++)
            bounds.Encapsulate(corners[i]);

        return bounds;
    }


    //returns true if of rectA is inside rectB including margin.  Margin is in world space units
    public static bool IsInsideRect(RectTransform a, RectTransform b, Vector2 margin = new Vector2()){

        //calculate bounds of rectA and rectB
        Bounds boundsA = RectTransformToBounds(a);
        Bounds boundsB = RectTransformToBounds(b);

        //decrease boundsB by margin
        boundsB.extents -= (Vector3)margin;

        //test if the bounds intersect
        return boundsA.Intersects(boundsB);
    }

    //gets world space width and height of a RectTransform
    public static Vector2 GetWorldSize(RectTransform rect){

        Vector2     size    = new Vector2();
        Vector3[]   corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        size.x = Mathf.Abs(corners[0].x - corners[2].x);
        size.y = Mathf.Abs(corners[0].y - corners[2].y);

        return size;
    }  
}