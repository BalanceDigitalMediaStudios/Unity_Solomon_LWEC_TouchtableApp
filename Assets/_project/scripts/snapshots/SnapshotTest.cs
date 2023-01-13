using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


#if(UNITY_EDITOR)
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SnapshotTest))]
public class SnapshotTestEditor : Editor{

    SnapshotTest t;

    protected virtual void OnEnable(){

        t = (SnapshotTest)target;
    }

    public override void OnInspectorGUI(){

        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Snapshot Normal"))
                t.SaveNormal();
            if (GUILayout.Button("Snapshot Upside Down"))
                t.SaveUpsideDown();
            if (GUILayout.Button("Snapshot Clockwise"))
                t.SaveClockwise();
            if (GUILayout.Button("Snapshot Counter Clockwise"))
                t.SaveCounterClockwise();
        }        
    }
}
#endif



public class SnapshotTest : MonoBehaviour{

    public Camera                   cam;
    public RectTransform            rect;
    public string                   filePrefix  = "test";
    public int                      width       = 1920;
    public Texture2D output;

    void Awake(){

        if(!rect)
            rect = GetComponent<RectTransform>();
    }

    public void SaveNormal(){

        SnapshotMaker.instance.TakeSnapshot(cam, SnapshotMaker.RectTransformToScreenSpaceRect(cam, rect), width, (texture) => 
        {
            Save(texture);
            //Store(texture);
        });
    }
    public void SaveUpsideDown(){

        SnapshotMaker.instance.TakeSnapshot(cam, SnapshotMaker.RectTransformToScreenSpaceRect(cam, rect), width, (texture) => 
        {
            texture = SnapshotMaker.RotateTexture(texture, SnapshotMaker.Rotation.uTurn180);
            Save(texture);
            //Store(texture);
        });
    }

    public void SaveClockwise(){

        SnapshotMaker.instance.TakeSnapshot(cam, SnapshotMaker.RectTransformToScreenSpaceRect(cam, rect), width, (texture) => 
        {
            texture = SnapshotMaker.RotateTexture(texture, SnapshotMaker.Rotation.clockwise90);
            Save(texture);
            //Store(texture);
        });
    }

    public void SaveCounterClockwise(){

        SnapshotMaker.instance.TakeSnapshot(cam, SnapshotMaker.RectTransformToScreenSpaceRect(cam, rect), width, (texture) => 
        {
            texture = SnapshotMaker.RotateTexture(texture, SnapshotMaker.Rotation.counterClockwise90);
            Save(texture);
            //Store(texture);
        });
    }

    void Save(Texture2D texture){

        string fileName = string.Format ("{0}_{1}.{2}", filePrefix, System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"), "jpg");
        string path     = Path.Combine(Application.streamingAssetsPath, fileName);
        byte[] bytes    = texture.EncodeToJPG ();

        System.IO.File.WriteAllBytes (path, bytes);
    }

    void Store(Texture2D texture){ output = texture; }
}