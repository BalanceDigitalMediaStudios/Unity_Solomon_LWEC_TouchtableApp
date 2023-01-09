using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class PlayAnimation : MonoBehaviour{

    [SerializeField] bool   playOnEnable;
    [SerializeField] float  delay;
    [SerializeField] string state;

    Animator _animator;
    Animator animator{get { if (_animator == null) _animator = GetComponent<Animator>(); return _animator; } }

    void OnEnable(){

        if(playOnEnable)
            Play();
    }

    public void Play(){ Play(delay); }
    public void Play(float delay){

        StopAllCoroutines();
        StartCoroutine(PlayRoutine(delay));
    }

    IEnumerator PlayRoutine(float delay){

        if(delay > 0)
            yield return new WaitForSeconds(delay);

        animator.Play(state, -1, 0f);
    }
}