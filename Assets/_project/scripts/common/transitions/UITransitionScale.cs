using UnityEngine;
using System.Collections;



public class UITransitionScale : MonoBehaviour {

	public enum PositionUse		{start,end};
	public enum OnEnableUse		{startToEnd,endToStart,scaleAtStart,scaleAtEnd,none};
	public enum DisableUse		{transitionsToStart,transitionsToEnd,both,none};
	public enum InterpolationUse{linear,smooth,extraSmooth, rubberBand01};


	public RectTransform 	rectTransform;
	public bool				unscaledTime		= false;
	public bool 			loop 				= false;
	public float 			delayTime;
	public float			delayTime_loop;
	public float 			transitionTime 		= 1f;
	public float			transitionTime_loop	= 0f;
	public InterpolationUse	interpolation 		= InterpolationUse.extraSmooth;
	public OnEnableUse 		actionOnEnable 		= OnEnableUse.none;
	public DisableUse		disableObjectAfter 	= DisableUse.none;
	public DisableUse		destroyObjectAfter 	= DisableUse.none;
	public DisableUse		stopLoopAfter 		= DisableUse.none;
	public PositionUse 		originalScaleIs 	= PositionUse.start;
	public Vector3			secondScale;


	private Vector3 		scaleStart;
	private Vector3 		scaleEnd;


	void Awake(){

		if (rectTransform == null)
			rectTransform = transform as RectTransform;
		

		scaleStart 	= (originalScaleIs == PositionUse.start 	? rectTransform.localScale 	: secondScale);
		scaleEnd 	= (originalScaleIs == PositionUse.start 	? secondScale 				: rectTransform.localScale);
	}


	void OnEnable(){

		switch (actionOnEnable) 
		{
		case OnEnableUse.endToStart:
			TransitionToStart (false);
			break;
		case OnEnableUse.startToEnd:
			TransitionToEnd (false);
			break;	
		case OnEnableUse.scaleAtStart:
			TransitionToStart (false, 0f, 0f);
			break;
		case OnEnableUse.scaleAtEnd:
			TransitionToEnd (false, 0f, 0f);
			break;	
		}
	}

	public void Transition(Vector3 endScale){

		StopAllCoroutines();
		StartCoroutine(TransitionRoutine(rectTransform.localScale, endScale, transitionTime, delayTime));
	}
	public void Transition(Vector3 endScale, float myTransitionTime, float myDelayTime){

		StopAllCoroutines();
		StartCoroutine(TransitionRoutine(rectTransform.localScale, endScale, myTransitionTime, myDelayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentScale){
		
		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : scaleStart), scaleEnd, transitionTime, delayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentScale, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : scaleStart), scaleEnd, myTransitionTime, myDelayTime));
	}


	public void TransitionToStart(bool myStartFromCurrentScale){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : scaleEnd), scaleStart, transitionTime, delayTime));
	}
	public void TransitionToStart(bool myStartFromCurrentScale, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : scaleEnd), scaleStart, myTransitionTime, myDelayTime));
	}


	public void TransitionToPosition(Vector3 endPosition){

		StopAllCoroutines ();
		StartCoroutine (TransitionRoutine(rectTransform.localScale,endPosition,transitionTime,delayTime));
	}


	public IEnumerator TransitionRoutine(Vector3 myScaleStart, Vector3 myScaleEnd, float myTransitionTime, float myDelayTime){

		rectTransform.localScale = myScaleStart;

		if(myDelayTime > 0)
		{
			if(unscaledTime)
				yield return new WaitForSecondsRealtime (myDelayTime);
			else
				yield return new WaitForSeconds (myDelayTime);
		}

		
		if(myTransitionTime > 0)
		{
			float t = 0;
			while (t < 1) 
			{
				t += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / myTransitionTime;
				float interp = t;

				switch (interpolation) 
				{
				case InterpolationUse.smooth:
					interp = Mathf.SmoothStep (0, 1, t);
					break;
				case InterpolationUse.extraSmooth:
					interp = Mathf.SmoothStep (0, 1, Mathf.SmoothStep(0,1,t));
					break;
				case InterpolationUse.rubberBand01:
					interp = (Mathf.Sin (-3f * Mathf.PI * t) / (3f * Mathf.PI * t)) * (1f - t) + 1f;
					break;
				}

				Vector3 newScale = Vector3.LerpUnclamped (myScaleStart,myScaleEnd,interp);
				if(float.IsNaN(newScale.x) || float.IsNaN(newScale.y) || float.IsNaN(newScale.z))
					yield return new WaitForEndOfFrame();

				rectTransform.localScale = newScale;
				//rectTransform.localScale = Vector3.LerpUnclamped (myScaleStart,myScaleEnd,interp);
				yield return new WaitForEndOfFrame();
			}
		}

		//make sure end scale is reached
		rectTransform.localScale = myScaleEnd;


		if (disableObjectAfter == DisableUse.both ||
			(disableObjectAfter == DisableUse.transitionsToEnd 	&& myScaleEnd == scaleEnd) ||
			(disableObjectAfter == DisableUse.transitionsToStart 	&& myScaleEnd == scaleStart)) 
		{
			gameObject.SetActive (false);
		}	


		if (destroyObjectAfter == DisableUse.both ||
			(destroyObjectAfter == DisableUse.transitionsToEnd 	&& myScaleEnd == scaleEnd) ||
			(destroyObjectAfter == DisableUse.transitionsToStart 	&& myScaleEnd == scaleStart)) 
		{
			Destroy (gameObject);
		}	


		if (loop && gameObject.activeInHierarchy) 
		{
			if (stopLoopAfter == DisableUse.both ||
				(stopLoopAfter == DisableUse.transitionsToEnd 	&& myScaleEnd == scaleEnd) ||
				(stopLoopAfter == DisableUse.transitionsToStart 	&& myScaleEnd == scaleStart)) 
			{
				yield break;
			}		

			StartCoroutine (TransitionRoutine (myScaleEnd, myScaleStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, delayTime_loop));
		}
	}
}