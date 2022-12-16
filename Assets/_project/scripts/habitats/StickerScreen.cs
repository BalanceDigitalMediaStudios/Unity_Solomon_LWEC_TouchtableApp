using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class StickerScreen : SimpleSingleton<StickerScreen>
{
    [SerializeField] Button backButton;

    [Header("Panel")]
    [SerializeField] UITransitionFade stickerScreen;
    [SerializeField] Transform habitatMenu;
    [SerializeField] CanvasGroup infoScreen;

    [Header("Stickers")]
    [SerializeField] Image stickerCanvas;
    [SerializeField] StickerButton stickerPrefab;
    [SerializeField] Transform activeStickerContainer;
    [SerializeField] Transform lockedStickerContainer;

    [Header("Data")]
    [SerializeField] [ReadOnly] Data.Habitat selectedHabitat;
    [SerializeField] [ReadOnly] Data.Sticker[] stickerList;

    void Awake()
    {
        backButton.onClick.AddListener(BackButton);
    }

    public void Open(Data.Habitat data)
    {
        selectedHabitat = data;

        backButton.interactable = true;

        //assign canvas image
        stickerCanvas.sprite = DataManager.Instance.GetSprite(data.image_src);

        //assign stickers
        PopulateStickers(DataManager.Instance.GetStickersByHabitat(data.id));

        //Setup information screen
        infoScreen.alpha = 1;
        infoScreen.gameObject.SetActive(true);

        stickerScreen.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;
        stickerScreen.gameObject.SetActive(true);
        stickerScreen.TransitionToEnd(true);

    }

    void PopulateStickers(Data.Sticker[] stickers)
    {
        //clear all pre-existing stickers
        foreach (Transform t in activeStickerContainer)
            Destroy(t.gameObject);
        foreach (Transform t in lockedStickerContainer)
            Destroy(t.gameObject);

        int counter = 0;
        //popuplate stickers from data
        foreach (Data.Sticker p in stickers.Where(sticker => sticker.is_earned))
        {
            counter++;
            StickerButton temp = Instantiate(stickerPrefab, activeStickerContainer).GetComponent<StickerButton>();
            temp.Initialize(p);

            //Setup information screen (use 3rd earned sticker as example on info screen)
            if(counter == 3)
                infoScreen.GetComponentsInChildren<StickerButton>().Where(sticker => sticker.gameObject.name == "sticker").First().GetComponentsInChildren<Image>().First().sprite = DataManager.Instance.GetSprite("stickers/" + p.image_src);

        }

        //Reset Counter
        counter = 0;
        foreach (Data.Sticker p in stickers.Where(sticker => !sticker.is_earned))
        {
            counter++;
            StickerButton temp = Instantiate(stickerPrefab, lockedStickerContainer).GetComponent<StickerButton>();
            temp.Initialize(p);

            //Setup information screen (use 1st locked sticker as example on info screen)
            if (counter == 1)
                infoScreen.GetComponentsInChildren<StickerButton>().Where(sticker => sticker.gameObject.name == "locked_sticker").First().GetComponentsInChildren<Image>().First().sprite = DataManager.Instance.GetSprite("stickers/" + p.image_src);
        }
    }

    void Close()
    {
        backButton.interactable = false;
        if (stickerScreen.gameObject.activeInHierarchy)
        {
            habitatMenu.gameObject.SetActive(true);
            stickerScreen.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
            stickerScreen.TransitionToStart(true);
        }
    }

    void BackButton()
    {
        Close();
    }
}