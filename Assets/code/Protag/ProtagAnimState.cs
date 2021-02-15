using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtagAnimState : StateMachineBehaviour{
    protected ProtagAnimTalker pat;
    public string enter, exit;
    public updateAnim[] updateAnimations;
    protected int updateptr;   
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        updateptr = 0;
        lastloop=0;
        if(enter.Length > 0)
            pat.ReceiveMessage(enter);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        if(exit.Length > 0)
            pat.ReceiveMessage(exit);
    
    }
    protected float lastloop=0;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        float flrsi = Mathf.Floor(stateInfo.normalizedTime);
        if(flrsi > lastloop)
        {
            updateptr=0;
            lastloop = flrsi;
        }
        float time = stateInfo.normalizedTime - flrsi;
        if(updateptr < updateAnimations.Length){
            if(time >=updateAnimations[updateptr].time){
                if(updateAnimations[updateptr].msg.Length > 0)
                pat.ReceiveMessage(updateAnimations[updateptr].msg);
                updateptr+=1;
            }
        }
    }
    public void SetTalker(ProtagAnimTalker pat){
        this.pat = pat;
    }
    [System.Serializable]
    public class updateAnim{
        public string msg;
        public float time;
    } 
}