using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTarget : NVComponent {
    static InteractTarget _itar;
    public static InteractTarget itar {get{if(!_itar) _itar = FindObjectOfType<InteractTarget>(); return _itar;}}

    Renderer _r;
    Renderer r {get{if(!_r) _r = GetComponentInChildren<Renderer>(); return _r;}}


    public Vector3 target;
    public bool debugMode;

    void Update(){
        tform.position = target;
    }

    public void SetActive(bool active){
        r.enabled = active;
    }
}