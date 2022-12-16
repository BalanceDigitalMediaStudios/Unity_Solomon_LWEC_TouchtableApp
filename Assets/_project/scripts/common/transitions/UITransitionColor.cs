using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UITransitionColor : MonoBehaviour {


	public Image image;

	public enum OnEnableAction		{startToEnd,endToStart,jumpToStart,jumpToEnd,none};
	public enum DisableCondition		{transitioningToStart,transitioningToEnd,both,none};
	public enum BlockRaycastCondition	{duringTransition,afterTransition,always,never};

	public bool					loop					= false;
	public bool 					loopDelay 			= false;
	public float 					alphaStart  			= 0f;
	public float 					alphaEnd    			= 1f;
	public float 					delayTime 			= 0f;
	public float 					delayTime_loop 		= 0f;
	public float 					transitionTime			= 1f;
	public float					transitionTime_loop		= 0f;
	public OnEnableAction  			onEnableAction 			= OnEnableAction.startToEnd;
	public DisableCondition 			disableCondition		= DisableCondition.transitioningToStart;
	public BlockRaycastCondition 		blockRaycastCondition 	= BlockRaycastCondition.afterTransition;


	/* void Awake(){

		if(image == null)
			image = GetComponent<Image> ();
	}


	void OnEnable(){

		switch (onEnableAction) 
		{
		case OnEnableAction.startToEnd:
			TransitionToEnd (false);
			break;
		case OnEnableAction.endToStart:
			TransitionToStart (false);
			break;	
		case OnEnableAction.jumpToEnd:
			TransitionToEnd (false, 0f,0f);
			break;
		case OnEnableAction.jumpToStart:
			TransitionToStart (false, 0f,0f);		
			break;	
		}
	}



	//---------------PUBLIC METHODS--------------------------------


	public void TransitionToEnd(bool myStartFromCurrentPosition){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? canvasGroup.alpha : alphaStart), alphaEnd, transitionTime, delayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? canvasGroup.alpha : alphaStart), alphaEnd, myTransitionTime, myDelayTime));
	}


	public void TransitionToStart(bool myStartFromCurrentPosition){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? canvasGroup.alpha : alphaEnd), alphaStart, transitionTime, delayTime));
	}
	public void TransitionToStart(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? canvasGroup.alpha : alphaEnd), alphaStart, myTransitionTime, myDelayTime));
	}


	public void TransitionToValue(float myAlphaEnd){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine(canvasGroup.alpha, myAlphaEnd, transitionTime, delayTime));
	}
	public void TransitionToValue(float myAlphaEnd, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine(canvasGroup.alpha, myAlphaEnd, myTransitionTime, myDelayTime));
	}



	//---------------COROUTINE--------------------------------

	public IEnumerator TransitionRoutine(float myAlphaStart, float myAlphaEnd, float myTransitionTime, float myDelayTime){

		canvasGroup.blocksRaycasts 	= (blockRaycastCondition == BlockRaycastCondition.always || blockRaycastCondition == BlockRaycastCondition.duringTransition);
		canvasGroup.alpha 			= myAlphaStart;


		yield return new WaitForSeconds (myDelayTime);


		float t = 0;
		while (t < 1) 
		{
			t += Time.deltaTime / myTransitionTime;
			canvasGroup.alpha = Mathf.Lerp (myAlphaStart,myAlphaEnd,Mathf.SmoothStep(0,1,Mathf.SmoothStep(0,1,t)));
			yield return null;
		}

		if (disableCondition == DisableCondition.both ||
			(disableCondition == DisableCondition.transitioningToEnd 	&& canvasGroup.alpha == alphaEnd) ||
			(disableCondition == DisableCondition.transitioningToStart 	&& canvasGroup.alpha == alphaStart)) 
		{
			gameObject.SetActive (false);
			yield break;
		}	

		canvasGroup.blocksRaycasts 	= (blockRaycastCondition == BlockRaycastCondition.always || blockRaycastCondition == BlockRaycastCondition.afterTransition);

		if(loop)
			StartCoroutine(TransitionRoutine(myAlphaEnd, myAlphaStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, loopDelay ? (delayTime_loop != 0 ? delayTime_loop : delayTime) : 0f));
	} */
}