using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum CONTROL{XBUTTON,YBUTTON,BBUTTON}
public class ActionDisplay : MonoBehaviour{
    static ActionDisplay _xButton, _yButton, _bButton;
    public static ActionDisplay xButton{
        get{
            if(!_xButton){
                ActionDisplay[] ad = FindObjectsOfType<ActionDisplay>();
                foreach(ActionDisplay a in ad){
                    switch(a.control){
                        case CONTROL.BBUTTON:
                        break;
                        case CONTROL.YBUTTON:
                        break;
                        case CONTROL.XBUTTON:
                            _xButton = a;
                        break;
                    }
                }

            }
            return _xButton;
        }
    }public static ActionDisplay bButton{
        get{
            if(!_bButton){
                ActionDisplay[] ad = FindObjectsOfType<ActionDisplay>();
                foreach(ActionDisplay a in ad){
                    switch(a.control){
                        case CONTROL.BBUTTON:
                            _bButton = a;
                        break;
                        case CONTROL.YBUTTON:
                        break;
                        case CONTROL.XBUTTON:
                        break;
                    }
                }

            }
            return _bButton;
        }
    }public static ActionDisplay yButton{
        get{
            if(!_yButton){
                ActionDisplay[] ad = FindObjectsOfType<ActionDisplay>();
                foreach(ActionDisplay a in ad){
                    switch(a.control){
                        case CONTROL.BBUTTON:
                        break;
                        case CONTROL.YBUTTON:
                            _yButton = a;
                        break;
                        case CONTROL.XBUTTON:
                        break;
                    }
                }

            }
            return _yButton;
        }
    }
    public CONTROL control;
    Image _img1, _img2;
    public Image img1 {get{if(!_img1)_img1=GetComponent<Image>(); return _img1;}}
    public Image img2;
    public Sprite[] sprites1, sprites2;

    public void SetState(int st){
        if(st < 0 || st >= sprites2.Length){
            img1.sprite = sprites1[0];
            img2.enabled = false;
        }
        else{
            img1.sprite = sprites1[1];
            img2.enabled=true;
            img2.sprite = sprites2[st];
        }
    }
}