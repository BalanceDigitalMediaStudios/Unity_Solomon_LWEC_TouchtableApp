using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class ReloadScene : SimpleSingleton<ReloadScene> {

	[SerializeField]KeyCode 			hotkey 				= KeyCode.F3;
	[SerializeField]float 				debugReloadDelay 	= 2f;
	[SerializeField]UITransitionFade 	overlayFade;  		//fades in to hide loading
	[SerializeField][ReadOnly]bool 		isReloading 		= false;


	void Update() {
	
		if(Input.GetKeyUp(hotkey))
			Reload(debugReloadDelay);
	}

	public void Reload(float delay = 0){

		if(isReloading)
			return;

		
		isReloading = true;
		Debug.Log("Reloading Scene");

		
		StopAllCoroutines();
		StartCoroutine(ReloadRoutine(delay));
	}

	IEnumerator ReloadRoutine(float delay){

		//stop kinect to give it time to shut down before a scene reload
		//KinectManager.Instance.StopKinect();


		//fade in overlay
		if(overlayFade != null)
		{
			overlayFade.unscaledTime = true;
			overlayFade.gameObject.SetActive(true);
			overlayFade.TransitionToEnd(false, .5f, 0f);
			yield return new WaitForSecondsRealtime(1);
		}

		if(delay > 0)
			yield return new WaitForSecondsRealtime(delay);


		TimeManager.instance.paused = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}