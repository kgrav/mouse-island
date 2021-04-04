using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MPRESPONSE{FULL, INACTIVE, AVAILABLE, REJECTED, RSVP}

public abstract class MousePoint : NVComponent {
    

    
    public static List<MousePoint> points=null;
    public static Dictionary<MSTIMULUS, List<MousePoint>> pointsByType;

    int auditTrailLength=4;
    public float auditTrailRefreshRate;
    protected HistoryQueue<int> auditTrail;
    public List<int> pullAuditTrail {get{return auditTrail.getQueue;}}
    public MotivesRatio motivesRatio;
    protected List<int> _occupants;
    public List<int> occupants {get{return _occupants;}}
    public int maxCapacity;
    int mpindex;
    int mbcalls = 0;
    MouseBody _body;
    public MouseBody body {get{if(!_body && mbcalls <1)_body=GetComponent<MouseBody>();mbcalls++;return _body;}}

    public ShipTag shipTag;
    protected int[] lastIndx;
    protected bool _active;

    public bool startActive;
    public bool active {get{return _active;}}
    public void Activate(){
        _active=true;
    }
    public void Deactivate(){
        _active=false;
    }
    void Awake(){
        if(points == null){
            points = new List<MousePoint>();
            pointsByType = new Dictionary<MSTIMULUS, List<MousePoint>>();
        }
        _occupants = new List<int>();
        mpindex = points.Count;
        auditTrail = new HistoryQueue<int>(auditTrailLength);
        points.Add(this);
        foreach(MSTIMULUS m in MPUtil.stimList){
            if(motivesRatio.GetMotive(m).value > 0 && motivesRatio.GetMotive(m).use){
                if(!pointsByType.ContainsKey(m)){
                    pointsByType.Add(m, new List<MousePoint>());
                }
                pointsByType[m].Add(this);
            }
        }
        if(startActive)
            _active=true;
        Init();
    }

    protected override void NVUpdate()
    {
        ChUpdate();
        auditTrail.UpdateTime(Time.deltaTime);
    }

    protected virtual void ChUpdate(){}
    protected virtual void Init(){
    }
    public virtual MPRESPONSE Availability(int mouseIndex){
        if(!_active)
            return MPRESPONSE.INACTIVE;
        if(occupants.Count >= maxCapacity)
            return MPRESPONSE.FULL;
        return MPRESPONSE.AVAILABLE;
    }

    public void Engage(Mice.Mouse mouse){
        if(!occupants.Contains(mouse.index))
            occupants.Add(mouse.index);
        auditTrail.Log(mouse.index,auditTrailRefreshRate);
        mouse.SetMousePointCallback(this);
        OnEngage(mouse);
    }

    protected virtual void OnEngage(Mice.Mouse mouse){}
    
    public virtual void Access(Mice.Mouse mouse){}
    public void Disengage(Mice.Mouse mouse){
        if(occupants.Contains(mouse.index))
            occupants.Remove(mouse.index);
        OnDisengage(mouse);
    }

    protected virtual void OnDisengage(Mice.Mouse mouse){}

    public class MPType {
        public MSTIMULUS type;
        public int rating;
    }
}