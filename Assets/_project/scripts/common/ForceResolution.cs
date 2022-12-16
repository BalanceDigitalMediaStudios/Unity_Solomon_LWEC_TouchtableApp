using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceResolution : MonoBehaviour {

	public int 				width, height;
	public FullScreenMode 	fullScreenMode;

	void Awake(){

		if(Screen.width != width || Screen.height != height || Screen.fullScreenMode != fullScreenMode)
			Screen.SetResolution (width, height, fullScreenMode);			
	}
}