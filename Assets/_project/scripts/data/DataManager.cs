using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;
using System.Linq;

public partial class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    string root = Application.streamingAssetsPath + "/XML/";
    const string HABITATS_LOCATION = "habitats.xml";
    const string STICKERS_LOCATION = "stickers.xml";

    [Header("Canvas")]
    [SerializeField] Canvas canvas;

    [Header("Menu Items")]
    [SerializeField] [ReadOnly] List<HabitatButton> habitatMenuButtons;

    [Header("Data")]
    [SerializeField] List<Data.Habitat> habitats;
    [SerializeField] List<Data.Sticker> stickers;

    public void Awake()
    {
        Instance = this;

        ImportHabitatData(root + HABITATS_LOCATION);
        ImportStickerData(root + STICKERS_LOCATION);

        LoadHabitatMenu();
    }

    void ImportHabitatData(string path)
    {

        habitats.Clear();

        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlNode root = doc.ChildNodes[1];
        foreach (XmlNode node in root.ChildNodes)
        {
            habitats.Add(new Data.Habitat()
            {
                id = Convert.ToInt32(node["id"].InnerText),
                title = node["title"].InnerText,
                image_src = node["image_src"].InnerText,
                button_src = node["button_src"].InnerText
            });
        }
    }
    void ImportStickerData(string path)
    {

        stickers.Clear();

        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlNode root = doc.ChildNodes[1];
        foreach (XmlNode node in root.ChildNodes)
        {
            stickers.Add(new Data.Sticker()
            {
                id = Convert.ToInt32(node["id"].InnerText),
                habitatId = Convert.ToInt32(node["fk_habitat_id"].InnerText),
                image_src = node["image_src"].InnerText,
                is_earned = node["is_earned"].InnerText == "1"
            });
        }
    }

    public Data.Habitat[] GetHabitats()
    {

        return habitats.ToArray();
    }

    public Data.Habitat GetHabitat(int habitatId)
    {

        return habitats.FindAll(habitat => habitat.id == habitatId)[0];
    }

    public Data.Sticker[] GetStickersByHabitat(int habitatId)
    {
        return stickers.FindAll(sticker => sticker.habitatId == habitatId).ToArray();
    }

    public void LoadHabitatMenu()
    {
        habitatMenuButtons = canvas.gameObject.GetComponentsInChildren<HabitatButton>(true).ToList();

        foreach(HabitatButton button in habitatMenuButtons)
        {
            button.Initialize();
        }
    }

    #region Extra Functions
    Color HexToColor(string hex)
    {

        Color newCol;
        if (ColorUtility.TryParseHtmlString(hex, out newCol))
            return newCol;

        return newCol;
    }

    public Sprite GetSprite(string image_src)
    {
        return Resources.Load<Sprite>(image_src.Substring(0, image_src.LastIndexOf('.')));
    }
    #endregion
}