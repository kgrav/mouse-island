using System;
using UnityEngine;
using UnityEngine.UI;

public class NVText : NVComponent{
    
    public string text;
    public NVFont font;
    public int kerning;
    public int lineSpacing;
    public int width, height;

    NVChar[] chars;

    void Awake(){
        chars = new NVChar[width*height];
        int indx = 0;
        for(int j = 0; j < height; ++j){
            for(int i = 0; i < width; ++i){
                chars[indx] = Instantiate(new GameObject()).AddComponent<NVChar>();
                chars[indx].tform.parent = tform;
                chars[indx].tform.localPosition = new Vector3(i*kerning, j*lineSpacing,0);

            }
        }
    }

    string lastText;

    void Update(){
        if(!lastText.Equals(text)){
            for(int i = 0; i < chars.Length; ++i){
                if(i < text.Length){
                    chars[i].SetChar(font.chars[(int)text[i]]);
                }
                else{
                    chars[i].SetChar(null);
                }
            }
        }
    }
}