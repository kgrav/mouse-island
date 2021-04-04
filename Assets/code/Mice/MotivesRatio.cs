using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class MotivesRatio{
    public Motive fullness;
    public Motive physicalEnergy;
    public Motive socialEnergy;
    public Motive sexEnergy;
    public Motive bathroom;
    public Motive boredom;

    public Motive GetMotive(MSTIMULUS motive){
        switch(motive){
            case MSTIMULUS.HUNGER:
                return fullness;
            case MSTIMULUS.ENERGY:
                return physicalEnergy;
            case MSTIMULUS.SOCIAL:
                return socialEnergy;
            case MSTIMULUS.SEXDRIVE:
                return sexEnergy;
            case MSTIMULUS.BATHROOM:
                return bathroom;
            case MSTIMULUS.BOREDOM:
                return boredom;
        }
        return new Motive();
    }

    public class Motive {
        public float value;
        public bool use;
        public int rating;
        public bool passive;
    }

}