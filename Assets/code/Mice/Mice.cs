using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MSTATE {MOVE, EAT, SLEEP, WAIT, SOCIALIZE, FUCK, HELD, USEMP}
public enum MSTIMULUS{ NONE=-1,HUNGER=0, ENERGY=1, SOCIAL=2, SEXDRIVE=3, BATHROOM=4, BOREDOM=5, CALL=6}
public class Mice : MonoBehaviour {
    static Mice _mice;
    public static Mice mice {get{if(!_mice) _mice = FindObjectOfType<Mice>(); return _mice;}}
    public GameObject droppingsPrefab;
    public static Mouse GetMouse(int mouseIndex){
        return mice.activeMice[mouseIndex];
    }

    public static int population {get{return Mice.mice.activeMice.Count;}}

    public AdhocBedPoint adhocBedPointPrefab;
    public AdhocBathroomPoint adhocBathroomPointPrefab;

    public static AdhocBedPoint CreateAdhocBedPoint(int i, Vector3 point){
        AdhocBedPoint q = Instantiate(Mice.mice.adhocBedPointPrefab, point, Quaternion.identity).GetComponent<AdhocBedPoint>();
        q.SetOwner(i);
        return q;
    }

    public static AdhocBathroomPoint CreateAdhocBathroomPoint(int i, Vector3 point){
        AdhocBathroomPoint q = Instantiate(Mice.mice.adhocBathroomPointPrefab, point, Quaternion.identity).GetComponent<AdhocBathroomPoint>();
        q.SetOwner(i);
        return q;
    }



    public static void NewDay(){
        if(DayCycle.dnc.sunday)
            mice.marked = new List<int>();
        foreach(Mouse m in mice.activeMice){
            m.NewDay();
        }
    }

    List<int> marked;
    int populationCount=0;

    public float pubertyf, pubertym;
    public float lifeExpectancy;
    public MotivesRatio waitMotives, moveMotives;
    public int popcount {get{return populationCount;}}

    public Emote[] emojiTable;
    public static float MiceScale(float age, float maxage, float maxscale){
        float sc = Mathf.Min(1, (age/(maxage/2)))*maxscale;
        return Mathf.Max(sc, 0.2f);
    }
    public static string MouseName(){
        return "";
    }
        public float hungerThreshold;
        public float mouseSenseRadius;
    float hungerRateG;
    float energyLevelG, speedG, consumptionRate;
    public Vector2 rstepRange;
    public void Breed(int f, int m){
        activeMice.Add(new Mouse(activeMice.Count, f, m));
    }
    public static float rstep{get{return UnityEngine.Random.Range(Mice.mice.rstepRange.x, Mice.mice.rstepRange.y);}}

    public float baseProtagAffection, baseMouseAffection;
    public GameObject bodyPrefab;
    public bool destroyOnLoad;
    public float globalMiceScale;
    public MicePreset preset;
    void Awake(){
        if(!destroyOnLoad){
            DontDestroyOnLoad(mice);
            
        }
        marked = new List<int>();
        hungerRateG=preset.hungerRateG;
        energyLevelG=preset.energyLevelG;
        speedG=preset.speedG;
        consumptionRate=preset.consumptionRate;
        if(activeMice == null){
                activeMice = new List<Mouse>();
                for(int i = 0; i < preset.mice.Length; ++i){
                    activeMice.Add(new Mouse(i, preset.mice[i]));
                }
            }
    }
    void Update(){
        populationCount=0;
        foreach(Mouse m in activeMice){
            if(m.alive){
                populationCount++;
                m.UpdateMouse();
            }
        }
    }

    static float rmod{get{return UnityEngine.Random.Range(0.7f,1);}}

    List<Mouse> activeMice;

    public class Mouse {
        public bool female;
        public int index;
        public float scale, maxScale;
        public float age, maxAge;
        public MSTATE state;
        public MSTIMULUS motive;
        public MouseBody body;
        MouseMousePoint _socializePoint;
        public MouseMousePoint socializePoint{get{if(!_socializePoint) _socializePoint = body.GetComponent<MouseMousePoint>(); return _socializePoint;}}
        float rstep, rtime;
        float speed;
        float affectionRate;
        float[] _relationships;
        public float[] relationships{get{return _relationships;}}
        float redShift;
        float blueShift;
        float greenShift;
        bool _alive = true;
        public bool alive {get{return _alive;}}
        MousePoint goalMousePoint, callbackMousePoint;
        Vector3 goalPoint;
        float timeInState=0, timeInMotive=0;
        public float protagRelationship;
        bool rbool {get{return UnityEngine.Random.Range(0,1) > 0.5f;}}
        List<int> relationshipsView;
        Appetite[] motives;
        MotivesRatio motiveMod;

        int father=-1,mother=-1;

        float dayExercise;
        float dayFeeding;
        float dayAging;


        public string StatsString() {
                                            string r = "index: " + index + "\n"
                                            + "state: " + state + "\n"
                                            + "affectionRate: " + affectionRate/DayCycle.hour + "\n"
                                            + "scale: " + scale + "\n"
                                            + "age: " + age + "/"+maxAge+"\n"
                                            + "motive: " + motive +"\n"
                                            + "rstep: " + rtime + "/" + rstep + "\n";
                                            
                                            for(int i = 0 ; i < motives.Length; ++i){
                                                r += (MSTIMULUS)i + ": " + motives[i].value + " / " + motives[i].max + "\n";
                                            }

                                            r+= "relationships: {"; 

                                            foreach(int i in relationshipsView)
                                            { 
                                                r += i + ": " + relationships[i] + ", ";
                                            };
                                            r += "} + \n"; 
                                             return r;
                                    }

        public void NewDay(){
            age+= dayAging;
            scale = Mice.MiceScale(age+0.1f, this.female ? Mice.mice.pubertyf : Mice.mice.pubertym, maxScale);
            print(index + ": feeding: " + dayFeeding + " exercise: " + dayExercise);
        } 

        public Mouse(int index, int father, int mother){
            Mouse f = Mice.GetMouse(father);
            Mouse m = Mice.GetMouse(mother);
            this.father=father;
            this.mother=mother;
            motives = new Appetite[7];
            this.index=index;
            GameObject q = Instantiate(Mice.mice.bodyPrefab);
            body = q.GetComponent<MouseBody>();
            body.tform.parent = mice.transform;
            age = 0;
            maxAge = (m.maxAge + f.maxAge)/2f;
            rstep = Mice.rstep;
            rtime=0;
            maxScale = rbool ? m.maxScale : f.maxScale;
            female = rbool;
            scale = Mice.MiceScale(age+0.1f, this.female ? Mice.mice.pubertyf : Mice.mice.pubertym, maxScale);
            Color fc = f.body.body.material.color;
            Color mc = m.body.body.material.color;
            float ccr = rbool ? fc.r : mc.r;
            float ccg = rbool ? fc.g : mc.g;
            float ccb = rbool ? fc.b : mc.b;
                Color cc = new Color(ccr*Mice.rmod, ccg*Mice.rmod, ccb*Mice.rmod, 255);
            redShift = ccr/255f;
            greenShift = ccg/255f;
            blueShift = ccb/255f;
            body.SetupBody(scale, cc, index);
            speed = (rbool ? m.speed : f.speed)*(1+0.5f*redShift);
            relationshipsView = new List<int>();
            motiveMod = rbool ? m.motiveMod : f.motiveMod;
            _relationships = new float[200];
            protagRelationship = Mice.rmod*Mice.mice.baseProtagAffection;
            for(int i = 0; i < relationships.Length; ++i){
                relationships[i] = Mice.rmod*Mice.mice.baseMouseAffection;
            }
            relationships[father] = 6f;
            relationships[mother] = 6f;
            protagRelationship = Mice.rmod*Mice.mice.baseProtagAffection;
            affectionRate = rbool ? f.affectionRate : m.affectionRate;
            goalPoint = MiceZone.zone.GetWanderPoint();
            motives[(int)MSTIMULUS.HUNGER] = new AppHunger((int)MSTIMULUS.HUNGER,0.5f, 0.3f, 0.7f, Mice.mice.hungerThreshold+2*greenShift, 10);
            motives[(int)MSTIMULUS.ENERGY] = new AppEnergy((int)MSTIMULUS.ENERGY,1f, 0.1f, 0.9f, Mice.mice.energyLevelG+(greenShift + redShift),15);
            motives[(int)MSTIMULUS.SOCIAL] = new AppSocial((int)MSTIMULUS.SOCIAL,0.5f, 0.1f, 0.9f, Mice.mice.energyLevelG+2*blueShift,8);
            motives[(int)MSTIMULUS.SEXDRIVE] = new AppSex((int)MSTIMULUS.SEXDRIVE,0.5f, 0.1f, 0.9f, Mice.mice.energyLevelG+2*redShift, 9);
            motives[(int)MSTIMULUS.BATHROOM] = new AppBathroom((int)MSTIMULUS.BATHROOM,1, 0.3f, 1, 1, 11);
            motives[(int)MSTIMULUS.BOREDOM] = new AppBoredom((int)MSTIMULUS.BOREDOM,0.5f, 0.1f, 0.9f, Mice.mice.energyLevelG+2*redShift, 8);
            motives[(int)MSTIMULUS.CALL] = new AppCall((int)MSTIMULUS.CALL,1, 1, 1, 1, 100);
            body.emo.SetNotVisible();
        }
        public Mouse(int index,MicePreset.MousePreset preset){
            this.index = index;
            GameObject q = Instantiate(Mice.mice.bodyPrefab);
            body = q.GetComponent<MouseBody>();
            motiveMod = preset.motiveMods;
            motives = new Appetite[7];
            body.tform.parent = mice.transform;
            scale = Mice.MiceScale(preset.age, preset.maxAge,preset.maxScale);
            maxScale = preset.maxScale;
            age = preset.age;
            rstep = Mice.rstep;
            rtime = 0;
            maxAge = preset.maxAge;
            speed = (preset.energyLevel/Mice.mice.energyLevelG)*Mice.mice.speedG;
            affectionRate = preset.affectionRate;
            relationshipsView = new List<int>();
            female=preset.female;
            _relationships = new float[200];
            protagRelationship = Mice.rmod*Mice.mice.baseProtagAffection;
            scale = Mice.MiceScale(age+0.1f, this.female ? Mice.mice.pubertyf : Mice.mice.pubertym, maxScale);
            for(int i = 0; i < relationships.Length; ++i){
                relationships[i] = Mice.rmod*Mice.mice.baseMouseAffection;
            } 
            redShift = (preset.color.r/255f)-0.5f;
            greenShift = (preset.color.g/255f)-0.5f;
            blueShift = (preset.color.b/255f)-0.5f;
            body.SetupBody(scale, preset.color, index);
            goalPoint = MiceZone.zone.GetWanderPoint();
            motives[(int)MSTIMULUS.HUNGER] = new AppHunger((int)MSTIMULUS.HUNGER,0.5f, 0.3f, 0.7f, Mice.mice.hungerThreshold+2*greenShift, 10);
            motives[(int)MSTIMULUS.ENERGY] = new AppEnergy((int)MSTIMULUS.ENERGY,1f, 0.1f, 0.9f, Mice.mice.energyLevelG+(greenShift + redShift),15);
            motives[(int)MSTIMULUS.SOCIAL] = new AppSocial((int)MSTIMULUS.SOCIAL,1f, 0.1f, 0.9f, Mice.mice.energyLevelG+2*blueShift,8);
            motives[(int)MSTIMULUS.SEXDRIVE] = new AppSex((int)MSTIMULUS.SEXDRIVE,0.5f, 0.1f, 0.9f, Mice.mice.energyLevelG+2*redShift, 9);
            motives[(int)MSTIMULUS.BATHROOM] = new AppBathroom((int)MSTIMULUS.BATHROOM,1, 0.3f, 1, 1, 11);
            motives[(int)MSTIMULUS.BOREDOM] = new AppBoredom((int)MSTIMULUS.BOREDOM,0.5f, 0.1f, 0.9f, Mice.mice.energyLevelG+2*redShift, 8);
            motives[(int)MSTIMULUS.CALL] = new AppCall((int)MSTIMULUS.CALL,1, 0.5f, 0.75f, 1, 100);
            body.emo.SetNotVisible();
        }
        
        public void UpdateMouse(){
            rtime+=Time.deltaTime;
            float perHour = (1/DayCycle.hour)*Time.deltaTime;
            if(rtime > rstep){
                rstep = Mice.rstep;
                rtime=0;
                rStep();
            }
            timeInState+=Time.deltaTime;
            timeInMotive+=Time.deltaTime;
            SpecialUpdate(perHour);
            StateUpdate(perHour);
            MotiveUpdate(perHour);
        }
        void SpecialUpdate(float perHour){
            dayAging += Time.deltaTime/perHour;
        }
        void StateUpdate(float perHour){
            switch(state){
                case MSTATE.MOVE:
                    Vector3 velocity = goalPoint - body.tform.position;
                    velocity = velocity.normalized;
                    body.tform.LookAt(body.tform.position + velocity);
                    dayExercise += Time.deltaTime;
                    if(Vector3.Distance(body.tform.position, goalPoint)<scale*1.5f){
                        velocity = Vector3.zero;
                        if(goalMousePoint){
                            switch(goalMousePoint.Availability(index)){
                                case MPRESPONSE.AVAILABLE:
                                    goalMousePoint.Engage(this);
                                break;
                                case MPRESPONSE.FULL:
                                    foreach(int m in goalMousePoint.occupants)
                                    {
                                        IncreaseAffection(m,-0.2f);
                                    }
                                    ChangeState(MSTATE.WAIT);
                                break;
                                case MPRESPONSE.REJECTED:
                                    MouseBody mb = goalMousePoint.GetComponent<MouseBody>();
                                    if(mb){
                                        int i = mb.index;
                                        IncreaseAffection(i,-0.2f);
                                    }
                                    ChangeState(MSTATE.WAIT);
                                break;
                            }
                        }
                        else{
                            ChangeState(MSTATE.WAIT);
                        }
                    }
                    body.rbody.velocity = velocity*speed;
                break;
                case MSTATE.HELD:
                    body.rbody.velocity = Vector3.zero;
                    goalMousePoint=null;
                break;
                case MSTATE.WAIT:
                break;
                case MSTATE.USEMP:
                    if(!callbackMousePoint){
                        ChangeState(MSTATE.WAIT);
                    }
                    else{
                        callbackMousePoint.Access(this);
                        
                        if(motives[(int)callbackMousePoint.type].isFull()){
                            print("index " + index + " hunger full");
                            DisengageMousePoint(false);
                        
                        }
                    }
                break;
            }
        }

        public void DisengageSleep(bool bed){
            print("disengage sleep: ");
        }

        public void DisengageMousePoint(bool hard){
            if(!hard)
                callbackMousePoint.Disengage(this);
            callbackMousePoint=null;
            ChangeState(MSTATE.WAIT);
        }
        public void SetHeld(bool b){
            ChangeState(b ? MSTATE.HELD : MSTATE.WAIT);
        }

        public bool DryHump(int i){
            return motives[(int)MSTIMULUS.SEXDRIVE].isFull()  && relationships[i] > 0.69;
        }


        public void Pet(){
            protagRelationship +=0.01f;
        }

        public void SetMousePointCallback(MousePoint mousePoint){
            callbackMousePoint=mousePoint;
            ChangeState(MSTATE.USEMP);
        }

        List<int> orderMotives(){
            List<Appetite> e1 = new List<Appetite>();
            foreach(Appetite a in motives){
                e1.Add(a);
            }
            return orderMotivesStep(new List<int>(), e1);
        }

        List<int> orderMotivesStep(List<int> running, List<Appetite> total){
            if(total.Count<=0)
                return running;
            Appetite highest = null;
            float maxPriority = float.MinValue;
            foreach(Appetite a in total){
                if(a.priority > maxPriority){
                    highest = a;
                    maxPriority = a.priority;
                }
            }
            running.Add(highest.key);
            total.Remove(highest);
            return orderMotivesStep(running, total);
        }

        public void IncreaseAffection(int indx, float multiplier){
            if(indx<0)
                protagRelationship+=affectionRate*multiplier*Time.deltaTime;
            else
                relationships[indx]+=affectionRate*multiplier*Time.deltaTime;
        }

        
        void MotiveUpdate(float perHour){
            MotivesRatio ratio = Mice.mice.waitMotives;
            switch(state){
                case MSTATE.USEMP:
                    ratio = callbackMousePoint.motivesRatio;
                break;
                case MSTATE.MOVE:
                    ratio = Mice.mice.moveMotives;
                break;
            }
            motives[(int)MSTIMULUS.HUNGER].Update(ratio.fullness*motiveMod.fullness, perHour);
            if(ratio.fullness > 0){
                dayFeeding += ratio.fullness*perHour;
                motives[(int)MSTIMULUS.BATHROOM].Update(-1,perHour);
            }
            motives[(int)MSTIMULUS.ENERGY].Update(ratio.physicalEnergy*motiveMod.physicalEnergy,perHour);
            motives[(int)MSTIMULUS.SOCIAL].Update(ratio.socialEnergy*motiveMod.socialEnergy, perHour);
            motives[(int)MSTIMULUS.SEXDRIVE].Update(ratio.sexEnergy*motiveMod.sexEnergy, perHour);
            motives[(int)MSTIMULUS.BATHROOM].Update(ratio.bathroom*motiveMod.bathroom, perHour);
            motives[(int)MSTIMULUS.BOREDOM].Update(ratio.boredom*motiveMod.boredom, perHour);
            MSTIMULUS smotive = MSTIMULUS.NONE;
            float maxPriority = float.NegativeInfinity;
            for(int i = 0; i < motives.Length; ++i){
                if(motives[i].isLow() && motives[i].priority > maxPriority)
                    {
                        smotive = (MSTIMULUS)i;
                        maxPriority = motives[i].priority;
                    }
            }
            if(state!= MSTATE.USEMP){
            ChangeMotive(smotive);}
        } 
        bool marked = false;
        public void Mark (){
            if(!marked && mice.marked.Count < DayCycle.sundayCount){
                mice.marked.Add(index);
                if(mice.marked.Count == DayCycle.sundayCount)
                    ShadeController.shade.SetVisible(true);
            
            }
        }


        public void UseItemOn(int item){
            
        }
        public int partner = -1;
        public MouseMousePoint socializeCallback = null;
        public bool drySocialize(int i){
            return partner==-1 && GetRelationship(i) >= 5 && !motives[(int)MSTIMULUS.SOCIAL].isLow();
        }


        public MouseMousePoint GetMMP(){
            return body.GetComponent<MouseMousePoint>();
        }

        public void SetSocial(int mouse){
            if(!relationshipsView.Contains(mouse)){
                relationshipsView.Add(mouse);
            }
            partner=mouse;
        }

        public float GetRelationship(int i){
            return relationships[i];
        }

        void ChangeState(MSTATE newState){
            if(state!=newState){
            timeInState=0;
            state=newState;
            }
        }

        void ChangeMotive(MSTIMULUS newMotive){
            if(motive != newMotive){
                timeInMotive=0;
                motive = newMotive;
                if((int)newMotive > -1){
                    body.emo.SetSprite((int)newMotive);
                    body.emo.SetVisibleAndFade();
                }
                goalMousePoint = null;
            }
        }

        void rStep(){
            if(state==MSTATE.WAIT){
                if(timeInState > Mice.rmod*2.5f){
                    ChangeState(MSTATE.MOVE);
                    goalMousePoint = getNearestPoint();
                    if(goalMousePoint)
                        goalPoint = goalMousePoint.tform.position;
                    else
                        goalPoint = MiceZone.zone.GetWanderPoint();
                }
            }


        }
        MSTIMULUS getBoredBehaviour(){
            return MSTIMULUS.HUNGER;
        }

        MousePoint getNearestPoint(){
            float mindist=float.MaxValue;
            MSTIMULUS localGoal = (motive == MSTIMULUS.NONE ? getBoredBehaviour() : motive);
            if(localGoal == MSTIMULUS.NONE)
                return null;
            int best = -1;
            if(MousePoint.points!=null && MousePoint.points.Count>0){
                if(localGoal==MSTIMULUS.SOCIAL){
                    for(int i = 0; i < MouseMousePoint.socialPoints.Count; ++i){
                        MouseMousePoint mmpi = MouseMousePoint.socialPoints[i];
                        if(mmpi.Availability(index)==MPRESPONSE.AVAILABLE){
                            float dist = Vector3.Distance(body.tform.position, mmpi.tform.position);
                            float sdist = mmpi.body.index >= 0 ? 100f*((10f-relationships[mmpi.body.index])/10f) :
                                                                100f*((10f-protagRelationship)/10f);
                            float totalSdist = Mathf.Sqrt(dist*dist + sdist*sdist);
                            if(totalSdist < mindist)
                            {
                                best = i;
                                mindist = totalSdist;
                            }
                        }
                    }
                    return best > -1 ? MouseMousePoint.socialPoints[best] : null;
                }
                else{
                    for(int i = 0; i < MousePoint.points.Count; ++i){
                        if(MousePoint.points[i].type==localGoal && MousePoint.points[i].Availability(index)==MPRESPONSE.AVAILABLE
                            && Mathf.Abs(body.tform.position.y-MousePoint.points[i].tform.position.y)<2){
                            float dist = Vector3.Distance(body.tform.position, MousePoint.points[i].tform.position);
                            if(dist < mindist){
                                best = i;
                                mindist = dist;
                            }
                        }
                    }

                    if(best==-1){
                        if(localGoal==MSTIMULUS.ENERGY){
                            return Mice.CreateAdhocBedPoint(index, MiceZone.zone.GetWanderPoint());
                        }
                        if(localGoal==MSTIMULUS.BATHROOM){
                            return Mice.CreateAdhocBathroomPoint(index, MiceZone.zone.GetWanderPoint());
                        }
                    }
                    return best > -1 ? MousePoint.points[best] : null;
                }
            }
            return null;
        }
    } 


    abstract class Appetite{
        public static float criticalityMultiplier = 5;
        protected int _key;
        public int key {get{return _key;}}
        protected float _value;
        public float value {get{return _value;}}
        protected float _loAppetite, _hiAppetite;
        public float loAppetite {get{return _loAppetite;}}
        public float hiAppetite {get{return _hiAppetite;}}
        protected float _priority;
        public float priority {get{return _priority;}}
        protected float _max;
        public float max {get{return _max;}}

        public Appetite(){
            _key=0;
            _value=0;
            _loAppetite=0;
            _hiAppetite=0;
            _priority=0;
            _max=0;
        }

        public Appetite(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key=key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }

        public void Update(float rate, float time){
            _value += rate*time;
            _value = Mathf.Min(value,max);
        }

        public bool isLow(){
            return value <=loAppetite;

        }

        public bool isFull(){
            return value >=hiAppetite;
        }

        public bool halfFull(){
            return value >= 0.5f*max;
        }

        public float Criticality(){
            return criticalityMultiplier*((loAppetite-value)/loAppetite);
        }

        public bool isPositive(){
            return value > 0;
        }

        public bool isCritical(){
            return value/loAppetite <=0.33f;
        }

        public abstract MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal);
    }

    class AppHunger : Appetite {
        public AppHunger(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key = key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }
        public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
        {
            if(MousePoint.points!=null && MousePoint.points.Count>0){
                int best = -1;
                float mindist = float.MaxValue;
                for(int i = 0; i < MousePoint.points.Count; ++i){
                    if(MousePoint.points[i].type==localGoal && MousePoint.points[i].Availability(m.index)==MPRESPONSE.AVAILABLE
                        && Mathf.Abs(m.body.tform.position.y-MousePoint.points[i].tform.position.y)<2){
                        float dist = Vector3.Distance(m.body.tform.position, MousePoint.points[i].tform.position);
                        if(dist < mindist){
                            best = i;
                            mindist = dist;
                        }
                    }
                }
                return best > -1 ? MousePoint.points[best] : null;
            }
            return null;
        }
        
    }
    class AppEnergy : Appetite {
        public AppEnergy(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key=key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }
        public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
        {
            if(MousePoint.points!=null && MousePoint.points.Count>0){
                int best = -1;
                float mindist = float.MaxValue;
                for(int i = 0; i < MousePoint.points.Count; ++i){
                    if(MousePoint.points[i].type==localGoal && MousePoint.points[i].Availability(m.index)==MPRESPONSE.AVAILABLE
                        && Mathf.Abs(m.body.tform.position.y-MousePoint.points[i].tform.position.y)<2){
                        float dist = Vector3.Distance(m.body.tform.position, MousePoint.points[i].tform.position);
                        if(dist < mindist){
                            best = i;
                            mindist = dist;
                        }
                    }
                }
                return best > -1 ? MousePoint.points[best] : Mice.CreateAdhocBedPoint(m.index, MiceZone.zone.GetWanderPoint());
            }
            return Mice.CreateAdhocBedPoint(m.index, MiceZone.zone.GetWanderPoint());
        }
    }
    class AppSocial : Appetite {
    public AppSocial(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
        _key=key;
        _value = valuepct*max;
        _loAppetite = loAppetitepct*max;
        _hiAppetite = hiAppetitepct*max;
        _priority = priority;
        _max = max;
    }
    public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
    {
        if(MousePoint.points!=null && MousePoint.points.Count>0){
                float mindist = float.MaxValue;
                int best = -1;
                for(int i = 0; i < MouseMousePoint.socialPoints.Count; ++i){
                    MouseMousePoint mmpi = MouseMousePoint.socialPoints[i];
                if(mmpi.Availability(m.index)==MPRESPONSE.AVAILABLE){
                    float dist = Vector3.Distance(m.body.tform.position, mmpi.tform.position);
                    float sdist = mmpi.body.index >= 0 ? 100f*((10f-m.relationships[mmpi.body.index])/10f) :
                                                                100f*((10f-m.protagRelationship)/10f);
                    float totalSdist = Mathf.Sqrt(dist*dist + sdist*sdist);
                    if(totalSdist < mindist)
                    {
                        best = i;
                        mindist = totalSdist;
                    }
                }
            }
            return best > -1 ? MouseMousePoint.socialPoints[best] : null;
        }
        return null;
    }
}
    class AppSex : Appetite {
        public AppSex(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key = key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }
        public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
        {
            return null;
        }
    }
    class AppBathroom : Appetite {
        public AppBathroom(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key = key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }
        public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
        {
            return null;
        }
    }
    class AppBoredom : Appetite {
        public AppBoredom(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key = key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }
        public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
        {
            return null;
        }
    }
    class AppCall : Appetite {
        public AppCall(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key = key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
        }
        public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
        {
            if(MousePoint.points!=null && MousePoint.points.Count>0){
                int best = -1;
                float mindist = float.MaxValue;
                for(int i = 0; i < MousePoint.points.Count; ++i){
                    if(MousePoint.points[i].type==localGoal && MousePoint.points[i].Availability(m.index)==MPRESPONSE.AVAILABLE
                        && Mathf.Abs(m.body.tform.position.y-MousePoint.points[i].tform.position.y)<2){
                        float dist = Vector3.Distance(m.body.tform.position, MousePoint.points[i].tform.position);
                        if(dist < mindist){
                            best = i;
                            mindist = dist;
                        }
                    }
                }
                return best > -1 ? MousePoint.points[best] : null;
            }
            return null;
        }
    }
    [System.Serializable]
    public class Emote {
        public Sprite sprite;
        public AudioClip audioClip;
    }
}