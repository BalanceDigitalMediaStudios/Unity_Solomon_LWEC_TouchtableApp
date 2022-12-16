using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DynamicMeshCollider : MonoBehaviour{

    float previousWeight = -1;

    [SerializeField][ReadOnly]SkinnedMeshRenderer _r;
    SkinnedMeshRenderer r{

        get{
            if(_r == null)
                _r = GetComponent<SkinnedMeshRenderer>();
            return _r;
        }
    }
    [SerializeField][ReadOnly]MeshCollider _c;
    MeshCollider c{

        get{
            if(_c == null)
                _c = GetComponent<MeshCollider>();
            return _c;
        }
    }


    void Update(){

        float currentWeight = r.GetBlendShapeWeight(0);
        if(previousWeight != currentWeight)
        {
            previousWeight = currentWeight;
            UpdateMeshCollider();
        }
    }


    public void UpdateMeshCollider(){
        
        Mesh bakedMesh = new Mesh();
        r.BakeMesh(bakedMesh);
        c.sharedMesh = bakedMesh;
    }
}