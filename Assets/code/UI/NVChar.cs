using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NVChar : NVComponent{
    Image _img;
    Image img {get{if(!_img)_img=GetComponent<Image>(); return _img;}}
    void Awake(){
        SetChar(null);
    }
    public void SetChar(Sprite sprite){
        img.enabled = sprite != null;
        img.sprite = sprite;
    }
}