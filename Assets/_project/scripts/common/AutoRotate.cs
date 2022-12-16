using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour{

    public Vector3 rate;

    Vector3 originalEuler;

    void Awake(){

        originalEuler = transform.localEulerAngles;
    }

    void OnEnable(){

        transform.localEulerAngles = originalEuler;
    }

    void Update(){

        transform.Rotate(rate * Time.deltaTime, Space.Self);
    }
}