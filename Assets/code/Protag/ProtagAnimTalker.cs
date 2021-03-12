using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProtagAnimTalker : NVComponent {
    Animator _anim;
    Animator anim {get{if(!_anim) _anim=GetComponent<Animator>(); return _anim;}}
    ProtagController _con;
    ProtagController con {get{if(!_con) _con = GetComponent<ProtagController>(); return _con;}}
    AudioSource _asc;
    AudioSource asc {get{if(!_asc)_asc=GetComponent<AudioSource>(); return _asc;}}
    void Start(){
        ProtagAnimState[] bs = anim.GetBehaviours<ProtagAnimState>();
        foreach(ProtagAnimState p in bs){
            p.SetTalker(this);
        }
    }


    public void ReceiveMessage(string msg){
        Invoke(msg,0);    
    }

    void PickupHit(){
        if(con.targetInteractive)
        con.targetInteractive.Trigger(con, CONTROL.BBUTTON);
    }

    void InteractHit(){
        if(con.targetInteractive)
        con.targetInteractive.Trigger(con, CONTROL.XBUTTON);
    }

    void EnSit(){
        con.EnSit();
    }

    void EnStand(){
        con.EnStand();
    }

    public void DropHit(){
        if(con.heldItem > -1)
        {
            Items.GetItem(con.heldItem).body.Drop(con);
        }
    }

    void Footstep(){
        float f = UnityEngine.Random.Range(-1.0f, 1.0f)*0.1f;
        asc.pitch = 1 + f;
        if(Ground.fsSound != null){
            asc.PlayOneShot(Ground.fsSound);
        }
        asc.pitch=1;
    }

    void AnimStart(){
        print(
            "anim start"
        );
    }   

    void AnimEnd(){
        print("anim end");
        con.AnimEnd();
    }
}