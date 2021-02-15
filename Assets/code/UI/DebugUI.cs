using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugUI : MonoBehaviour{
    static DebugUI _dbui;
    static DebugUI dbui {get{if(!_dbui)_dbui=FindObjectOfType<DebugUI>();return _dbui;}}
    
    public static void ActivateMouseUI(int mouseIndex){
        if(dbui)
            dbui.Activate(mouseIndex);
    }

    /*public static void DeactivateMouseUI(){
        if(dbui)
            dbui.DebugUI();
    }*/
    int i = 0;
    Image _img;
    Image img {get{if(!_img) _img=GetComponent<Image>(); return _img;}}
    Text _txt;
    Text txt {get{if(!_txt) _txt = GetComponentInChildren<Text>(); return _txt;}}

    void Activate(int mouseIndex){
        i = mouseIndex;
        txt.enabled=true;
        img.enabled=true;
    }

    void Update(){
        InteractTarget.itar.target = Mice.GetMouse(i).body.tform.position;
        InteractTarget.itar.SetActive(true);
        txt.text = Mice.GetMouse(i).StatsString();
        if(Input.GetButtonDown("RB")){
            i++;
            if(i >= Mice.population)
                i = 0;
        }
    }

    void Deactivate(){
        txt.enabled=false;
        img.enabled=false;
    }
}