using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Animator>().Play("scale_animation", -1, Random.Range(0.0f, 1.0f));
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
