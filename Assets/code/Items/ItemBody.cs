using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemBody : Interactive {

    public Sprite ui;
    public XTYPE xType;
    public YTYPE yType;
    Renderer _r=null;
    Renderer r {get{if(!_r) _r = GetComponent<Renderer>(); return _r;}}
    Rigidbody _rbody;
	Rigidbody rbody{get{if(!_rbody)_rbody=GetComponent<Rigidbody>(); return _rbody;}}
    Collider _col;
    Collider col {get{if(!_col) _col = GetComponent<Collider>(); return _col;}}
    int _index;
    public int index{get{return _index;}set{_index=value;}}
    MousePoint mousePoint;
    public bool storable;
    public bool serialized;
    int _mouseIndex = -1;
    public int mouseIndex {get{return _mouseIndex;}}
    bool visible;
    public ITEMTYPE type;
    void Awake(){
        mousePoint = GetComponent<MousePoint>();
        if(!serialized){
            Items.items.CreateItem(this);
        }
    }
    public void SetAttachPoint(Vector3 localPos, Transform parent, ProtagController protag){
        rbody.isKinematic = true;
        rbody.useGravity = false;
        col.enabled=false;
        tform.parent = parent;
        tform.localPosition = localPos;
        if(mousePoint)
        {mousePoint.Deactivate();}
        if(mouseIndex > -1){
            Mice.GetMouse(mouseIndex).SetHeld(true);
        }
    }
    public void SetMouseIndex(int i){
        _mouseIndex = i;
        Items.GetItem(index).SetMouseIndex(i);
    }

    public bool mouse;

    public void SetVisible(bool visible){
        r.enabled=visible;
    }
    public void Drop(ProtagController protag){
        
        Items.GetItem(index).SetLocation(protag.room);
        protag.SetHold(-1);
        tform.parent = null;
        if(mousePoint)
        {mousePoint.Activate();}
        col.enabled=true;
        rbody.isKinematic = false;
        rbody.useGravity=true;
    }

    public override void Trigger(ProtagController protag, CONTROL button)
    {
        switch(button){
            case CONTROL.BBUTTON:
                switch(Items.GetItem(index).iLoc){
                    case ILOC.BEACH:
                    case ILOC.FOREST:
                    case ILOC.VOID:
                    case ILOC.HOUSE:
                        protag.SetHold(index);
                        Items.GetItem(index).SetLocation(ILOC.HAND);
                        SetAttachPoint(Vector3.zero, protag.attachBone,protag);
                    break;
                    case ILOC.HAND:
                    break;

                }
            break;
        }
    }
}