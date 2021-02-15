using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodMP : MousePoint{
    public float foodTotal;

    protected override void Init()
    {
        
        originalFoodTotal = foodTotal;
        originalScale = tform.localScale;
        Activate();
    }
    float originalFoodTotal;

    ParticleSystem _psys;
    ParticleSystem psys {get{if(!_psys)_psys=GetComponent<ParticleSystem>(); return _psys;}}
    Vector3 originalScale;

    public override void Access(Mice.Mouse mouse){
        foodTotal -= Mice.mice.preset.consumptionRate*Time.deltaTime;
        psys.Emit(1);
        foreach(int m in occupants){
            foreach(int n in occupants){
                if(m != n){
                    Mice.GetMouse(m).IncreaseAffection(n, 0.1f*Time.deltaTime);
                }
            }
        }
        tform.localScale = (foodTotal/originalFoodTotal)*originalScale;
        if(foodTotal < 0){
            foreach(int i in occupants){
                Mice.GetMouse(i).DisengageMousePoint(true);
            }
            MousePoint.points.Remove(this);
            Destroy(gameObject);
        }
    }


}