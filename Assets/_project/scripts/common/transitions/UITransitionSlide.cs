using UnityEngine;
using System.Collections;



public class UITransitionSlide : MonoBehaviour {

	public enum PositionUse		{start,end};
	public enum OffsetUse		{absolute,relative};
	public enum OnEnableUse		{startToEnd,endToStart,positionAtStart,positionAtEnd,none};
	public enum DisableUse		{transitionsToStart,transitionsToEnd,both,none};
	public enum InterpolationUse{linear,smooth,extraSmooth};


	[SerializeField]RectTransform 	_rectTransform;
	public RectTransform rectTransform{

		get{
			if(_rectTransform == null)
				_rectTransform = transform as RectTransform;
			return _rectTransform;
		}
	}


	public bool				unscaledTime		= false;	
	public bool 			loop 				= false;	
	public float 			delayTime;
	public float 			delayTime_loop;
	public float 			transitionTime 		= 1f;
	public float			transitionTime_loop	= 0f;
	public InterpolationUse	interpolation 		= InterpolationUse.extraSmooth;
	public OnEnableUse 		actionOnEnable 		= OnEnableUse.none;
	public DisableUse		disableObjectAfter 	= DisableUse.none;
	public PositionUse 		originalPositionIs 	= PositionUse.start;
	public OffsetUse		offsetUsed 			= OffsetUse.absolute;
	public Vector3			secondPosition;


	protected Vector3 		positionStart;
	protected Vector3 		positionEnd;


	protected virtual void Awake(){

		switch (offsetUsed)
		{
		case OffsetUse.absolute:
			positionStart 	= (originalPositionIs == PositionUse.start 	? rectTransform.anchoredPosition3D 	: secondPosition);
			positionEnd 	= (originalPositionIs == PositionUse.start 	? secondPosition 					: rectTransform.anchoredPosition3D);
			break;
		case OffsetUse.relative:
			positionStart 	= (originalPositionIs == PositionUse.start 	? rectTransform.anchoredPosition3D 					: rectTransform.anchoredPosition3D + secondPosition);
			positionEnd 	= (originalPositionIs == PositionUse.start 	? rectTransform.anchoredPosition3D + secondPosition : rectTransform.anchoredPosition3D);
			break;
		}
	}


	protected virtual void OnEnable(){

		switch (actionOnEnable) 
		{
		case OnEnableUse.endToStart:
			TransitionToStart (false);
			break;
		case OnEnableUse.startToEnd:
			TransitionToEnd (false);
			break;	
		case OnEnableUse.positionAtStart:
			TransitionToStart (false, 0f, 0f);
			break;
		case OnEnableUse.positionAtEnd:
			TransitionToEnd (false, 0f, 0f);
			break;	
		}
	}


	public void TransitionToEnd(bool myStartFromCurrentPosition){
		
		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.anchoredPosition3D : positionStart), positionEnd, transitionTime, delayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.anchoredPosition3D : positionStart), positionEnd, myTransitionTime, myDelayTime));
	}


	public void TransitionToStart(bool myStartFromCurrentPosition){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.anchoredPosition3D : positionEnd), positionStart, transitionTime, delayTime));
	}
	public void TransitionToStart(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.anchoredPosition3D : positionEnd), positionStart, myTransitionTime, myDelayTime));
	}


	public void TransitionToPosition(Vector3 endPosition){

		TransitionToPosition(endPosition, transitionTime, delayTime);
	}
	public void TransitionToPosition(Vector3 endPosition, float transitionTime, float delayTime){

		StopAllCoroutines ();
		StartCoroutine (TransitionRoutine(rectTransform.anchoredPosition3D,endPosition,transitionTime,delayTime));
	}


	public IEnumerator TransitionRoutine(Vector3 myPositionStart, Vector3 myPositionEnd, float myTransitionTime, float myDelayTime){

		rectTransform.anchoredPosition3D = myPositionStart;

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
				}

				rectTransform.anchoredPosition3D = Vector3.Lerp (myPositionStart,myPositionEnd,interp);
				yield return null;
			}
		}

		//make sure end position is reached
		rectTransform.anchoredPosition3D = myPositionEnd;

		

		if (disableObjectAfter == DisableUse.both ||
			(disableObjectAfter == DisableUse.transitionsToEnd 		&& myPositionEnd == positionEnd) ||
			(disableObjectAfter == DisableUse.transitionsToStart 	&& myPositionEnd == positionStart)) 
		{
			gameObject.SetActive (false);
		}		

		if(loop && isActiveAndEnabled)
			StartCoroutine(TransitionRoutine(myPositionEnd, myPositionStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, delayTime_loop));
	}
}