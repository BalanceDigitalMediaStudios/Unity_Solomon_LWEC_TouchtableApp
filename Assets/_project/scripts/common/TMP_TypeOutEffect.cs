using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_TypeOutEffect : MonoBehaviour{

    [SerializeField] bool   playOnEnable;
    [SerializeField] bool   loop;
    [SerializeField] float  preDelay;
    [SerializeField] float  charInterval;
    [SerializeField] float  postDelay;

    [Header("Debug")]
    [SerializeField, ReadOnly] int characters = 0;
    [SerializeField, ReadOnly] int _index = 0;
    int index{
        set{
            _index = value;
            tmp.maxVisibleCharacters = _index;
        }
        get { return _index; }
    }
    

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
        StartCoroutine(PlayEffectRoutine());
    }

    IEnumerator PlayEffectRoutine(){
        
        tmp.maxVisibleCharacters = 0;
        if(preDelay > 0)
            yield return new WaitForSeconds(postDelay);
        
        for (int i = 1; i < characters + 1; i++)
        {
            index = i;
            yield return new WaitForSeconds(charInterval);
        }


        if(postDelay > 0)
            yield return new WaitForSeconds(postDelay);
        

        if(loop)
            PlayEffect();
    }
}