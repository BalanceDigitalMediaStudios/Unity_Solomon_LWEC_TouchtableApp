using UnityEngine;
using System.Collections;

public class InitialConditions : MonoBehaviour {

    public bool ActivateOnEnable;
	public GameObject[] enabledObjects;
	public GameObject[] disabledObjects;

	void Awake(){

        if(!this.enabled)
            return;

        foreach (GameObject go in disabledObjects)
			go.SetActive (false);
		foreach (GameObject go in enabledObjects)
			go.SetActive (true);
	}

    void OnEnable(){

        if(!this.enabled)
            return;

        if(ActivateOnEnable)
        {
            foreach (GameObject go in disabledObjects)
                go.SetActive(false);
            foreach (GameObject go in enabledObjects)
                go.SetActive(true);
        }
    }
}