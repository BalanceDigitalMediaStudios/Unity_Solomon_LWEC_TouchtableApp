using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ForceDrawCameraFrustum : MonoBehaviour {

    Camera _camera;
    Camera Camera{

        get{

            if(_camera == null)
                _camera = GetComponent<Camera>();
            
            return _camera;
        }
        set{

            _camera = value;
        }        
    }

    private void OnDrawGizmos()
    {
        if (this.enabled && Camera)
        {
            Gizmos.matrix = Camera.transform.localToWorldMatrix;

            Gizmos.color = Color.yellow;

            Gizmos.DrawFrustum(
                Vector3.zero, 
                Camera.fieldOfView, 
                Camera.farClipPlane, 
                Camera.nearClipPlane, 
                Camera.aspect
                );
        }
    }
}