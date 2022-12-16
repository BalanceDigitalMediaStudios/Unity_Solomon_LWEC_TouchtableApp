using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeed : MonoBehaviour {

	public delegate 	void 	PauseEvent ();
	public static 		event 	PauseEvent onPause;
	public static 		event 	PauseEvent onResume;
	public delegate 	void 	GameSpeedEvent (float speed);
	public static 		event 	GameSpeedEvent onUpdateSpeed;




	public static GameSpeed instance;

	[Header("Hotkeys")]
	public KeyCode fasterHotkey 	= KeyCode.Period;
	public KeyCode slowerHotkey	= KeyCode.Comma;
	public KeyCode pauseHotkey 	= KeyCode.Space;

	[Header("Settings")]
	public float 	fasterRate = 20f;
	public float 	slowerRate = .25f;
	
	[Header("Debug")]
	[SerializeField][ReadOnly]bool 	isPaused;
	[SerializeField][ReadOnly]float 	multiplier;
	[SerializeField][ReadOnly]float 	playSpeed;
	
	float actualPlaySpeed;

	
	
	void Awake(){

		instance = this;

		SetPlaySpeed(1);
		SetMultiplier(1);
		Pause(false);
	}

	void LateUpdate(){

		//check for pausing
		if (Input.GetKeyDown(pauseHotkey))
			TogglePause();


		//check for multipliers
		if 		(Time.timeScale != fasterRate && Input.GetKeyDown(fasterHotkey))
			SetMultiplier(fasterRate);
		else if	(Time.timeScale != slowerRate && Input.GetKeyDown(slowerHotkey))
			SetMultiplier(slowerRate);
		else if	(Input.GetKeyUp(fasterHotkey) || Input.GetKeyUp(slowerHotkey))
			SetMultiplier(1);
	}
	
	public void SetPlaySpeed(float value){

		playSpeed = Mathf.Clamp(value, 0, Mathf.Infinity);

		if(onUpdateSpeed != null)
			onUpdateSpeed(playSpeed);
	}



	public void TogglePause(){

		isPaused = !isPaused;
		Pause(isPaused);
	}
	public void Pause(bool value){

		isPaused 			= value;
		actualPlaySpeed 	= value ? 0 : playSpeed;
		UpdateTimeScale();


		//send pause events
		if(isPaused 	&& onPause != null)
			onPause();
		if(!isPaused 	&& onResume != null)
			onResume();
	}

	
	void SetMultiplier(float value){
		
		multiplier = Mathf.Clamp(value, 0, Mathf.Infinity);
		UpdateTimeScale();
	}


	void UpdateTimeScale(){

		Time.timeScale = actualPlaySpeed * multiplier;
	}
}