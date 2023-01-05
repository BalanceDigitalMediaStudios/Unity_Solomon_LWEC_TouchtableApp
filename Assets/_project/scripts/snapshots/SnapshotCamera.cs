using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Camera))]
public class SnapshotCamera : MonoBehaviour {

	public enum FileType{jpg, png, exr};

	public string 		fileNamePrefix;
	public FileType 	fileType = FileType.png;

	private Camera 	snapshotCamera;
	private Texture2D 	texture;
	private string      fileName;
	private string		filePath;
	private byte[]      bytes;



	void Awake () {snapshotCamera = GetComponent<Camera> ();}


	public static Rect RectTransformToScreenSpace(RectTransform transform){

		Vector2 	size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		Rect 	temp = new Rect((Vector2)transform.position - (size * 0.5f), size);

		//Debug.LogFormat("Converting Rect to Screen Space: {0}", temp);        	
        	return temp;
    	}
	

	public IEnumerator SnapshotRoutine(Rect rect, int outputWidth){

		float supersampling = (float)outputWidth/rect.width;
		yield return StartCoroutine(SnapshotRoutine(rect, supersampling));
	}
	public IEnumerator SnapshotRoutine(Rect rect, float supersampling = 1){

		Debug.LogFormat("Taking snapshot [Super Sampling: {0}]: {1}", supersampling, gameObject.name);
	

		//store super sampled rect
		Rect rectSS = new Rect(rect.xMin * supersampling, rect.yMin * supersampling, rect.width * supersampling, rect.height * supersampling);
		//Debug.Log(rectSS);


		//reassign texture
		Destroy(texture);
		texture = new Texture2D ((int)(rect.width * supersampling), (int)(rect.height * supersampling), TextureFormat.RGB24, 2, true);
		

		//wait for update, and assign new rendertexture
		yield return new WaitForEndOfFrame ();
		RenderTexture currentRT 	= snapshotCamera.targetTexture;
		RenderTexture snapshotRT = new RenderTexture((int)(Screen.width * supersampling), (int)(Screen.height * supersampling), 24, RenderTextureFormat.ARGB32);


		//render camera
		RenderTexture.	active 		= snapshotRT;
		snapshotCamera.targetTexture 	= snapshotRT;		
		snapshotCamera.Render ();
		

		//read from specified rect and apply pixels to texture		
		texture.ReadPixels (rectSS, 0, 0);
		texture.Apply ();


		//encode texture
		string extension;
		switch (fileType) 
		{
		case FileType.jpg:
			bytes = texture.EncodeToJPG ();
			extension = "jpg";
			break;
		case FileType.png:
			bytes = texture.EncodeToPNG ();
			extension = "png";
			break;
		default:
			bytes = texture.EncodeToEXR ();
			extension = "exr";
			break;
		}


		//store file name
		fileName = string.Format ("{0}_{1}.{2}", fileNamePrefix, System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"), extension);


		//reassign pre-existing rendertexture, if any
		RenderTexture.	active 		= currentRT;
		snapshotCamera.targetTexture 	= currentRT;
		yield return null;		
		Debug.LogFormat ("Snapshot complete: {0}\nWidth: {1}\nHeight: {2}", fileName, texture.width, texture.height);
	}
	public Texture2D 	GetTexture()	{return texture;}
	public Sprite 		GetSprite()	{return Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (.5f, .5f));}


	public IEnumerator SaveSnapshot(string savePath){

		filePath	= savePath + fileName;

		System.IO.File.WriteAllBytes (filePath, bytes);
		Debug.Log ("Snapshot saved: " + filePath);
		yield return null;
	}
	public string GetFileName()	{return fileName;}
	public string GetFilePath()	{return filePath;}
	public byte[] GetBytes()		{return bytes;}






	//this assumes an already attached render texture with a set resolution
	public IEnumerator SnapshotRoutine(){

		Debug.Log ("Taking snapshot: " + gameObject.name);

		yield return new WaitForEndOfFrame ();
		RenderTexture currentRT 	= RenderTexture.active;
		RenderTexture.active 	= snapshotCamera.targetTexture;


		snapshotCamera.Render ();

		
		Destroy (texture);
		texture = new Texture2D (snapshotCamera.targetTexture.width, snapshotCamera.targetTexture.height,TextureFormat.RGB24,false);
		texture.ReadPixels (new Rect(0,0,snapshotCamera.targetTexture.width,snapshotCamera.targetTexture.height),0,0);
		texture.Apply ();


		string extension;
		switch (fileType) 
		{
		case FileType.jpg:
			bytes = texture.EncodeToJPG ();
			extension = "jpg";
			break;
		case FileType.png:
			bytes = texture.EncodeToPNG ();
			extension = "png";
			break;
		default:
			bytes = texture.EncodeToEXR ();
			extension = "exr";
			break;
		}

		fileName = string.Format ("{0}_{1}.{2}", fileNamePrefix, System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"), extension);

		RenderTexture.active = currentRT;
		Debug.Log ("Snapshot complete: " + fileName);
		yield return null;
	}
}