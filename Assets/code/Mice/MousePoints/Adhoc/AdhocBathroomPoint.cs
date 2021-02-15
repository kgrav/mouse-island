using System;
using UnityEngine;

public class AdhocBathroomPoint : MousePoint{
    int owner;
    public void SetOwner(int i){
        owner = i;
    }

    public override MPRESPONSE Availability(int mouseindex){
        if(!_active || mouseindex != owner)
            return MPRESPONSE.INACTIVE;
        return MPRESPONSE.AVAILABLE;
    }
    protected override void OnDisengage(Mice.Mouse mouse)
    {
        Instantiate(Mice.mice.droppingsPrefab,mouse.body.tform.position,mouse.body.tform.rotation);
    }
}