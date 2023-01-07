using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostcardMaker : ZonedMonobehaviour{

    public delegate void  InitializeEvent (string zoneId);
    public static   event InitializeEvent onHasInitialized;


    [SerializeField, ReadOnly] HabitatData data;
    [Space()]

    //area that draggable stickers can be moved
    [SerializeField] RectTransform _draggableArea;
    public RectTransform draggableArea{get { return _draggableArea; } }

    //bounds of draggable area
    Bounds _draggableBounds;
    public Bounds draggableBounds{        
        get{ if (_draggableBounds.size == Vector3.zero) _draggableBounds = RectTransformHelper.RectTransformToBounds(draggableArea); return _draggableBounds; } }

    //the image that stickers will be dragged onto
    [SerializeField] Image _habitatImage;
    public Image            habitatImage{get { return _habitatImage; } }
    public RectTransform    habitatRect{ get { return habitatImage.transform as RectTransform; } }

    [SerializeField] Animator _trashBinAnimator;
    public Animator         transBinAnimator{get { return _trashBinAnimator; } }
    public RectTransform    trashBinRect{ get { return transBinAnimator.transform as RectTransform; } }


    [Header("Sticker Area")]
    [SerializeField] StickerSpawner spawnerPrefab;

    [SerializeField] Transform      _unlockedGroup;
    public Transform unlockedGroup{get { return _unlockedGroup; } }

    [SerializeField] Transform      _lockedGroup;
    public Transform lockedGroup{get { return _lockedGroup; } }

    [SerializeField] GameObject     maxStickersMessage;
    [SerializeField] int            maxStickers = 10;
    [SerializeField, ReadOnly] int _stickerCount = 0;
    int stickerCount{
        get { return _stickerCount; }
        set { 
            _stickerCount = value;
            maxStickersMessage.SetActive(stickerCount >= maxStickers);
        }
    }

    [Header("Snapshot Settings")]
    [SerializeField] string                 filePrefix = "snapshots\\filePrefix";
    [SerializeField] Camera                 snapshotCamera;
    [SerializeField] RectTransform          snapshotRect;
    [Tooltip("Desired width of the image.  NOTE: This is used BEFORE image rotation, so images that are sideways should enter desired height instead")]
    [SerializeField] int                    snapshotWidth;
    [Tooltip("After snapshot is taken, how should the image be rotated to orient correctly?")]
    [SerializeField] SnapshotMaker.Rotation correctiveRotation = SnapshotMaker.Rotation.none;
    [SerializeField, ReadOnly] Texture2D    outputTexture;



    public void Initialize(HabitatData data){

        StopAllCoroutines();
        StartCoroutine(InitializeRoutine(data));
    }
    IEnumerator InitializeRoutine(HabitatData data){

        this.data = data;

        //set image
        habitatImage.sprite = data.sprite;

        //remove any sticker spawners and draggables        
        DestroyAll<StickerSpawner>(transform);
        DestroyAll<DraggableSticker>(transform);

        //create new spawners based on data
        if(data != null)
            PopulateStickerSpawners();
        
        stickerCount = 0;

        //wait a frame to allow Unity garbage collection before firing event
        yield return new WaitForEndOfFrame();
        if(onHasInitialized != null)
            onHasInitialized(zoneId);
    }

    void DestroyAll<T>(Transform parent) where T : MonoBehaviour{

        T[] temp = parent.GetComponentsInChildren<T>(true);

        foreach(T item in temp)
            Destroy(item.gameObject);
    }



    void PopulateStickerSpawners(){

        for (int i = 0; i < data.stickers.Length; i++)
        {
            StickerSpawner newSpawner = Instantiate(spawnerPrefab, unlockedGroup);
            newSpawner.Initialize(data.stickers[i]);            
        }
    }


    public void OnAddSticker(){

        stickerCount++;
    }  

    public void OnRemoveSticker(){

        stickerCount--;
        transBinAnimator.CrossFadeInFixedTime("pulse", .25f, -1, 0f);
    }
}