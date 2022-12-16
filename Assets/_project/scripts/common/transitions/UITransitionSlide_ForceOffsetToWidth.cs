using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionSlide_ForceOffsetToWidth : UITransitionSlide{

    protected override void Awake(){

        positionEnd = rectTransform.anchoredPosition3D;
	}

    protected override void OnEnable(){

		StartCoroutine(DelayOnEnable());
	}
    IEnumerator DelayOnEnable(){
		
		yield return null;
		yield return null;
		secondPosition  = new Vector3(-rectTransform.rect.width, 0, 0);		
		positionStart   = rectTransform.anchoredPosition3D + secondPosition;


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
}