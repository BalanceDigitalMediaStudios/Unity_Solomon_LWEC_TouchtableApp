using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagerDebugKeys : MonoBehaviour {    

    [Tooltip("Hold one of these down to change the fast and slow hotkeys from holds to toggles")]
    [SerializeField]KeyCode[]   toggleModifiers = new KeyCode[] {KeyCode.LeftControl, KeyCode.RightControl};    
    [Space(15)]


    [SerializeField]                    KeyCode fastkey     = KeyCode.Period;
    [Range(1f, 100f)][SerializeField]   float 	fastRate    = 20f;	
    [Space(15)]


	[SerializeField]                    KeyCode slowkey	    = KeyCode.Comma;
    [Range(.01f, 1f)][SerializeField]   float   slowRate    = .2f;
    [Space(15)]
    

	[SerializeField]KeyCode     pausekey 	= KeyCode.Space;
    [SerializeField]KeyCode     stepkey     = KeyCode.Tab;



    //tracks repeated frame steps as long as hotkey is down
    const   float       minStepHoldTime    = .5f; 
            float       stepHoldTime       = 0;

    TimeManager timeManager;
    



    private void Awake(){ timeManager = TimeManager.instance; }


    private void LateUpdate(){

        //pause hotkey is ALWAYS a toggle
        if(Input.GetKeyDown(pausekey))
            timeManager.paused = !timeManager.paused;

        
        //step hold
        if(Input.GetKey(stepkey) && timeManager.paused)
        {
            stepHoldTime += Time.unscaledDeltaTime;

            if(stepHoldTime >= minStepHoldTime)
                timeManager.Step();
        }

        //step single
        if(Input.GetKeyDown(stepkey))
        {
            if(stepHoldTime < minStepHoldTime)
                timeManager.Step();

            stepHoldTime = 0; 
        }


        //if using as toggle
        if(ToggleModifier())
        {
            //fast
            if(Input.GetKeyDown(fastkey))
                timeManager.multiplier = timeManager.multiplier != fastRate ? fastRate : 1;

            //slow
            if(Input.GetKeyDown(slowkey))
                timeManager.multiplier = timeManager.multiplier != slowRate ? slowRate : 1;            
        }
        //if using as hold
        else
        {
            //fast
            if(Input.GetKeyDown(fastkey))
                timeManager.multiplier = fastRate;
            
            //slow
            if(Input.GetKeyDown(slowkey))
                timeManager.multiplier = slowRate;

            //release
            if(Input.GetKeyUp(fastkey) || Input.GetKeyUp(slowkey))
                timeManager.multiplier = 1;          
        }		
    }

    private bool ToggleModifier(){

        foreach(KeyCode k in toggleModifiers)
        {
            if(Input.GetKey(k))
                return true;
        }
        return false;
    }
}