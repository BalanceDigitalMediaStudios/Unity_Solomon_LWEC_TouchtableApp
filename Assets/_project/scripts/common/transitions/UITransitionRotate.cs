using UnityEngine;
using System.Collections;



public class UITransitionRotate : MonoBehaviour {

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
	public PositionUse 		originalRotateIs 	= PositionUse.start;
	public Vector3			secondRotation;


	private Vector3 		rotateStart;
	private Vector3 		rotateEnd;


	void Awake(){

		if (rectTransform == null)
			rectTransform = transform as RectTransform;
		

		rotateStart 	= (originalRotateIs == PositionUse.start 	? rectTransform.localScale 	: secondRotation);
		rotateEnd 		= (originalRotateIs == PositionUse.start 	? secondRotation 				: rectTransform.localScale);
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


	public void TransitionToEnd(bool myStartFromCurrentScale){
		
		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : rotateStart), rotateEnd, transitionTime, delayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentScale, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : rotateStart), rotateEnd, myTransitionTime, myDelayTime));
	}


	public void TransitionToStart(bool myStartFromCurrentScale){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : rotateEnd), rotateStart, transitionTime, delayTime));
	}
	public void TransitionToStart(bool myStartFromCurrentScale, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentScale ? rectTransform.localScale : rotateEnd), rotateStart, myTransitionTime, myDelayTime));
	}


	public void TransitionToPosition(Vector3 endPosition){

		StopAllCoroutines ();
		StartCoroutine (TransitionRoutine(rectTransform.localScale,endPosition,transitionTime,delayTime));
	}

	public IEnumerator TransitionRoutine(Vector3 myRotateStart, Vector3 myRotateEnd, float myTransitionTime, float myDelayTime){

		rectTransform.localRotation = RotateOrderXYZ(myRotateStart);


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

				Vector3 newEuler = Vector3.Lerp(myRotateStart, myRotateEnd, interp);

				rectTransform.localRotation = RotateOrderXYZ(newEuler);
				yield return null;
			}
		}

		//make sure end rotation is reached
		rectTransform.localRotation = RotateOrderXYZ(myRotateEnd);


		if (disableObjectAfter == DisableUse.both ||
			(disableObjectAfter == DisableUse.transitionsToEnd 	&& myRotateEnd == rotateEnd) ||
			(disableObjectAfter == DisableUse.transitionsToStart 	&& myRotateEnd == rotateStart)) 
		{
			gameObject.SetActive (false);
		}	


		if (destroyObjectAfter == DisableUse.both ||
			(destroyObjectAfter == DisableUse.transitionsToEnd 	&& myRotateEnd == rotateEnd) ||
			(destroyObjectAfter == DisableUse.transitionsToStart 	&& myRotateEnd == rotateStart)) 
		{
			Destroy (gameObject);
		}	


		if (loop && gameObject.activeInHierarchy) 
		{
			if (stopLoopAfter == DisableUse.both ||
				(stopLoopAfter == DisableUse.transitionsToEnd 	&& myRotateEnd == rotateEnd) ||
				(stopLoopAfter == DisableUse.transitionsToStart 	&& myRotateEnd == rotateStart)) 
			{
				yield break;
			}		

			StartCoroutine (TransitionRoutine (myRotateEnd, myRotateStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, delayTime_loop));
		}
	}



	public enum Axis{
		x, y, z
	}

	Quaternion RotateOrderXYZ(Vector3 euler){

		return RotateByOrder(euler.x, Axis.x, euler.y, Axis.y, euler.z, Axis.z);
	}

	Quaternion RotateByOrder(float angle01, Axis axis01, float angle02, Axis axis02, float angle03, Axis axis03){

		Quaternion rotation = Quaternion.AngleAxis(angle01, GetAxis(axis01)) *
								Quaternion.AngleAxis(angle02, GetAxis(axis02)) *
								Quaternion.AngleAxis(angle03, GetAxis(axis03));

		return rotation;
	}
	Vector3 GetAxis(Axis axis){

		switch(axis)
		{
		case Axis.x:
			return Vector3.right;
		
		case Axis.y:
			return Vector3.up;
		
		case Axis.z:
		default:
			return Vector3.forward;
		}
	}


	/* public IEnumerator TransitionRoutine(Vector3 myRotateStart, Vector3 myRotateEnd, float myTransitionTime, float myDelayTime){

		Quaternion quatStart 	= Quaternion.Euler(myRotateStart);
		Quaternion quatEnd		= Quaternion.Euler(myRotateEnd);

		rectTransform.localRotation = quatStart;


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

				rectTransform.localRotation = Quaternion.Lerp (quatStart,quatEnd,interp);
				yield return null;
			}
		}

		//make sure end rotation is reached
		rectTransform.localRotation = quatEnd;


		if (disableObjectAfter == DisableUse.both ||
			(disableObjectAfter == DisableUse.transitionsToEnd 	&& myRotateEnd == rotateEnd) ||
			(disableObjectAfter == DisableUse.transitionsToStart 	&& myRotateEnd == rotateStart)) 
		{
			gameObject.SetActive (false);
		}	


		if (destroyObjectAfter == DisableUse.both ||
			(destroyObjectAfter == DisableUse.transitionsToEnd 	&& myRotateEnd == rotateEnd) ||
			(destroyObjectAfter == DisableUse.transitionsToStart 	&& myRotateEnd == rotateStart)) 
		{
			Destroy (gameObject);
		}	


		if (loop && gameObject.activeInHierarchy) 
		{
			if (stopLoopAfter == DisableUse.both ||
				(stopLoopAfter == DisableUse.transitionsToEnd 	&& myRotateEnd == rotateEnd) ||
				(stopLoopAfter == DisableUse.transitionsToStart 	&& myRotateEnd == rotateStart)) 
			{
				yield break;
			}		

			StartCoroutine (TransitionRoutine (myRotateEnd, myRotateStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, delayTime_loop));
		}
	} */
}