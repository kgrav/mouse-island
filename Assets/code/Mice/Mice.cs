using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MSTATE {MOVE, WAIT, HELD, USEMP}
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

    public static AdhocBedPoint CreateAdhocMousePoint(int i, Vector3 point, MSTIMULUS type){
        AdhocBedPoint q = Instantiate(Mice.mice.adhocBedPointPrefab, point, Quaternion.identity).GetComponent<AdhocBedPoint>();
        q.SetOwner(i);
        return q;
    }

    public static AdhocBathroomPoint CreateAdhocBathroomPoint(int i, Vector3 point){
        AdhocBathroomPoint q = Instantiate(Mice.mice.adhocBathroomPointPrefab, point, Quaternion.identity).GetComponent<AdhocBathroomPoint>();
        q.SetOwner(i);
        return q;
    }

    public float globalAffectionRate;



    public static void NewDay(){
        if(DayCycle.dnc.sunday)
            mice.marked = new List<int>();
        foreach(Baby b in mice.babyQueue){
            mice.activeMice.Add(new Mouse(mice.activeMice.Count, b.father, b.mother));
        }
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
    List<Baby> babyQueue;
    public Vector2 rstepRange;
    public void CreateBaby(int f, int m){
        if(babyQueue==null)
            babyQueue=new List<Baby>();
        babyQueue.Add(new Baby(f,m));
        print("created baby. father: " + f + ", mother: " + m);
    }

    struct Baby {
        public int father, mother;

        public Baby(int father, int mother){
            this.father=father;
            this.mother=mother;
        }
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
        babyQueue = new List<Baby>();
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
        public float redShift;
        public float blueShift;
        public float greenShift;
        bool _alive = true;
        int socialPartner = -1;
        public bool alive {get{return _alive;}}
        MousePoint goalMousePoint, callbackMousePoint;
        Vector3 goalPoint;
        float timeInState=0, timeInMotive=0;
        public float protagRelationship;
        bool rbool {get{return UnityEngine.Random.Range(0,1) > 0.5f;}}
        bool pctChance(float pct){return UnityEngine.Random.Range(0f,1f) < pct;}
        List<int> relationshipsView, motivesView;
        Appetite[] motives;
        MotivesRatio motiveMod;

        public Vector3 affinity;
        int hiAffinity=-1, penHiAffinity=-1;

        public Vector3 position {get {return body.tform.position;}}
        public Vector3 velocity {get {return body.rbody.velocity;}}

        int father=-1,mother=-1,_currMate=-1;

        public int currMate {get{return _currMate;}}

        float dayExercise;
        float dayFeeding;
        float dayAging;


        public string StatsString() {
                                            string r = "index: " + index + "\n"
                                            + "affinity: R: " + redShift + " G: " + greenShift + " B: " + blueShift + "\n"
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
                                            r += "} \n"; 
                                            r+= "motives: ";
                                            for(int i = 0; i < motivesView.Count; ++i){
                                                r += i + ": " + (MSTIMULUS)motivesView[i] + "; ";
                                            }
                                            r+= "} \n";
                                            r += goalMousePoint + "\n";
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
            redShift = ccr/255f-0.5f;
            greenShift = ccg/255f-0.5f;
            blueShift = ccb/255f-0.5f;
            affinity = new Vector3(cc.r, cc.g, cc.b);
            hiAffinity = (int)Mathf.Max(affinity[0], Mathf.Max(affinity[1], affinity[2]));
            float hi = float.MinValue;
            for(int i = 0; i < 3; ++i){
                if(affinity[i] > hi && i != hiAffinity){
                    penHiAffinity = i;
                    hi = affinity[i];
                }
            }
            body.SetupBody(scale, cc, index);
            motivesView = new List<int>();
            speed = (rbool ? m.speed : f.speed)*(1+0.5f*redShift);
            relationshipsView = new List<int>();
            motiveMod = rbool ? m.motiveMod : f.motiveMod;
            _relationships = new float[2000];
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
            motives[(int)MSTIMULUS.BATHROOM] = new AppBathroom((int)MSTIMULUS.BATHROOM,0.9f, 0.3f, 1, 1, 11);
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
            relationshipsView = new List<int>();
            female=preset.female;
            _relationships = new float[200];
            motivesView = new List<int>();
            protagRelationship = Mice.rmod*Mice.mice.baseProtagAffection;
            scale = Mice.MiceScale(age+0.1f, this.female ? Mice.mice.pubertyf : Mice.mice.pubertym, maxScale);
            for(int i = 0; i < relationships.Length; ++i){
                relationships[i] = Mice.rmod*Mice.mice.baseMouseAffection;
            } 
            redShift = (preset.color.r/255f)-0.5f;
            greenShift = (preset.color.g/255f)-0.5f;
            blueShift = (preset.color.b/255f)-0.5f;
            affectionRate = Mice.mice.globalAffectionRate - Mice.mice.globalAffectionRate*blueShift;
            affinity = new Vector3(preset.color.r, preset.color.g, preset.color.b);
            hiAffinity = (int)Mathf.Max(affinity[0], Mathf.Max(affinity[1], affinity[2]));
            float hi = float.MinValue;
            for(int i = 0; i < 3; ++i){
                if(affinity[i] > hi && i != hiAffinity){
                    penHiAffinity = i;
                    hi = affinity[i];
                }
            }
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
        Vector3 BoidAdjust(Vector3 velocity){
            List<Mouse> ms = Mice.mice.activeMice;
            Vector3 c1 = Vector3.zero;
            Vector3 c2 = Vector3.zero;
            Vector3 c3 = Vector3.zero;
            int c3s = 0;
            for(int i = 0; i < ms.Count; ++i){
                if(ms[i].index != index){
                    c1 += ms[i].position;
                    if(Vector3.Distance(position, ms[i].position) < 10)
                        c2 = c2 - (position - ms[i].position);
                    if(ms[i].velocity.magnitude > 0)
                    {
                        c3 += ms[i].velocity;
                        c3s++;
                    }
                }
                
            }
            c1 = c1/(ms.Count - 1);
            c3 = (c3/c3s)/8;
            return c1 + c2 + c3;
        }
        void StateUpdate(float perHour){
            switch(state){
                case MSTATE.MOVE:
                    Vector3 velocity = goalPoint - body.tform.position;
                    velocity = velocity.normalized*speed;
                    //velocity += BoidAdjust(velocity);
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
                            print("no mouse point, change to wait");
                            ChangeState(MSTATE.WAIT);
                        }
                    }
                    body.rbody.velocity = velocity;
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
                        
                        if(motives[(int)motive].isFull()){
                            DisengageMousePoint(false);
                            ChangeState(MSTATE.WAIT);
                        }
                    }
                break;
            }
        }

        public void EngageToyMP(ToyMousePoint tmp){
            motives[(int)MSTIMULUS.BOREDOM].Update(tmp.motivesRatio.boredom.value*(1-(Vector3.Distance(tmp.vectorAffinity, affinity)/255)), 1);
            foreach(int i in tmp.pullAuditTrail){
                if(i!=index)
                    IncreaseAffection(i, 0.2f);
            }
            state = MSTATE.WAIT;
        }

        public void SexUpdate(int partner){
            bool rule1 = pctChance(75*(redShift+0.5f));
            bool rule2 = DryHump(partner);
            bool rule3 = Mice.GetMouse(partner).DryHump(index);
            if(rule1 && rule2 && rule3){
                InitSex(partner);
                Mice.GetMouse(partner).InitSex(index);
            }
        }

        public void InitSex(int partner){
            _currMate = partner;
            ChangeMotive(MSTIMULUS.SEXDRIVE);
            goalMousePoint = getSexPoint();
            print("sex point: " + goalMousePoint);
            if(goalMousePoint!=null){
                ChangeState(MSTATE.MOVE);
            }
        }

        public void DisengageSexPoint(){
            _currMate = -1;
            motives[(int)MSTIMULUS.SEXDRIVE].SetValuePct(1);
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
            return currMate == -1 && motives[(int)MSTIMULUS.SEXDRIVE].isLow()  && (relationships[i] > 0.69);
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
                if(a.isLow()){
                    e1.Add(a);
                }
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
            if(!relationshipsView.Contains(indx)){
                relationshipsView.Add(indx);
            }
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
 
            for(int i = 0; i < motives.Length; ++i){
                float mul = ratio.GetMotive((MSTIMULUS)i).use ? ratio.GetMotive((MSTIMULUS)i).value : 1;
                float mmmul = motiveMod.GetMotive((MSTIMULUS)(i)).use ? motiveMod.GetMotive((MSTIMULUS)i).value : 1;
                motives[i].Update(mul*mmmul, perHour);
                if(motives[i].isLow()){
                    motives[i].emoteTimer -= Time.deltaTime;
                    if(motives[i].emoteTimer <=0){
                        body.emo.SetVisibleAndFade();
                        motives[i].emoteTimer = UnityEngine.Random.Range(60f,90f);
                    }
                }
                else{
                    motives[i].emoteTimer=0;
                }
            }
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
            bool rule1 = GetRelationship(i)>=.35f;
            bool rule2 = motives[(int)MSTIMULUS.SOCIAL].isLow();
            //print(i + " slides up to " + index + ". rule 1: " + rule1 + " rule2: " + rule2);
            return GetRelationship(i) >= .35f && motives[(int)MSTIMULUS.SOCIAL].isLow();
        }


        public MouseMousePoint GetMMP(){
            return body.GetComponent<MouseMousePoint>();
        }

        public void SetSocial(int mouse, MouseMousePoint mmp){
            if(!relationshipsView.Contains(mouse)){
                relationshipsView.Add(mouse);
            }
            partner = mouse;
            callbackMousePoint = mmp;
            ChangeState(MSTATE.USEMP);
        }

        public float GetRelationship(int i){
            return i == -1 ? protagRelationship : relationships[i];
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
                goalMousePoint=null;
            }
        }

        void rStep(){
            if(state==MSTATE.WAIT){
                if(timeInState > Mice.rmod*2.5f){
                    ChangeState(MSTATE.MOVE);
                    motivesView = orderMotives();
                    MSTIMULUS mptype;
                    MousePoint newGoal = getNearestPoint(motivesView, out mptype);
                    if(newGoal) {
                            ChangeMotive(mptype);
                            goalMousePoint = newGoal;
                            goalPoint = goalMousePoint.tform.position;
                        }
                    else
                        goalPoint = MiceZone.zone.GetWanderPoint();
                }
            }
            else if (state==MSTATE.USEMP){
                motives[(int)motive].rstep(this,callbackMousePoint);
            }


        }
        MSTIMULUS getBoredBehaviour(){
            return MSTIMULUS.HUNGER;
        }

        MousePoint getNearestPoint(List<int> orderedMotives, out MSTIMULUS stim){
            stim = MSTIMULUS.NONE;
            foreach(int i in orderedMotives){
                if(i==(int)MSTIMULUS.SEXDRIVE)
                    continue;
                List<MousePoint> mps = MousePoint.pointsByType[(MSTIMULUS)i];
                
                int best=-1;
                float ratingThreshold = motives[i].Criticality();
                float minRating=float.MaxValue;
                for(int j = 0; j < mps.Count; ++j)
                {
                    if(mps[j].Availability(index)==MPRESPONSE.AVAILABLE && mps[j].motivesRatio.GetMotive((MSTIMULUS)i).rating < ratingThreshold){
                        float overallRating = motives[i].RateMousePoint(this, (MSTIMULUS)i, mps[j]);
                        if(overallRating < minRating){
                            best = j;
                            minRating = overallRating;
                        }
                    }
                }
                if(best != -1)
                {
                    stim = (MSTIMULUS)i;    
                    return mps[best];
                }
                else if(motives[i].adhoc){
                    stim = (MSTIMULUS)i;
                    return Mice.CreateAdhocMousePoint(index, MiceZone.zone.GetWanderPoint(), stim);
                }
            }
            return null;
        }

        MousePoint getSexPoint(){
            return motives[(int)MSTIMULUS.SEXDRIVE].GetNearestMousePoint(this, MSTIMULUS.SEXDRIVE);
        }

        MousePoint getNearestPoint(List<int> orderedMotives){
            foreach(int i in orderedMotives){
                if(i==(int)MSTIMULUS.SEXDRIVE)
                    continue;
                MousePoint q = motives[i].GetNearestMousePoint(this,(MSTIMULUS)i);
                if(q)
                {  
                    return q;
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

        public float emoteTimer=0;

        protected bool _adhoc;
        public bool adhoc { get{return _adhoc;}}

        public Appetite(){
            _key=0;
            _value=0;
            _loAppetite=0;
            _hiAppetite=0;
            _priority=0;
            _max=0;
            _adhoc=false;
        }

        public Appetite(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority, bool adhoc){
            _key=key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
            _adhoc = adhoc;
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
            return (loAppetite-value)/loAppetite;
        }

        public bool isPositive(){
            return value > 0;
        }

        public void SetValuePct(float pct){
            _value = pct*max;
        }

        public bool isCritical(){
            return value/loAppetite <=0.33f;
        }

        public virtual float RateMousePoint(Mouse mouse, MSTIMULUS stim, MousePoint mousePoint){
            return float.MaxValue;
        }

        public virtual MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal){
            
            return null;
        }

        public virtual void rstep(Mouse m, MousePoint mp){

        }
    }

    class AppHunger : Appetite {
        public AppHunger(int key, float valuepct, float loAppetitepct, float hiAppetitepct, float max, float priority){
            _key = key;
            _value = valuepct*max;
            _loAppetite = loAppetitepct*max;
            _hiAppetite = hiAppetitepct*max;
            _priority = priority;
            _max = max;
            _adhoc = false;
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
            _adhoc=true;
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
        _adhoc=false;
    }
    public override MousePoint GetNearestMousePoint(Mouse m, MSTIMULUS localGoal)
    {
        if(MousePoint.points!=null && MousePoint.points.Count>0){
                float mindist = float.MaxValue;
                int best = -1;
                for(int i = 0; i < MouseMousePoint.socialPoints.Count; ++i){
                    MouseMousePoint mmpi = MouseMousePoint.socialPoints[i];
                if(mmpi.Availability(m.index)==MPRESPONSE.AVAILABLE && mmpi.body.index != m.index){
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
            _adhoc=false;
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
            _adhoc=true;
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
                if(best > -1){return MousePoint.points[best];}
            }
            return Mice.CreateAdhocBathroomPoint(m.index, MiceZone.zone.GetWanderPoint());
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
            _adhoc=false;
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
            _adhoc=false;
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

class Relationships {
    public float protagRelationship;
    public float[] mouseRelationships;
    public float[] itemRelationships;
}

    [System.Serializable]
    public class Emote {
        public Sprite sprite;
        public AudioClip audioClip;
    }
}