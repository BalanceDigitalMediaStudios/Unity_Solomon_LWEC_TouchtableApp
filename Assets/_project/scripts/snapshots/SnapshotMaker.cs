using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Singleton that captures an area of the screen and outputs to a texture
/// </summary>
public class SnapshotMaker : SimpleSingleton<SnapshotMaker>{

    public enum Rotation { none, clockwise90, counterClockwise90, uTurn180 };  


    /// <summary>
    /// Creates a texture within the bounds of a screen space rect.  Output resolution is dimension of rect in screen space.
    /// </summary>
    /// <param name="camera"> Camera to render snapshot </param>
    /// <param name="screenRect"> Rect in screen space relative to camera </param>
    /// <param name="onTextureCreated"> Event called that contains the resulting texture </param>
    public void TakeSnapshot(Camera camera, Rect screenRect, System.Action<Texture2D> onTextureCreated){

        TakeSnapshot(camera, screenRect, 1f, onTextureCreated);
    }

    /// <summary>
    /// Creates a texture within the bounds of a screen space rect.  Output resolution is scaled to have specified width.
    /// </summary>
    /// <param name="camera"> Camera to render snapshot </param>
    /// <param name="screenRect"> Rect in screen space relative to camera </param>
    /// <param name="width"> Upscales output texture to this width </param>
    /// <param name="onTextureCreated"> Event called that contains the resulting texture </param>   
    public void TakeSnapshot(Camera camera, Rect screenRect, int width, System.Action<Texture2D> onTextureCreated){

        TakeSnapshot(camera, screenRect, (float)width/screenRect.width, onTextureCreated);
    }

    /// <summary>
    /// Creates a texture within the bounds of a screen space rect.  Output resolution is scaled by supersampling.
    /// </summary>
    /// <param name="camera"> Camera to render snapshot </param>
    /// <param name="screenRect"> Rect in screen space relative to camera </param>
    /// <param name="supersampling"> Upscales output texture by multiplier </param>
    /// <param name="onTextureCreated"> Event called that contains the resulting texture </param> 
    public void TakeSnapshot(Camera camera, Rect screenRect, float supersampling, System.Action<Texture2D> onTextureCreated){

        StartCoroutine(SnapshotRoutine(camera, screenRect, supersampling, onTextureCreated));
    }    

    IEnumerator SnapshotRoutine(Camera camera, Rect screenRect, float supersampling, System.Action<Texture2D> onTextureCreated){
        
        System.DateTime startTime = System.DateTime.Now;
        Debug.LogFormat("Capturing snapshot of screen area - X: {0} Y: {1} Width: {2} Height: {3}\nSupersampling: {4}",
        screenRect.x, screenRect.y, screenRect.width, screenRect.height, supersampling.ToString("F3"));


        //convert rect to supersampled dimensions and create texture at that size
        Rect superSampledRect = new Rect
        (
            screenRect.x        * supersampling, 
            screenRect.y        * supersampling, 
            screenRect.width    * supersampling, 
            screenRect.height   * supersampling
        );
        Texture2D texture = new Texture2D((int)(superSampledRect.width), (int)(superSampledRect.height), TextureFormat.RGB24, 2, true);


        //wait for update, assign temporary render texture to camera and render it
        yield return new WaitForEndOfFrame ();
		RenderTexture currentRT = camera.targetTexture;
		RenderTexture tempRT    = new RenderTexture((int)(Screen.width * supersampling), (int)(Screen.height * supersampling), 24, RenderTextureFormat.ARGB32);
        RenderTexture.active    = tempRT;
		camera.targetTexture 	= tempRT;
		camera.Render ();
		

		//read from specified rect and apply pixels to texture
		texture.ReadPixels (superSampledRect, 0, 0);
		texture.Apply ();


		//reassign original render texture
		RenderTexture.active    = currentRT;
		camera.targetTexture 	= currentRT;


        //send result texture via event
        yield return new WaitForEndOfFrame();
        onTextureCreated(texture);
        System.TimeSpan duration = System.DateTime.Now.Subtract(startTime);
        Debug.LogFormat ("Snapshot captured in {0} sec, resolution: {1} x {2}", duration.TotalSeconds.ToString("F3"), texture.width, texture.height);
    }


    
    /// <summary>
    /// Helper function to convert a RectTransform into a Rect in screen space
    /// </summary>
    /// <param name="camera"> Camera to view RectTransform from </param>
    /// <param name="rectTransform"> RectTransform to convert </param>
    /// <returns> Rect in screen space </returns>
    public static Rect RectTransformToScreenSpaceRect(Camera camera, RectTransform rectTransform){

        //get corners in world space
        Vector3[] corners  = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        //create bounds using these corners to negate rotation, scaling, etc
        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for(int i = 1; i < 4; i++)
            bounds.Encapsulate(corners[i]);        

        //convert bounds to screen space corners (0 = bottom left, 1 = top left, 2 = top right, 3 = bottom right)
        Vector3[] screenCorners  = new Vector3[4];
        screenCorners[0] = RectTransformUtility.WorldToScreenPoint(camera, bounds.min);
        screenCorners[1] = RectTransformUtility.WorldToScreenPoint(camera, bounds.min + new Vector3(0, bounds.size.y, 0));
        screenCorners[2] = RectTransformUtility.WorldToScreenPoint(camera, bounds.max);
        screenCorners[3] = RectTransformUtility.WorldToScreenPoint(camera, bounds.min + new Vector3(bounds.size.x, 0, 0));

        //invert rect yPos so that top corner of screen is 0, get size by subtracting opposite corners
        Vector2 position    = screenCorners[1];
        position.y          = camera.pixelHeight - position.y;
        Vector2 size        = screenCorners[2] - screenCorners[0];
        
        //return screen space rect
        Rect screenRect = new Rect(position, size);
        return screenRect;
    }



    /// <summary>
    /// Helper function to rotate a texture to other orientations
    /// </summary>
    /// <param name="input"> Texture to rotate </param>
    /// <param name="rotation"> Direction of rotation </param>
    /// <returns></returns>
    public static Texture2D RotateTexture(Texture2D input, Rotation rotation = Rotation.clockwise90){        

        //do nothing
        if(rotation == Rotation.none)
            return input;

        
        Color32[] pixelsOut;
        Texture2D output;

        //upside down is just pixel array reversed
        if(rotation == Rotation.uTurn180)
        {
            output      = new Texture2D(input.width, input.height, input.format, false, true);
            pixelsOut   = input.GetPixels32(0);
            System.Array.Reverse(pixelsOut);
        }
        else
        {       
            //flip width and height since we are rotating 90 degrees
            output = new Texture2D(input.height, input.width, input.format, false, true);
            Color32[] pixelsIn  = input.GetPixels32(0);
            int width           = input.width;
            int height          = input.height;
            pixelsOut           = new Color32[width * height];            


            //perform counter-clockwise
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixelsOut[(height - 1) - y + x * height] = pixelsIn[(y * width) + x];
                }
            }

            //if clockwise, it's just counter-clockwise reversed
            if(rotation == Rotation.clockwise90)
                System.Array.Reverse(pixelsOut);
        }


        //set and apply new pixels
        output.SetPixels32(pixelsOut);
        output.Apply();
        return output;
    }
}