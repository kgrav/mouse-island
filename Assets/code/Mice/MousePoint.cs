using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MPRESPONSE{FULL, INACTIVE, AVAILABLE, REJECTED}

public abstract class MousePoint : NVComponent {
    

    
    public static List<MousePoint> points=null;
    public static Dictionary<MSTIMULUS, List<int>> pointsByType;

    public int auditTrailLength;
    public float auditTrailRefreshRate;
    protected HistoryQueue<int> auditTrail;
    public MotivesRatio motivesRatio;
    protected List<int> _occupants;
    public List<int> occupants {get{return _occupants;}}
    public int maxCapacity;
    int mpindex;
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
            pointsByType = new Dictionary<MSTIMULUS, List<int>>();
        }
        _occupants = new List<int>();
        mpindex = points.Count;
        auditTrail = new HistoryQueue<int>(auditTrailLength);
        if(auditTrailRefreshRate > 0){
            Invoke("flushAuditTrail", auditTrailRefreshRate);
        }
        points.Add(this);
        if(!pointsByType.ContainsKey(type)){
            pointsByType[type] = new List<int>();
        }
        pointsByType[type].Add(mpindex);
        if(startActive)
            _active=true;
        Init();
    }

    void flushAuditTrail(){
        auditTrail.Clear();
        Invoke("flushAuditTrail", auditTrailRefreshRate);
    }
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
        auditTrail.Log(mouse.index);
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

    public MSTIMULUS type;
}