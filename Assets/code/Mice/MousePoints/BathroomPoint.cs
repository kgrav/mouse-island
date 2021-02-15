using UnityEngine;
using System;

public class BathroomPoint : MousePoint {

    public int maxShits;
    int shits;
    protected override void OnDisengage(Mice.Mouse mouse){
        shits++;
    } 

    public override MPRESPONSE Availability(int mouseIndex){
        return shits >= maxShits ? MPRESPONSE.INACTIVE : base.Availability(mouseIndex);
    }
}