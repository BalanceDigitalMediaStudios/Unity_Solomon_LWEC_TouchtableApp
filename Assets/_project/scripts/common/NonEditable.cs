using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(NonEditable))]
public class NonEditableEditor : Editor{

    Transform t;

    protected virtual void OnEnable(){

        t = (target as NonEditable).transform;

        Tools.hidden    = true;
        t.hideFlags     = HideFlags.NotEditable;        
    }
    protected virtual void OnDisable(){

        Tools.hidden    = false;

        if(t != null)
            t.hideFlags = HideFlags.None;        
    }

    public override void OnInspectorGUI(){
        
        EditorGUI.BeginDisabledGroup(true);
        GUILayout.TextArea(
            "All direct transform control on this object is disabled " +
            "and can only be controlled via other scripts."
            );
        EditorGUI.EndDisabledGroup();
    }
}

#endif



public class NonEditable : MonoBehaviour{}