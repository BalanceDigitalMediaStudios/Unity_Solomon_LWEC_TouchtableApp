using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropSticker : MonoBehaviour {

	RectTransform rectTransform;
	EventTrigger eventTrigger;
	
	// Update is called once per frame
	void Start () 
	{
		rectTransform = this.GetComponent<RectTransform>();

		eventTrigger = this.gameObject.AddComponent<EventTrigger>();
        AddTrigger.AddEventTriggerListener(eventTrigger, EventTriggerType.PointerUp, (eventData) => DestoryThis());
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			rectTransform.position = Input.mousePosition;
		}
		else
		{
			if(Input.touchCount > 0)
				rectTransform.position = Input.GetTouch(0).position;
			else
			{
				DestoryThis();
			}
		}
	}

	void DestoryThis()
	{
		Destroy(this.gameObject);
	}
}
