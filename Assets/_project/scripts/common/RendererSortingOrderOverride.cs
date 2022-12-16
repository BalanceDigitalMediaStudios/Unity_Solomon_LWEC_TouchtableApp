using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererSortingOrderOverride : MonoBehaviour{

    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int    sortingOrder;

    void OnValidate(){

        Renderer renderer           = GetComponent<Renderer>();
        renderer.sortingLayerName   = sortingLayerName;
        renderer.sortingOrder       = sortingOrder;
    }
}