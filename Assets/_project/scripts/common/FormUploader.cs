using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;


public class FormUploader : SimpleSingleton<FormUploader>{


    public void Upload(WWWForm form, string url, float minimumTime, System.Action<string> onSuccess, System.Action<string> onFail){

        StartCoroutine(UploadRoutine(form, url, minimumTime, onSuccess, onFail));
    }
    IEnumerator UploadRoutine(WWWForm form, string url, float minimumTime, System.Action<string> onSuccess, System.Action<string> onFail){

        Debug.LogFormat("Uploading form data to: {0}", Colorize(url, "yellow"));

        using(UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            float startTime = Time.time;

            yield return request.SendWebRequest();
            while(Time.time - startTime < minimumTime)
                yield return null;


            if(request.result == UnityWebRequest.Result.Success)
            {
                Debug.LogFormat("Upload succeeded:\n{0}", Colorize(request.downloadHandler.text, "yellow"));
                onSuccess.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogFormat("Upload failed:\n{0}", Colorize(request.downloadHandler.text, "yellow"));
                Debug.LogFormat("Error Message:\n{0}", Colorize(request.downloadHandler.error, "red"));
                onFail.Invoke(request.downloadHandler.error);
            }
        }
    }

    public static string Colorize(string input, string color){ return $"<color={color}>{input}</color>"; }
}