using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] CanvasGroup mapIconsContainer;
    [SerializeField] [ReadOnly] List<MapIcon> icons;

    public void Awake()
    {
        Instance = this;

        icons = mapIconsContainer.gameObject.GetComponentsInChildren<MapIcon>(true).ToList();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
