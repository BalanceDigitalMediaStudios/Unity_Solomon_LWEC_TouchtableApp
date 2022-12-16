using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerButton : MonoBehaviour
{

    [SerializeField] Button button;
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] Image image;
    [SerializeField] Transform earnOverlay;

    [Header("Data")]
    [SerializeField] [ReadOnly] Data.Sticker data;


    void Awake()
    {

        button.onClick.AddListener(ButtonAction);
    }

    public void Initialize(Data.Sticker sticker)
    {

        data = sticker;
        image.sprite = DataManager.Instance.GetSprite("stickers/" + sticker.image_src);
        earnOverlay.gameObject.SetActive(!sticker.is_earned);

        float aspectRatio = (float)image.sprite.texture.width / (float)image.sprite.texture.height;
        float width = LayoutUtility.GetPreferredWidth(transform as RectTransform);
        float height = width / aspectRatio;
    }

    void ButtonAction()
    {
        //PhotoMenu.Instance.Open(data);
    }
}