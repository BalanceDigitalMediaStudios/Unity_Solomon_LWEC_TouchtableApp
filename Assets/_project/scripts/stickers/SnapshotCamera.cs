using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SnapshotCamera : MonoBehaviour
{

    public string FileNamePrefix = "NatureSelfie";
    public string LastSavedFilePath { get; private set; } = null;

    [SerializeField]
    private Vector2Int textureSize = Vector2Int.zero;

    public Camera snapshotCamera { get; private set; }
    private Texture2D texture;

    public byte[] GetJpgData() { return texture ? texture.EncodeToJPG() : new byte[0]; }
    public byte[] GetPngData() { return texture ? texture.EncodeToPNG() : new byte[0]; }
    public Texture2D GetTexture() { return texture; }
    public Sprite GetSprite() { return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f)); }

    void Awake() { snapshotCamera = GetComponent<Camera>(); }

    void Start()
    {
        Invoke("LateStart", 0.2f);
    }

    void LateStart()
    {
        RenderTexture renderTexture;

        if (textureSize == Vector2Int.zero)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
        }
        else
        {
            renderTexture = new RenderTexture(textureSize.x, textureSize.y, 16);
        }

        snapshotCamera.targetTexture = renderTexture;
    }

    public void CaptureAndSaveSnapshot(string savePath, Action<string> onComplete)
    {
        CaptureSnapshot((data) =>
        {
            SaveSnapshot(savePath, onComplete);
        });
    }

    public void CaptureSnapshot(Action<Texture2D> onComplete)
    {
        StartCoroutine(SnapshotRoutine(onComplete));
    }

    IEnumerator SnapshotRoutine(Action<Texture2D> onComplete)
    {
#if DEBUG
        Debug.Log("Taking snapshot: " + gameObject.name);
#endif

        yield return new WaitForEndOfFrame();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = snapshotCamera.targetTexture;

        snapshotCamera.Render();
        Destroy(texture);
        texture = new Texture2D(snapshotCamera.targetTexture.width, snapshotCamera.targetTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, snapshotCamera.targetTexture.width, snapshotCamera.targetTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRT;
#if DEBUG
        Debug.Log("Snapshot complete");
#endif

        yield return null;

        if (onComplete != null)
        {
            onComplete(texture);
        }

    }

    public void SaveSnapshot(string savePath, Action<string> onComplete)
    {
        var fileName = $"{FileNamePrefix}.jpg"; //string.Format("{0}.jpg", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        var filePath = Path.Combine(savePath, fileName);

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        File.WriteAllBytes(filePath, GetJpgData());

#if DEBUG
        Debug.Log("Snapshot saved: " + filePath);
#endif

        if (onComplete != null)
        {
            onComplete(filePath);
        }

        LastSavedFilePath = filePath;
    }
}