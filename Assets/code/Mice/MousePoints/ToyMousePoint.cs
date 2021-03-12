using System;
using UnityEngine;

public class ToyMousePoint : MousePoint {
    public float baseForce;
    public Color affinity;
    public Vector3 vectorAffinity {get{return new Vector3(affinity.r, affinity.g, affinity.b);}}
    Rigidbody _rg;
    Rigidbody rg {get{if(!_rg) _rg = GetComponent<Rigidbody>(); return _rg;}}

    protected override void OnEngage(Mice.Mouse mouse)
    {
        mouse.EngageToyMP(this);
        Vector3 forceDir = (mouse.body.tform.position-tform.position).normalized;
        rg.AddForce(baseForce*(mouse.redShift+0.5f)*forceDir);
        foreach(int i in pullAuditTrail){
            mouse.IncreaseAffection(1, 0.2f);
        }
    }
}