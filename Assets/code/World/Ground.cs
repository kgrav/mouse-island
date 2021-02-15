using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : NVComponent{
    static AudioClip _fsSound;
    public static AudioClip fsSound;


    public AudioClip footStepSound;

    public void OnCollisionEnter(Collision collision){
        if(collision.collider.GetComponent<ProtagController>()){
            _fsSound=footStepSound;
        }
    }  
}