using UnityEngine;
using System.Collections;



public class UITransitionSize : MonoBehaviour {

	public enum PositionUse		{start,end};
	public enum OffsetUse		{absolute,relative};
	public enum OnEnableUse		{startToEnd,endToStart,positionAtStart,positionAtEnd,none};
	public enum DisableUse		{transitionsToStart,transitionsToEnd,both,none};
	public enum InterpolationUse{linear,smooth,extraSmooth, rubberBand01};


	public RectTransform 	rectTransform;
	public bool 			loop 				= false;
	public float 			delayTime;
	public float 			transitionTime 		= 1f;
	public float			transitionTime_loop	= 0f;
	public InterpolationUse	interpolation 		= InterpolationUse.extraSmooth;
	public OnEnableUse 		actionOnEnable 		= OnEnableUse.none;
	public DisableUse		disableObjectAfter 	= DisableUse.none;
	public PositionUse 		originalPositionIs 	= PositionUse.start;
	public OffsetUse		offsetUsed 			= OffsetUse.absolute;
	public Vector2			secondPosition;


	private Vector2 		positionStart;
	private Vector2 		positionEnd;


	void Awake(){

		if (rectTransform == null)
			rectTransform = transform as RectTransform;

        Canvas.ForceUpdateCanvases();

        Vector2 currentSize = rectTransform.rect.size;

        switch (offsetUsed)
		{
		case OffsetUse.absolute:
			positionStart 	= (originalPositionIs == PositionUse.start 	? currentSize 					: secondPosition);
			positionEnd 	= (originalPositionIs == PositionUse.start 	? secondPosition 				: currentSize);
			break;
		case OffsetUse.relative:
			positionStart 	= (originalPositionIs == PositionUse.start 	? currentSize 					: currentSize + secondPosition);
			positionEnd 	= (originalPositionIs == PositionUse.start 	? currentSize + secondPosition 	: currentSize);
			break;
		}

        //Debug.LogFormat("Start: {0}\nEnd:   {1}", positionStart, positionEnd);
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
		case OnEnableUse.positionAtStart:
			rectTransform.sizeDelta = positionStart;
			break;
		case OnEnableUse.positionAtEnd:
			rectTransform.sizeDelta = positionEnd;
			break;	
		}
	}


	public void TransitionToEnd(bool myStartFromCurrentPosition){
		
		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionStart), positionEnd, transitionTime, delayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionStart), positionEnd, myTransitionTime, myDelayTime));
	}


	public void TransitionToStart(bool myStartFromCurrentPosition){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionEnd), positionStart, transitionTime, delayTime));
	}
	public void TransitionToStart(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionEnd), positionStart, myTransitionTime, myDelayTime));
	}


	public void TransitionToPosition(Vector2 endPosition){

		TransitionToPosition(endPosition,transitionTime,delayTime);
	}
	public void TransitionToPosition(Vector2 endPosition, float transitionTime, float delayTime){

		StopAllCoroutines ();
		StartCoroutine (TransitionRoutine(rectTransform.rect.size,endPosition,transitionTime,delayTime));
	}


	public IEnumerator TransitionRoutine(Vector2 myPositionStart, Vector2 myPositionEnd, float myTransitionTime, float myDelayTime){

		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 	myPositionStart.x);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 	myPositionStart.y);
		//rectTransform.sizeDelta = myPositionStart;

		yield return new WaitForSeconds (myDelayTime);

		float t = 0;
		while (t < 1) 
		{
			// # Sam 180724 - Added check to avoid division by 0
			t += (myTransitionTime == 0 ? 1f : (Time.deltaTime / myTransitionTime)/Time.timeScale);
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

			
			Vector2 newSize = Vector2.LerpUnclamped (myPositionStart,myPositionEnd,interp);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 	newSize.x);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 	newSize.y);
			//rectTransform.sizeDelta = Vector2.LerpUnclamped (myPositionStart,myPositionEnd,interp);

			

			yield return null;
		}

		//make sure final value is assigned, since we are lerping unclamped
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 	myPositionEnd.x);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 	myPositionEnd.y);
		//rectTransform.sizeDelta = myPositionEnd;
		

		if (disableObjectAfter == DisableUse.both ||
			(disableObjectAfter == DisableUse.transitionsToEnd 		&& myPositionEnd == positionEnd) ||
			(disableObjectAfter == DisableUse.transitionsToStart 	&& myPositionEnd == positionStart)) 
		{
			gameObject.SetActive (false);
		}		

		if(loop)
			StartCoroutine(TransitionRoutine(myPositionEnd, myPositionStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, 0f));
	}
}