using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableGraphicAlphaOnAwake : MonoBehaviour {

	void Awake(){

		Color tempColor = GetComponent<Graphic> ().color;
		tempColor.a = 0;
		GetComponent<Graphic> ().color = tempColor;
	}
}