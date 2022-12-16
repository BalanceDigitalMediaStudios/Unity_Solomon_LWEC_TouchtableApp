using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//inherit this for a singleton on a gameobject in a scene
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>{

    private static bool hasInstantiated;
    private static bool applicationIsQuitting;


    private static T _instance;
    public  static T instance
    {
        get
        {
            if(applicationIsQuitting)
            {
                Debug.LogWarningFormat("[Singleton] {0}: application is quitting, returning null", typeof(T));
                return null;
            }
            //if not already instanced
            if(!hasInstantiated)
            {
                //find all objects of this type
                Object[] objects = Resources.FindObjectsOfTypeAll(typeof(T));

                //if none found, create one
                if(objects == null || objects.Length < 1)
                {
                    GameObject singleton    = new GameObject();
                    singleton.name          = string.Format("[On Demand Singleton] {0}", typeof(T));
                    instance                = singleton.AddComponent<T>();
                    
                    Debug.LogWarningFormat("[Singleton] {0}: has no instances in scene.  Creating GameObject: {1}", typeof(T), singleton.name);
                }
                //if multiple found, choose first and remove the rest
                else if(objects.Length >= 1)
                {
                    instance = objects[0] as T;
                    if(objects.Length > 1)
                    {
                        Debug.LogWarningFormat("[Singleton] {0}: {1} instances in scene", typeof(T), objects.Length);
                        for(int i = 1; i < objects.Length; i++)
                        {
                            Debug.LogWarningFormat("[Singleton] {0}: Removing extra Instance on GameObject: {1}", typeof(T), objects[i].name);
                            Destroy(objects[i]);
                        }
                    }
                }
            }
            return _instance;
        }
        protected set
        {
            _instance = value;
            hasInstantiated = true;
            //DontDestroyOnLoad(_instance.gameObject);
        }
    }
    


    //for instances of this type already in the scene
    protected virtual void Awake(){
        
        if(!hasInstantiated)
        {
            instance = this as T;
            //name = string.Format("[Singleton] {0}", typeof(T));
        }
        
        else if(instance.GetInstanceID() != GetInstanceID())
        {
            Debug.LogWarningFormat("[Singleton] {0}: Removing extra Instance on GameObject: {1}", typeof(T), name);
            Destroy(this);
        }
        PostAwake();
    }
    protected virtual void PostAwake(){}


    protected void OnApplicationQuit(){

        applicationIsQuitting = true;
    }
}