using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class StickerTrash : MonoBehaviour {

    bool isPointerOver;

	// Use this for initialization
	void Start () {
        //Set Event Trigger Delegates
        EventTrigger eventTrigger = this.GetComponent<EventTrigger>();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { SetPointOver(true); });

        eventTrigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { SetPointOver(false); });

        eventTrigger.triggers.Add(exitEntry);
	}
	

    void SetPointOver(bool value)
    {
        isPointerOver = value;
    }

    public void CheckDeleteSticker(StickerElement sticker)
    {
        if(isPointerOver)
        {
            sticker.DeleteSticker();
            isPointerOver = false;
        }
    }

}
