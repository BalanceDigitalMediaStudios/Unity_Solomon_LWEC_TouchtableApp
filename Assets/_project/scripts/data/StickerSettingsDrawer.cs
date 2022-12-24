using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(HabitatData.StickerSettings))]
public class StickerSettingsDrawer : PropertyDrawer{

    float lineHeight = EditorGUIUtility.singleLineHeight;
    float spacing    = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

    SerializedProperty  m_name;
    SerializedProperty  m_unlockMethod;
    SerializedProperty  m_sticker;
    SerializedProperty  m_pollQuestion;
    SerializedProperty  m_habitatMatch;


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){

        m_unlockMethod  = property.FindPropertyRelative("_unlockMethod");

        int lines = 0;
        if (property.isExpanded)
        {
            lines = 2;
            if (m_unlockMethod.enumValueIndex != (int)UnlockMethod.unlocked)
                lines = 3;
        }

        return (spacing * (lines + 1));
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){

        m_name          = property.FindPropertyRelative("name");
        m_unlockMethod  = property.FindPropertyRelative("_unlockMethod");
        m_sticker       = property.FindPropertyRelative("_sticker");
        m_pollQuestion  = property.FindPropertyRelative("_pollQuestion");
        m_habitatMatch  = property.FindPropertyRelative("_habitatMatch");

		
		EditorGUI.BeginProperty(position, label, property);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;


        Rect foldoutRect    = new Rect(position.x, position.y,                  position.width, lineHeight);
        Rect stickerRect    = new Rect(position.x, position.y + spacing,        position.width, lineHeight);
        Rect unlockRect     = new Rect(position.x, position.y + 2 * spacing,    position.width, lineHeight);
        Rect optionalRect   = new Rect(position.x, position.y + 3 * spacing,    position.width, lineHeight);


        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, m_name.stringValue, EditorStyles.foldout);
        if(property.isExpanded)
        {
            EditorGUI.PropertyField(stickerRect,    m_sticker);
            EditorGUI.PropertyField(unlockRect,     m_unlockMethod);

            if(m_unlockMethod.enumValueIndex == (int)UnlockMethod.pollQuestion)
                EditorGUI.PropertyField(optionalRect, m_pollQuestion);

            if(m_unlockMethod.enumValueIndex == (int)UnlockMethod.habitatMatch)
                EditorGUI.PropertyField(optionalRect, m_habitatMatch);
        }
        EditorGUI.EndFoldoutHeaderGroup();


        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
	}
}