using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiceZone : NVComponent {
    static MiceZone _zone;

    public int slicesx,slicesy;
    public static MiceZone zone {get{if(!_zone)_zone = FindObjectOfType<MiceZone>(); return _zone;}}
    public float radius;
    public Vector3 GetWanderPoint(){
        Vector2 uc = UnityEngine.Random.insideUnitCircle;
        return new Vector3(uc.x*radius, tform.position.y, uc.y*radius);
    }
}