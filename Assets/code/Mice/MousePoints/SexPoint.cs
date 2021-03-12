using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SexPoint : MousePoint{


    int mother(Mice.Mouse mouse){
        return mouse.female ? mouse.index : mouse.currMate;
    }

    int father(Mice.Mouse mouse){
        return mouse.female ? mouse.currMate : mouse.index;
    }
    protected override void OnEngage(Mice.Mouse mouse)
    {
        if(occupants.Contains(mouse.currMate)){
            if(mouse.female != Mice.GetMouse(mouse.currMate).female)
            Mice.mice.CreateBaby(mother(mouse), father(mouse));
        }
    }

    public override void Access(Mice.Mouse mouse){
        mouse.IncreaseAffection(mouse.currMate, 5);
    }

}