using System.Collections;
using UnityEngine;

public class CursorVisibility : MonoBehaviour {

	public KeyCode hotkey 		= KeyCode.F2;
	public bool 	startEnabled 	= true;


	void Awake(){EnableCursor (startEnabled);}

	void Update(){

		if (Input.GetKeyUp (hotkey))
			ToggleCursor ();
	}

	void OnDestroy(){EnableCursor(true);}

	void ToggleCursor(){EnableCursor (!Cursor.visible);}
	void EnableCursor(bool value){Cursor.visible = value;}
}