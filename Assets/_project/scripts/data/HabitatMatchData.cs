using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(HabitatMatchData))]
public class HabitatMatchDataEditor : Editor{

    SerializedProperty m_choices;
    SerializedProperty m_correctIndex;
    SerializedProperty m_flavorText;


    protected virtual void OnEnable(){

        m_choices       = serializedObject.FindProperty("_choices");
        m_correctIndex  = serializedObject.FindProperty("_correctIndex");
        m_flavorText    = serializedObject.FindProperty("_flavorText");
    }

    public override void OnInspectorGUI(){

        serializedObject.Update();

        DrawChoices();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField(string.Format("Correct Index: {0}", m_correctIndex.intValue.ToString()));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(m_flavorText);        

        serializedObject.ApplyModifiedProperties();
    }

    void DrawChoices(){

            //GUILayout doesn't listen to indentLevel, so for those cases, we have to insert spacing and resize as needed
            float indentWidth = EditorGUI.indentLevel * 15; 
            float col01Width  = 75 - indentWidth;
            float radioOffset = 20;

            //key
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Is Correct", GUILayout.Width(col01Width));
            EditorGUILayout.LabelField("Habitat");

            EditorGUILayout.EndHorizontal();
            
            
            //choices
            int correctIndex = m_correctIndex.intValue;
            EditorGUI.BeginChangeCheck();
            {
                for (int i = 0; i < m_choices.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    //"Radio" style forces use of GUILayout, i.e. we have to manually add indent for the radio button
                    GUILayout.Space (indentWidth + radioOffset);
                    if(EditorGUILayout.Toggle(i == correctIndex, "Radio", GUILayout.Width(col01Width - radioOffset)))
                        correctIndex = i;
                    EditorGUILayout.PropertyField(m_choices.GetArrayElementAtIndex(i), GUIContent.none);

                    EditorGUILayout.EndHorizontal();                    
                }

            }if(EditorGUI.EndChangeCheck())
                m_correctIndex.intValue = correctIndex;
    }
}
#endif


[CreateAssetMenu(fileName = "match", menuName = "Data/Habitat Match Data")]
public class HabitatMatchData : ScriptableObject{

    [SerializeField] int _correctIndex = 0;
    public int          correctIndex{get { return _correctIndex; } }
    public HabitatData  correctHabitat{get { return choices[correctIndex]; } }

    const int CHOICES = 3;
    [SerializeField] HabitatData[] _choices = new HabitatData[CHOICES];
    public HabitatData[] choices{get { return _choices; } }    

    [SerializeField, TextArea(3, 3)] string _flavorText;
    public string flavorText{get { return _flavorText; } }
}