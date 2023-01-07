using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonedMonobehaviour : MonoBehaviour{

    private string _zoneId = string.Empty;
    protected string zoneId{

        get
        {
            if (_zoneId == string.Empty && TryGetComponentInParent<Zone>(out Zone zone))
            {
                _zoneId = zone.zoneId;
            }
            return _zoneId;            
        }    
    }

    bool TryGetComponentInParent<T>(out T result) where T:MonoBehaviour{

        result = GetComponentInParent<T>(true);
        return result != null;
    }

    protected bool IsThisZone(string zoneId){return this.zoneId == zoneId;}
}