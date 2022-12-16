using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoTranslate : MonoBehaviour{

    [SerializeField]private Vector3     rate;
    [SerializeField]private Transform   roomTransform;
    [SerializeField]private bool        autoResetAtEndOfRoom;
    [SerializeField]private KeyCode     resetKey = KeyCode.Space;

    private Vector3 originalPosition;

    void Awake(){

        originalPosition = transform.position;
    }

    void Update(){

        transform.Translate(rate * Time.deltaTime, Space.Self);

        if(autoResetAtEndOfRoom && roomTransform != null && -transform.position.z > roomTransform.localScale.z)
            ResetPosition();

        if(Input.GetKeyDown(resetKey))
            ResetPosition();            
    }

    void ResetPosition(){

        transform.position = originalPosition;
    }
}