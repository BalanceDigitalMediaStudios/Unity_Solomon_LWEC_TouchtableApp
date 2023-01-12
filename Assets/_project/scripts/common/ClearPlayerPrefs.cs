using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ClearPlayerPrefs))]
public class ClearPlayerPrefsEditor : Editor{

    ClearPlayerPrefs t;

    protected virtual void OnEnable(){

        t = (ClearPlayerPrefs)target;
    }

    public override void OnInspectorGUI(){

        base.OnInspectorGUI();

        serializedObject.Update();        

        if(GUILayout.Button("Clear Now"))
            t.Clear();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif



public class ClearPlayerPrefs : MonoBehaviour{

    [SerializeField] bool clearOnAwake = false;

    void Awake(){

        if(clearOnAwake)
            Clear();
    }

    public void Clear(){

        PlayerPrefs.DeleteAll();
        Debug.Log("All Player Prefs Deleted");
    }
}