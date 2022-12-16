using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreMathf : MonoBehaviour{

    ///<summary>Locally scales transform from a point in world coordinates.  Doing so adjusts the position of the transform as well.</summary>
    public static void ScaleAround(Transform target, Vector3 point, Vector3 scale){

        Vector3 a = target.position;
        Vector3 b = point;

        Vector3 difference          = a - b;  //diference from target pivot to desired scale pivot

        float   relativeScaleX      = scale.x / target.localScale.x;
        float   relativeScaleY      = scale.y / target.localScale.y;
        float   relativeScaleZ      = scale.z / target.localScale.z;



        //calculate the final position after scaling
        Vector3 finalPosition;
        finalPosition.x = b.x + difference.x * relativeScaleX;
        finalPosition.y = b.y + difference.y * relativeScaleY;
        finalPosition.z = b.z + difference.z * relativeScaleZ;


        //perform scale and translation
        target.localScale   = scale;        
        target.position     = finalPosition;
    }
}
