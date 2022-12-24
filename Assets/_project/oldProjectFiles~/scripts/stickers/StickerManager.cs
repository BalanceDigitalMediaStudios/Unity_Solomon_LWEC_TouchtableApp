using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct StickersPack
{
    public string packName;
    public StickersGroup[] stickerGroups;
    public string packDirectory;
}

[System.Serializable]
public struct StickersGroup
{
    public string groupName;

    public Texture2D[] spriteTextures;
    public Sprite[] groupSprites;

    public string[] spriteLocations;

    public float groupSize;
    public string groupDirectoryName;
    public string groupDirectory;

    public StickersGroup(StickersGroup newGroup)
    {

        this.groupName = newGroup.groupName;

        this.groupSprites = newGroup.groupSprites;
        this.spriteTextures = newGroup.spriteTextures;

        this.groupSize = newGroup.groupSize;

        this.groupDirectoryName = newGroup.groupDirectoryName;
        this.groupDirectory = newGroup.groupDirectory;
        this.spriteLocations = newGroup.spriteLocations;

    }
}

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;

    [SerializeField] Button backButton;
    [SerializeField] [ReadOnly] int currentHabitatId;

    [Header("TEMP")]
    [SerializeField] private int habitatId;

    [Header("Sticker Bars")]
    [SerializeField] Transform activeStickerContainer;
    [SerializeField] Transform lockedStickerContainer;
    [SerializeField] GameObject stickerPrefab;

    private List<GameObject> stickers = new List<GameObject>();

    [Header("Object References")]
    [SerializeField]
    private RectTransform dragAndDropParent; // Sticker instantiation location
    [SerializeField]
    private RectTransform stickerCameraParent; // Transform to move stickers to snapshotcamera
    [SerializeField]
    private RectTransform stickerManipulationParent; // transform to move stickers to manipulation screen
    public RectTransform StickerCameraRect { get { return stickerCameraParent; } }
    public RectTransform StickerManipulationRect { get { return stickerManipulationParent; } }

    public float ManipToCameraRatio { get { return (float)StickerManipulationRect.rect.width / StickerCameraRect.rect.width; } }
    public float CameraToManipRatio { get { return (float)StickerCameraRect.rect.width / StickerManipulationRect.rect.width; } }

    [Header("Trash Can")]
    [SerializeField]
    private StickerTrash stickerTrash;
    [SerializeField]
    private GameObject stickerTrashGraphic;

    [Header("Group References")]
    [SerializeField]
    [ReadOnly]
    private List<StickerElement> stickerElements = new List<StickerElement>();

    [SerializeField]
    private UnityEngine.UI.Button clearButton;

    private bool stickersLoaded = false;
    // Use this for initialization
    void Awake()
    {
        if (!Instance) { Instance = this; }
    }

    void Start()
    {
        if (clearButton)
            clearButton.onClick.AddListener(() => ClearStickers());

        SetStickerTrashActive(false);
        LoadStickers();
    }

    private void OnEnable()
    {
        // Check clear all visibility
        SetClearAllVisibility();

        LoadStickers();
    }

    private void OnDisable()
    {
        //ClearStickers();
        //UnloadStickers();
    }



    private void LoadStickers()
    {
        if (!stickersLoaded)
        {
            foreach (var sticker in DataManager.Instance.GetStickersByHabitat(habitatId))
            {
                //CreateSticker();
            }
            stickersLoaded = true;
        }
    }

    public void MoveStickersBetweenViews(bool toCamera)
    {
        if (toCamera)
        {
            dragAndDropParent.SetParent(StickerCameraRect);
            dragAndDropParent.localScale = Vector3.one * CameraToManipRatio;
        }
        else
        {
            dragAndDropParent.SetParent(StickerManipulationRect);
            dragAndDropParent.localScale = Vector3.one; // Assumes the dragDropParent starts at the same scale
        }

        dragAndDropParent.localPosition = Vector3.zero;
    }



    /// <summary>
    /// Default way to make a sticker. Is called if no face is detected.
    /// </summary>
    void CreateSticker(Sprite stickerSprite)
    {
        CreateSticker(stickerSprite, 1);
    }

    /// <summary>
    /// Standard way to make stickers
    /// </summary>
    public void CreateSticker(Sprite stickerSprite, float size)
    {
        CreateSticker(stickerSprite, size, Vector2.zero);

    }

    public void CreateSticker(Sprite stickerSprite, float size, Vector2 position)
    {
        //Instantiate StickerElement Prefab
        GameObject sticker = Instantiate(stickerPrefab, dragAndDropParent);

        sticker.GetComponent<StickerElement>().stickerImage.sprite = stickerSprite;
        sticker.GetComponent<UnityEngine.UI.Image>().sprite = stickerSprite;
        sticker.GetComponent<RectTransform>().pivot = new Vector2(stickerSprite.pivot.x / stickerSprite.rect.size.x, stickerSprite.pivot.y / stickerSprite.rect.size.y);
        StickerElement stickerEle = sticker.GetComponent<StickerElement>();

        //Set the size of the sticker
        stickerEle.stickerSizeRatio = size;

        //Set the starting position - for drag and drop
        stickerEle.dragStartPosition = position;

        // Add to list
        stickerElements.Add(stickerEle);

        sticker.name = "sticker_" + stickerElements.Count.ToString();

        // Check clear all visibility
        SetClearAllVisibility();

    }

    public void ClearStickers()
    {
        foreach (StickerElement sticker in stickerElements)
        {
            Destroy(sticker.gameObject);
        }

        //Debug, in case things go wrong and we miss some stickers
        foreach (StickerElement obj in GameObject.FindObjectsOfType<StickerElement>())
        {

            Destroy(obj.gameObject);
        }

        stickerElements.Clear();

        SetClearAllVisibility();

    }

    public void RemoveSticker(StickerElement sticker)
    {
        stickerElements.Remove(sticker);

        // Check clear all visibility
        SetClearAllVisibility();
    }

    public void SetStickersVisibility(bool value)
    {
        stickerCameraParent.GetComponent<CanvasGroup>().alpha = value ? 1 : 0;
        stickerCameraParent.GetComponent<CanvasGroup>().blocksRaycasts = value;
    }

    public void SetStickerTrashActive(bool isActive)
    {
        stickerTrashGraphic.GetComponent<CanvasGroup>().alpha = (float)(isActive ? 1 : 0.5);
        stickerTrashGraphic.SetActive(isActive);
    }

    public void CheckTrash(StickerElement sticker)
    {
        SetStickerTrashActive(false);
        stickerTrash.CheckDeleteSticker(sticker);
    }

    public void SetClearAllVisibility()
    {

        clearButton.gameObject.SetActive(stickerElements.Count != 0);

    }
}
