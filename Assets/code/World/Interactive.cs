using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BTYPE{BLOCK=-2,NONE=-1,PICKUP=0,DROP=1,CRAFT=2,STORAGE=3}
public enum XTYPE{BLOCK=-2,NONE=-1,PET=0,FEED=1,HEAL=2,PLACE=3, CHOP=4,REACH=5,STORE=6, MIDPRESS=7,LOPRESS=8, USE=9}
public enum YTYPE{BLOCK=-2,NONE=-1,CALL=0,MARK=1,SIT=2,STAND=3,EAT=4}
public abstract class Interactive : NVComponent {
    
    public string interactTrigger;
    
    public BTYPE bTYPE;

    public XTYPE xTYPE;
    public string Iname;
    public virtual string Description(){
        return Iname;
    }
    public virtual void UseItemOn(CONTROL button, Items.Item item){}
    public abstract void Trigger(ProtagController protag, CONTROL button);
    
}