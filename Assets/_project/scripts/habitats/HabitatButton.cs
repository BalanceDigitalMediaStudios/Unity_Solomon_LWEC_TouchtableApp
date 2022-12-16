using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabitatButton : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] int habitatId;
    [SerializeField] Button button;

    [Header("Screens")]
    [SerializeField] Transform habitatMenu;
    [SerializeField] StickerScreen stickerScreen;

    [Header("Data")]
    [SerializeField] [ReadOnly] Data.Habitat data;

    void Awake()
    {
        button.onClick.AddListener(ButtonAction);

        GetComponent<Animator>().Play("move_oval", -1, Random.Range(0.0f, 1.0f));
    }

    public void Initialize()
    {
        data = DataManager.Instance.GetHabitat(habitatId);
    }

    void ButtonAction()
    {
        habitatMenu.gameObject.SetActive(false);
        stickerScreen.Open(data);
    }
}