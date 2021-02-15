using System;
using UnityEngine;

public class AdhocBedPoint : MousePoint {
    int owner;
    public void SetOwner(int i){
        owner = i;
    }

    public override MPRESPONSE Availability(int mouseindex){
        if(!_active || mouseindex != owner)
            return MPRESPONSE.INACTIVE;
        if(occupants.Count > maxCapacity)
            return MPRESPONSE.FULL;
        return MPRESPONSE.AVAILABLE;
    }

    protected override void OnEngage(Mice.Mouse mouse)
    {
    
    }
}