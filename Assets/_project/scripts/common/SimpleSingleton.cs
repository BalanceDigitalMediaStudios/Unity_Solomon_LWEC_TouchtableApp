using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//inherit this for a singleton on a gameobject in a scene
public abstract class SimpleSingleton<T> : MonoBehaviour where T : SimpleSingleton<T>{

    private static bool hasInstantiated;
    private static bool applicationIsQuitting;


    private static T _instance;
    public  static T instance{

        get
        {
            if(_instance == null)
            {
                T[] instances = GameObject.FindObjectsOfType<T>(true);

                if(instances.Length == 0)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<T>();
                    _instance.name = string.Format("[On Demand Singleton] {0}", _instance.GetType().Name);
                }
                if(instances.Length > 0)
                {
                    _instance = instances[0];

                    for(int i = 1; i < instances.Length; i++)
                        Destroy(instances[i]);
                }
            }
            return _instance;
        }
    }
}