using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TMP_TypeOutEffect))]
public class TMP_TypeOutEffectEditor : Editor{

    SerializedProperty m_playOnEnable;
    //SerializedProperty m_loop;

    SerializedProperty m_preDelay;
    SerializedProperty m_timingMethod;
    SerializedProperty m_charInterval;
    SerializedProperty m_duration;
    SerializedProperty m_postDelay;

    SerializedProperty m_characters;
    SerializedProperty m_index;
    SerializedProperty m_onFinish;


    protected virtual void OnEnable(){

        m_playOnEnable  = serializedObject.FindProperty("playOnEnable");
        //m_loop          = serializedObject.FindProperty("loop");

        m_preDelay      = serializedObject.FindProperty("preDelay");
        m_timingMethod  = serializedObject.FindProperty("timingMethod");
        m_charInterval  = serializedObject.FindProperty("charInterval");
        m_duration      = serializedObject.FindProperty("duration");
        m_postDelay     = serializedObject.FindProperty("postDelay");

        m_characters    = serializedObject.FindProperty("characters");
        m_index         = serializedObject.FindProperty("_index");
        m_onFinish      = serializedObject.FindProperty("onFinish");
    }

    public override void OnInspectorGUI(){

        serializedObject.Update();

        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

        EditorGUILayout.PropertyField(m_playOnEnable);
        //EditorGUILayout.PropertyField(m_loop);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_timingMethod);
        if((TMP_TypeOutEffect.TimeSetting)m_timingMethod.enumValueIndex == TMP_TypeOutEffect.TimeSetting.perCharacter)
            EditorGUILayout.PropertyField(m_charInterval, new GUIContent("Interval Per Character"));        
        if((TMP_TypeOutEffect.TimeSetting)m_timingMethod.enumValueIndex == TMP_TypeOutEffect.TimeSetting.totalDuration)
            EditorGUILayout.PropertyField(m_duration);

        EditorGUILayout.PropertyField(m_preDelay);
        EditorGUILayout.PropertyField(m_postDelay);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_characters);
        EditorGUILayout.PropertyField(m_index);
        EditorGUILayout.PropertyField(m_onFinish);


        serializedObject.ApplyModifiedProperties();
    }
}
#endif


[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_TypeOutEffect : MonoBehaviour{

    public enum TimeSetting{ perCharacter, totalDuration};


    [SerializeField] bool   playOnEnable;
    //[SerializeField] bool   loop;
    
    [SerializeField] float          preDelay;
    [SerializeField] TimeSetting    timingMethod = TimeSetting.perCharacter;
    [SerializeField] float          charInterval;
    [SerializeField] float          duration;
    [SerializeField] float          postDelay;

    [SerializeField, ReadOnly] int characters = 0;
    [SerializeField, ReadOnly] int _index = 0;
    int index{
        set{
            _index = value;
            tmp.maxVisibleCharacters = _index;
        }
        get { return _index; }
    }

    public UnityEvent onFinish;


    TextMeshProUGUI _tmp;
    TextMeshProUGUI tmp{ get { if (_tmp == null) _tmp = GetComponent<TextMeshProUGUI>(); return _tmp; } }


    void OnEnable(){

        tmp.ForceMeshUpdate(true, true);        

        if(playOnEnable)
            PlayEffect();
    }

    void PlayEffect(){

        characters = tmp.textInfo.characterCount;

        StopAllCoroutines();
        if(timingMethod == TimeSetting.perCharacter)
            StartCoroutine(PerCharacterRoutine());
        else if(timingMethod == TimeSetting.totalDuration)
            StartCoroutine(TotalDurationRoutine());
    }

    IEnumerator PerCharacterRoutine(){
        
        tmp.maxVisibleCharacters = 0;
        
        if(preDelay > 0)
            yield return new WaitForSeconds(preDelay);


        if (charInterval > 0)
        {
            for (int i = 1; i < characters + 1; i++)
            {
                index = i;
                yield return new WaitForSeconds(charInterval);
            }
        }
        index = characters;


        if(postDelay > 0)
            yield return new WaitForSeconds(postDelay);
        
        OnFinish();
    }

    IEnumerator TotalDurationRoutine(){
        
        tmp.maxVisibleCharacters = 0;
        
        if(preDelay > 0)
            yield return new WaitForSeconds(preDelay);


        if (duration > 0)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                index = (int)Mathf.Lerp(0, characters, t);
                yield return null;
            }
        }
        index = characters;


        if(postDelay > 0)
            yield return new WaitForSeconds(postDelay);

        OnFinish();
    }

    void OnFinish(){

        if(onFinish != null)
            onFinish.Invoke();
    }
}