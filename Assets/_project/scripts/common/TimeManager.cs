using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeManager : SimpleSingleton<TimeManager> {

    public delegate     void    TimeScaleEvent (float timeScale);
    public static       event   TimeScaleEvent onTimeScaleChanged;
    public static       event   TimeScaleEvent onOutputTimeScaleChanged;

    public delegate 	void 	PauseEvent ();
	public static 		event 	PauseEvent onPause;
	public static 		event 	PauseEvent onResume;
    public static       event   PauseEvent onStep;



    [Tooltip("Toggles outputTimeScale between 0 and (timeScale * multiplier)")]
	[SerializeField][ReadOnly]private bool _paused = false;
    public bool paused{

        get { return _paused; }
        set
        {
            if(_paused != value)
            {
                _paused = value;                

                if(_paused && onPause != null)
                    onPause();
                else if(!_paused && onResume != null)
                    onResume();

                UpdateOutputTimeScale();
            }
        }        
    }
    
    [Tooltip("Only modify this for debugging during testing.  Use timeScale directly for your game")]
	[SerializeField][ReadOnly]private float _multiplier = 1;
    public float multiplier{

        get { return _multiplier; }
        set
        {
            if(_multiplier != value)
            {
                _multiplier = value;

                UpdateOutputTimeScale();
            }
        }        
    }

    [Tooltip("Base level time scale before multipliers and pausing")]
	[SerializeField][ReadOnly]private float _timeScale = 1;
    public float timeScale{

        get { return _timeScale; }
        set
        {
            if(_timeScale != value)
            {
                _timeScale = value;

                if(onTimeScaleChanged != null)
                    onTimeScaleChanged(_timeScale);

                UpdateOutputTimeScale();
            }
        }
        
    }

    [Tooltip("Resulting time scale after multiplier and pausing are accounted for")]
    [SerializeField][ReadOnly]private float _outputTimeScale = 1;
    public float outputTimeScale{

        get{ return _outputTimeScale; }
        /* set
        {
            if(_outputTimeScale != value)
            {
                _outputTimeScale = value;

                if(onOutputTimeScaleChanged != null)
                    onOutputTimeScaleChanged(_outputTimeScale);
            }
        } */
    }



    /* protected override void PostAwake(){

        //initialize default settings
        timeScale   = 1;
        multiplier  = 1;


        _paused = false;

        if(_paused && onPause != null)
            onPause();
        else if(!_paused && onResume != null)
            onResume();
	} */

    void Awake(){

        UpdateOutputTimeScale();
    }



    //sets actual time scale factoring pausing and multiplier
    private void UpdateOutputTimeScale(){

        float result = paused ? 0 : timeScale * multiplier;

        if(_outputTimeScale != result)
        {
            _outputTimeScale = result;

            if(onOutputTimeScaleChanged != null)
                onOutputTimeScaleChanged(_outputTimeScale);

            Time.timeScale  = _outputTimeScale;
        }
    }



    public void Step(){

        if(!paused)
            return;
        else
        {
            StopAllCoroutines();
            StartCoroutine(StepRoutine());
        }
    }
    private IEnumerator StepRoutine(){
        
        paused = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        paused = true;

        if(onStep != null)
            onStep();
    }
}