using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PollQuestionData))]
public class PollQuestionDataEditor : Editor{

    SerializedProperty m_question;
    SerializedProperty m_answers;

    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


    protected virtual void OnEnable(){

        m_question  = serializedObject.FindProperty("_question");
        m_answers   = serializedObject.FindProperty("_answers");
    }

    public override void OnInspectorGUI(){

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_question);
        EditorGUILayout.Space();
        DrawAnswers();       

        serializedObject.ApplyModifiedProperties();
    }

    void DrawAnswers(){

        int textAreaLines = 2;

        EditorGUILayout.LabelField("Answers");


        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        for (int i = 0; i < m_answers.arraySize; i++)
        {
            SerializedProperty a = m_answers.GetArrayElementAtIndex(i);

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(letters[i].ToString(), GUILayout.Width(25));
            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;
            a.stringValue = EditorGUILayout.TextArea(a.stringValue, style, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * textAreaLines));

            EditorGUILayout.EndHorizontal();            
        }

        EditorGUI.indentLevel = indent;
    }
}
#endif


[CreateAssetMenu(fileName = "poll", menuName = "Data/Poll Question Data")]
public class PollQuestionData : ScriptableObject{

    [SerializeField, TextArea(4, 4)] string _question;
    public string question{get { return _question; } }

    const int ANSWERS = 4;
    [SerializeField] string[] _answers = new string[ANSWERS];
    public string[] answers{get { return _answers; } }    
}