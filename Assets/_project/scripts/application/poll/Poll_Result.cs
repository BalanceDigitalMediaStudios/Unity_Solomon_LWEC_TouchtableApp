using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Poll_Result : MonoBehaviour{

    [SerializeField] UITransitionSlide _slide;
    public UITransitionSlide slide{get { return _slide; } }

    [SerializeField] TextMeshProUGUI    answerText;
    [SerializeField] TextMeshProUGUI    percentText;
    [SerializeField] Transform          fillRect;
    [SerializeField] float              defaultDuration = 1;

    float percent       = 0;
    float _percentCur   = 0;
    float percentCur{

        get { return _percentCur; }
        set {

            _percentCur         = value;
            percentText.text    = string.Format("{0:0%}", percentCur);
            fillRect.localScale = new Vector3(percentCur, 1, 1);
        }
    }

    void OnEnable(){

        percentCur = 0;
    }

    public void Initialize(string answer, float percent){

        answerText.text = answer;
        this.percent    = percent;
    }

    public void AnimateResult(){ AnimateResult(defaultDuration); }
    public void AnimateResult(float duration){

        StopAllCoroutines();
        StartCoroutine(AnimateResultRoutine(duration));
    }
    IEnumerator AnimateResultRoutine(float duration){

        //transition
        if (duration > 0)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                float lerp = Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, t));// Mathf.Pow(t - 1f, 3f) + 1f;

                percentCur = Mathf.Lerp(0, percent, lerp);
                yield return null;
            }
        }

        //final values
        percentCur = percent;
    }
}