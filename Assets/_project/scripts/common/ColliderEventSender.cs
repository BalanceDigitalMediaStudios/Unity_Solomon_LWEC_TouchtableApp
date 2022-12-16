using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(ColliderEventSender))]
public class ColliderEventSenderEditor : Editor{

    SerializedProperty  m_logEvents;
    SerializedProperty  m_logStayEvents;
    SerializedProperty  m_showCollisionPoints;
    SerializedProperty  m_collisionNormalsLength;
    SerializedProperty  m_collisionCrosshairSize;


    void OnEnable(){

        m_logEvents                 = serializedObject.FindProperty("logEvents");
        m_logStayEvents             = serializedObject.FindProperty("logStayEvents");
        m_showCollisionPoints       = serializedObject.FindProperty("showCollisionPoints");
        m_collisionNormalsLength    = serializedObject.FindProperty("collisionNormalsLength");
        m_collisionCrosshairSize    = serializedObject.FindProperty("collisionCrosshairSize");
    }

    public override void OnInspectorGUI(){

        serializedObject.Update();


        EditorGUILayout.PropertyField(m_logEvents);
        EditorGUI.BeginDisabledGroup(!m_logEvents.boolValue);
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_logStayEvents);
            EditorGUI.indentLevel--;
        }EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(m_showCollisionPoints);        
        if(m_showCollisionPoints.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_collisionNormalsLength);
            EditorGUILayout.PropertyField(m_collisionCrosshairSize);
            EditorGUI.indentLevel--;
        }


        serializedObject.ApplyModifiedProperties();
    }
}


#endif

public class ColliderEventSender : MonoBehaviour{

    public delegate void    CollisionEvent  (Collision collision);
    public event            CollisionEvent  onCollisionEnter, onCollisionExit, onCollisionStay;

    public delegate void    TriggerEvent    (Collider collider);
    public event            TriggerEvent    onTriggerEnter, onTriggerExit, onTriggerStay;


    [SerializeField]bool    logEvents              = false;
    [SerializeField]bool    logStayEvents          = false;
    [SerializeField]bool    showCollisionPoints    = false;
    [SerializeField]float   collisionNormalsLength = .5f;
    [SerializeField]float   collisionCrosshairSize = .25f;



    public void OnCollisionEnter(Collision collision){

        LogEvent(this, "OnCollisionEnter [{0}]: {1}", name, collision.gameObject.name);

        if(showCollisionPoints)
            ShowCollisionPoints(collision);

        if(onCollisionEnter != null)
            onCollisionEnter(collision);
    }
    public void OnCollisionExit(Collision collision){

        LogEvent(this, "OnCollisionExit [{0}]: {1}", name, collision.gameObject.name);

        if(onCollisionExit != null)
            onCollisionExit(collision);
    }
    public void OnCollisionStay(Collision collision){

        if(logStayEvents)
            LogEvent(this, "OnCollisionStay [{0}]: {1}", name, collision.gameObject.name);

        if(showCollisionPoints)
            ShowCollisionPoints(collision);

        if(onCollisionStay != null)
            onCollisionStay(collision);
    }



    public void OnTriggerEnter(Collider collider){

        LogEvent(this, "OnTriggerEnter [{0}]: {1}", name, collider.name);

        if(onTriggerEnter != null)
            onTriggerEnter(collider);
    }
    public void OnTriggerExit(Collider collider){

        LogEvent(this, "OnTriggerExit [{0}]: {1}", name, collider.name);

        if(onTriggerExit != null)
            onTriggerExit(collider);
    }
    public void OnTriggerStay(Collider collider){

        if(logStayEvents)
            LogEvent(this, "OnTriggerStay [{0}]: {1}", name, collider.name);

        if(onTriggerStay != null)
            onTriggerStay(collider);
    }



    void LogEvent(Object context, string message, params object[] args){
        
        if(logEvents)
            Debug.LogFormat(context, message, args);
    }
    void ShowCollisionPoints(Collision collision){

        for(int i = 0; i < collision.contactCount; i++)
        {   
            ContactPoint contact = collision.GetContact(i);

            float crossHairLength = collisionCrosshairSize * .5f;

            Debug.DrawRay(contact.point, contact.normal * collisionNormalsLength, Color.white);
            Debug.DrawLine(contact.point - crossHairLength * Vector3.left,     contact.point + crossHairLength * Vector3.left,    Color.yellow);
            Debug.DrawLine(contact.point - crossHairLength * Vector3.up,       contact.point + crossHairLength * Vector3.up,      Color.yellow);
            Debug.DrawLine(contact.point - crossHairLength * Vector3.forward,  contact.point + crossHairLength * Vector3.forward, Color.yellow);
        }
    }
}