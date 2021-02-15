using UnityEngine;
using System;

public class WorldSpriteDisplay : NVComponent {
        public Sprite[] sprites;
    float time=0;
    public float upTime;
    SpriteRenderer _spriteRenderer;
    SpriteRenderer spriteRenderer {get{if(!_spriteRenderer)_spriteRenderer=GetComponent<SpriteRenderer>(); return _spriteRenderer;}}

    protected override void NVUpdate()
    {
        if(fade){
            time += Time.deltaTime;
            if(time >= upTime){
                spriteRenderer.enabled=false;
                fade=false;
            }
        }
        tform.LookAt(SPCam.cam.tform);
    }
    bool fade = false;

    bool visible {get{return spriteRenderer.enabled;} set {spriteRenderer.enabled=value;}}

    public void SetSprite(int index){
        spriteRenderer.sprite = sprites[index];
    }

    public bool isVisible {get{return visible;}}

    public void SetVisibleAndStay(){
        visible=true;

    }

    public void SetVisibleAndFade(){
        visible=true;
        time=0;
        fade=true;
    }

    public void SetNotVisible(){
        visible = false;
    }


}