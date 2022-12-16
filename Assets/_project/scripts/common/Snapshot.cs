using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot : MonoBehaviour{

    private static Snapshot _instance;
    public  static Snapshot instance{

        get{
            if(_instance == null)
            {
                Snapshot[] instances = GameObject.FindObjectsOfType<Snapshot>(true);

                //none exist, make a new one
                if(instances.Length == 0)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<Snapshot>();
                    _instance.name = string.Format("[On Demand Singleton] {0}", _instance.GetType().Name);
                }
                if(instances.Length > 0)
                {
                    _instance = instances[0];

                    for(int i = 1; i < instances.Length; i++)
                        Destroy(instances[i]);
                }
            }
            return _instance;
        }
    }




    public void TakeSnapshot(Camera camera, Rect rect, System.Action<Texture2D> onSnapshotComplete, bool allowTransparency = true){

        StartCoroutine(TakeSnapshotRoutine(camera, rect, onSnapshotComplete, allowTransparency));
    }
    IEnumerator TakeSnapshotRoutine(Camera camera, Rect rect, System.Action<Texture2D> output, bool allowTransparency){

        Debug.Log("Taking Snapshot");
        yield return new WaitForEndOfFrame();
        

        //store these for when we are done
        RenderTexture currentTargetRT = camera.targetTexture;
        RenderTexture currentActiveRT = RenderTexture.active;
        

        //RGB32 for transparency, otherwise RGB24
        TextureFormat   format      = allowTransparency ? TextureFormat.RGBA32 : TextureFormat.RGB24;
        Texture2D       outputTex   = new Texture2D((int)rect.width, (int)rect.height, format, false);
        RenderTexture   outputRT    = new RenderTexture(Screen.width, Screen.height, 24);


        camera.targetTexture = outputRT;
        RenderTexture.active = outputRT;


        camera.Render();
        outputTex.ReadPixels(rect, 0, 0);
        outputTex.Apply();


        //restore original settings
        camera.targetTexture = currentTargetRT;
        RenderTexture.active = currentActiveRT;


        Debug.Log("Snapshot Complete");
        output.Invoke(outputTex);
    }
}