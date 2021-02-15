using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseBody : NVComponent{
    int _index = -1;
    public int index {get{return _index;}}
    Vector3 lastPosition;

    public bool dummy;

    public Renderer body;
    Renderer[] _r=null;
    Renderer[] r {get{if(_r==null) _r = GetComponentsInChildren<Renderer>(); return _r;}}

    public Rigidbody _rbody;
    public Rigidbody rbody {get{if(!_rbody) _rbody = GetComponent<Rigidbody>(); return _rbody;}}

    bool visible;

    WorldSpriteDisplay _emo;
    public WorldSpriteDisplay emo {get{if(!_emo) _emo = GetComponentInChildren<WorldSpriteDisplay>(); return _emo;}}
    ItemBody _iBody;
    ItemBody iBody {get{if(!_iBody) _iBody = GetComponent<ItemBody>(); return _iBody;}}


    
    public void SetupBody(float scale,Color color,int index){
        tform.localScale=scale*tform.localScale;
        tform.position = MiceZone.zone.GetWanderPoint();
        body.material.color = color;
        if(iBody)
            iBody.SetMouseIndex(index);
        this._index = index;
    }

    public void SetVisibility(bool vis){
        visible = vis;
        foreach(Renderer x in r){

        x.enabled = vis;
        }
    }
}