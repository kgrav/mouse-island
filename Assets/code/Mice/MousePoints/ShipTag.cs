using UnityEngine;
using System;
[Serializable]
public class ShipTag : NVComponent{
    public int relationshipKey;
    public Vector3 affinity;public bool mouse {get{return relationshipKey >= 0 && relationshipKey < 1000;}}
}