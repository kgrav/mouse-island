using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MicePreset")]
public class MicePreset : ScriptableObject {

    public MousePreset[] mice;


    public float hungerRateG,hungerThreshold;
    public float energyLevelG, speedG,consumptionRate;
    

    [System.Serializable]
    public class MousePreset{
        public bool female;
        public float hungerRate;
        public float affectionRate, socialStamina;
        public float energyLevel;
        public float maxScale;
        public float age, maxAge;
        public float sexEnergyMod;
        public MotivesRatio motiveMods;
        
        public Color color;

        void update(){
        }
    }
}