using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;


public class ImageLoader : MonoBehaviour {

	public static ImageLoader instance;
	public Texture2D defaultTexture;




	void Awake(){instance = this;}
	public static IEnumerator LoadTextureRoutine(string url, Action<Texture2D> callback)
	{
		Texture2D texture = new Texture2D (1,1);

		//download
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();



		if (url == string.Empty || request.isNetworkError || request.isHttpError) 
		{
			Debug.LogFormat ("Loading texture: <color=teal>{0}</color>\nError: <color=red>{1}</color>", url, request.error);
			texture = instance.defaultTexture;
		}
		else 
		{
			Debug.LogFormat ("Loading texture: <color=teal>{0}</color>\nSuccess!", url);
			texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
		}

		callback(texture);
	}
	public static IEnumerator LoadSpriteRoutine(string url, Action<Sprite> callback)
	{
		Texture2D texture = new Texture2D (1,1);

		//download
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();


		if (url == string.Empty || request.isNetworkError || request.isHttpError) 
		{
			Debug.LogFormat ("Loading sprite: <color=teal>{0}</color>\nError: <color=red>{1}</color>", url, request.error);
			texture = instance.defaultTexture;
		}
		else 
		{
			//Debug.LogFormat ("Loading sprite: <color=teal>{0}</color>\nSuccess!", url);
			texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
		}		

		Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));		
		callback(sprite);
	}
}