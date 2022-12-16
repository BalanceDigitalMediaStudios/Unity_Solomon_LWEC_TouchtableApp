using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AimAtTarget : MonoBehaviour{

    [Tooltip("Defaults to main camera if null")]
    public Transform    target;
    public GameObject   upObject;
    public Vector3      offset;


    void Awake(){

        if(target == null)
            target = Camera.main.transform;
    }

    void Update(){
        
        if(upObject != null)
        {
            transform.LookAt(target.transform, upObject.transform.up);
            transform.Rotate(offset, Space.Self);
        }
        else
        {
            transform.LookAt(Camera.main.transform, Vector3.up);
            transform.Rotate(offset, Space.Self);
        }
    }
}