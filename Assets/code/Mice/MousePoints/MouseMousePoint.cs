using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMousePoint : MousePoint {

    static List<MouseMousePoint> _socialPoints;
    public static List<MouseMousePoint> socialPoints{get{return _socialPoints;}}
    MouseBody _body;
    public MouseBody body {get{if(!_body)_body=GetComponent<MouseBody>();return _body;}}

    protected override void Init()
    {
        if(_socialPoints==null)
            _socialPoints = new List<MouseMousePoint>();
        _socialPoints.Add(this);
    }

    public override MPRESPONSE Availability(int mouseIndex){
        return Mice.GetMouse(body.index).drySocialize(mouseIndex) ? base.Availability(mouseIndex) : MPRESPONSE.REJECTED;
    }

    public override void Access(Mice.Mouse mouse){
        mouse.IncreaseAffection(body.index, 1);
    }

    protected override void OnEngage(Mice.Mouse mouse)
    {
        Mice.GetMouse(body.index).SetMousePointCallback(mouse.socializePoint);
    }

    protected override void OnDisengage(Mice.Mouse mouse){
        Mice.GetMouse(body.index).DisengageMousePoint(true);
    }


}