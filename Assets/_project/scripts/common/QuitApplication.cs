using UnityEngine;
using System.Collections;

public class QuitApplication : MonoBehaviour {

	public KeyCode hotkey = KeyCode.F4;

    //public GameObject QuitConfirmMenu;
    //public bool isQuitMenuOpen;
    //public bool ConfirmQuit = false;

	void Update () {
	
		if (Input.GetKeyUp (hotkey)) 
		{
			Debug.Log ("Quitting application");
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit ();
			#endif
		}
	}

    //public float showSplashTimeout = 2.0F;
    //private bool allowQuitting = false;
    //void OnApplicationQuit()
    //{

    //    if (!allowQuitting)
    //    {
    //        Application.CancelQuit();
    //        StartCoroutine("DelayedQuit");
    //    }

    //}
    //IEnumerator DelayedQuit()
    //{
    //    QuitConfirmMenu.SetActive(true);
    //    yield return new WaitForSeconds(showSplashTimeout);
    //    allowQuitting = true;
    //    Application.Quit();
    //}
}