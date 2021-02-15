using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShadeController : MonoBehaviour {
    static ShadeController _shade;
    public static ShadeController shade {get{if(!_shade) _shade=FindObjectOfType<ShadeController>(); return _shade;}}

    public float fadeTime, openTime;
    public string openBool;
    Renderer _rend;
    public Renderer rend {get{if(!_rend)_rend=GetComponentInChildren<SkinnedMeshRenderer>(); return _rend;}}
    Animator _anim;
    public Animator anim {get{if(!_anim)_anim=GetComponent<Animator>(); return _anim;}}

    void Awake(){
        rend.enabled=false;
    }

    public void SetVisible(bool v){
        rend.enabled=v;
    }

    public void SetOpenMouth(bool o){
        anim.SetBool(openBool, o);
    }

}