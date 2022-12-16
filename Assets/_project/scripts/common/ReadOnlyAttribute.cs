using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeEditor: PropertyDrawer{

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label){

		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		
		EditorGUI.BeginDisabledGroup(true);
		EditorGUI.PropertyField(position, property, label, true);
		EditorGUI.EndDisabledGroup();
	}
}
#endif

public class ReadOnlyAttribute : PropertyAttribute {}