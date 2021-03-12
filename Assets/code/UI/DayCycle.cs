using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DayCycle : MonoBehaviour {
    static DayCycle _dnc;
    public static DayCycle dnc{get{if(!_dnc) _dnc=FindObjectOfType<DayCycle>(); return _dnc;}}


    public static float hour{get{return dnc.secondsPerDay/24;}}


    int _sundayCount;
    public static int sundayCount {get{return dnc._sundayCount;}}


    Slider _slider;
    Slider slider {get{if(!_slider)_slider = GetComponent<Slider>(); return _slider;}}
    public Image handle;
    public Text text;

    public Sprite dayHandle, nightHandle;
    bool isSunday = false;
    public bool sunday{get{return _day%5==0;}}
    public float secondsPerDay;
    float totalTime=0;
    int _day=1;
    float tod;
    void Update(){
        totalTime += Time.deltaTime;
        if(Mathf.Ceil(totalTime/secondsPerDay)>_day){
            _day = (int)Mathf.Ceil(totalTime/secondsPerDay);
            Mice.NewDay();
            tod=0;
        }
        tod += Time.deltaTime;
        float normaltod = tod/secondsPerDay;
        slider.value = normaltod;
        if(normaltod > (6/24) && normaltod < (18/24))
            handle.sprite = dayHandle;
        else
            handle.sprite = nightHandle;
        text.text="DAY "+_day;
        if(sunday&&!isSunday){
            
        }
    }
}