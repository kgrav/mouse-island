using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PROTAGSTATE {NORMAL, HOLD, ANIMATION, SIT}
public class ProtagController : NVComponent {
	public ILOC room;
	public string moveAnim, speedFloat,hiPickupTrigger,midPickupTrigger,
	loPickupTrigger,mousePickupTrigger,sitTrigger,standTrigger,reachTrigger,dropTrigger,
	petTrigger,feedTrigger,callTrigger,markTrigger,loInteractTrigger,midInteractTrigger;
	
	static ProtagController _ptg;
	public static ProtagController ptg {get{if(!_ptg) _ptg = FindObjectOfType<ProtagController>(); return _ptg;}}
	int stamina = 5;
	string xKey(XTYPE x){
		string r = "";
		switch(x){
			case XTYPE.PET:
				r= petTrigger;
			break;
			case XTYPE.HEAL:
			case XTYPE.FEED:
				r=feedTrigger;
			break;
			case XTYPE.PLACE:
				r=dropTrigger;
			break;
			case XTYPE.REACH:
				r=reachTrigger;
			break;
			case XTYPE.STORE:
			case XTYPE.USE:
			case XTYPE.MIDPRESS:
				r=midInteractTrigger;
			break;
			case XTYPE.LOPRESS:
				r=loInteractTrigger;
			break;
		}
		return r;
	}
	public int Stamina {get{return stamina;}}
	public string lsticku, lstickv, abtn, bbtn,xbtn,ybtn;
	public float interactSphere,acceleration,speedMax, dropOffset;
	float speed=0;
	int[] inventory = {-1,-1,-1,-1};
	int ins = -1;
	public AxisButton[] inventoryAccess; 
	// Use this for initialization
	Rigidbody _rbody;
	Rigidbody rbody{get{if(!_rbody)_rbody=GetComponent<Rigidbody>(); return _rbody;}}
	Animator _anim;
	Animator anim {get{if(!_anim)_anim=GetComponent<Animator>(); return _anim;}}
	public PROTAGSTATE state = PROTAGSTATE.NORMAL;
	public Transform attachBone;
	Interactive targetinteractive;
	void FacePosition(Vector3 pos){
		Vector3 pposa = tform.position;
		pposa.y = 0;
		pos.y = 0;
		Vector3 r = pposa-pos;
		tform.LookAt(tform.position + r);
	}
	public AudioClip invalid, footstep, gather, call;
	void LookForInteractive(){
		if(heldItem < 0){
			Collider[] c = Physics.OverlapSphere(tform.position, interactSphere, 11);
			float closest = float.MaxValue;
			Collider near = null;
			foreach(Collider e in c){
				if(e.GetComponent<Interactive>()){
				float dist = Vector3.Angle((tform.position - e.transform.position), SPCam.cam.tform.forward);
				if(dist < closest){

					closest=dist;
					near=e;
					
				}}
			}
			if(near){
				InteractTarget.itar.target = near.transform.position;
				targetinteractive = near.GetComponent<Interactive>();
				ActionDisplay.bButton.SetState((int)targetInteractive.bTYPE);
			}
			else{
				targetinteractive = null;
			}
		}
		else{
			targetinteractive=null;
		}

		InteractTarget.itar.SetActive(targetinteractive!=null);
	}

	int _heldItem = -1;
	Vector3 direction;
	public Interactive targetInteractive{get{return targetinteractive;}}
	XTYPE adx = XTYPE.NONE;
	BTYPE adb = BTYPE.NONE;
	// Update is called once per frame
	protected override void NVUpdate () {
		if(state == PROTAGSTATE.HOLD && _heldItem == -1)
			state = PROTAGSTATE.NORMAL;
		if(state == PROTAGSTATE.NORMAL && _heldItem > -1)
			state= PROTAGSTATE.HOLD;
		switch(state){
			case PROTAGSTATE.NORMAL:
				YTYPE ystate = room==ILOC.HOUSE || room==ILOC.VOID ? YTYPE.SIT : YTYPE.CALL;

				ActionDisplay.yButton.SetState((int)ystate);
				LookForInteractive();
				if(targetInteractive){
					adx = targetInteractive.xTYPE;
					adb = targetInteractive.bTYPE;
					if((int)targetInteractive.bTYPE >= 0 && Input.GetButtonDown("B")){
					print("b");
					float f = tform.position.y - targetInteractive.tform.position.y;
					string trigger = f > 2 ? loPickupTrigger : (f < -2 ? hiPickupTrigger : midPickupTrigger);
					if(targetInteractive.GetComponent<MouseBody>())
						trigger = mousePickupTrigger;
					FacePosition(tform.position);
					anim.SetTrigger(trigger);
					state = PROTAGSTATE.ANIMATION;
					}
					if((int)targetInteractive.xTYPE >= 0 && Input.GetButtonDown("X")){
						print("x");
						FacePosition(tform.position);
						string trigger = xKey(targetInteractive.xTYPE);
						if(trigger.Length > 0){
							anim.SetTrigger(trigger);
							state = PROTAGSTATE.ANIMATION;
						}
					}
				}
				if(Input.GetButtonDown("Y")){
					anim.SetTrigger(ystate == YTYPE.SIT ? sitTrigger : callTrigger);
					state = PROTAGSTATE.ANIMATION;
				}
				ActionDisplay.xButton.SetState((int)adx);
				ActionDisplay.bButton.SetState((int)adb);
			//check for interactives;
				/*Interactive[] iact = FindObjectsOfType<Interactive>();
				float minDist = Mathf.Infinity;
				int closest=-1;
				for(int i = 0; i < iact.Length; ++i){
					float dist = Vector3.Distance(tform.position, iact[i].tform.position);
					if(dist < minDist){
						minDist = dist;
						closest = i;
					}
				}
				if(minDist<interactSphere && closest > -1){
					InteractTarget.itar.target = 
				}*/
			UpdateInventory();
			break;
			case PROTAGSTATE.HOLD:
				UpdateInventory();

				Vector3 dropPoint = tform.position-dropOffset*tform.forward;
				RaycastHit rae = default(RaycastHit);
				if(Physics.Raycast(new Ray(dropPoint, Vector3.down), out rae)){
					dropPoint = rae.point;
				}
				InteractTarget.itar.target=dropPoint;
				//ActionDisplay.yButton.SetState((int)YTYPE.CALL);
				if(Items.GetItem(_heldItem).mouseIndex>-1){
						
						//ActionDisplay.xButton.SetState(inventory[ins] == -1 ? (int)XTYPE.PET : (int)Items.GetItem(inventory[ins]).mouseAction);
						//ActionDisplay.yButton.SetState(DayCycle.dnc.sunday ? (int)YTYPE.MARK : -2);
						if(Input.GetButtonDown(xbtn)){
							if(inventory[ins]>-1&&(int)Items.GetItem(inventory[ins]).mouseAction > -1){
								Mice.GetMouse(Items.GetItem(_heldItem).mouseIndex).UseItemOn(inventory[ins]);
								inventory[ins]=-1;
							}
							else{
								anim.SetTrigger(petTrigger);
							}
						}
						if( Input.GetButtonDown(ybtn)){
							Mice.GetMouse(Items.GetItem(_heldItem).mouseIndex).Mark();
						}
				}
				else{

				}
				if(Input.GetButtonDown(bbtn))
				{
					state = PROTAGSTATE.ANIMATION;
					anim.SetTrigger(dropTrigger);
					
				}

			break;
			case PROTAGSTATE.ANIMATION:
				rbody.velocity=Vector3.zero;
			break;
			case PROTAGSTATE.SIT:
				ActionDisplay.xButton.SetState(anim.GetBool(reachTrigger) ? (int)XTYPE.STORE : (int)XTYPE.REACH);
				ActionDisplay.yButton.SetState((int)YTYPE.STAND);
				if(Input.GetButtonDown("X")){
					anim.SetBool(reachTrigger, !anim.GetBool(reachTrigger));
				}
				if(Input.GetButtonDown("Y")){
					anim.SetTrigger(standTrigger);
					state = PROTAGSTATE.ANIMATION;
				}
			break;
		}
		if(state!= PROTAGSTATE.SIT && state!=PROTAGSTATE.ANIMATION){
			float lsx = Input.GetAxis(lsticku);
			float lsy = Input.GetAxis(lstickv);
			if(Mathf.Abs(lsx) < 0.1f && Mathf.Abs(lsy) < 0.1f){
				rbody.velocity = new Vector3(0,rbody.velocity.y,0);
				anim.SetBool(moveAnim,false);
				anim.SetFloat(speedFloat, 0);
			}
			else{
				Vector3 dirc = lsx*SPCam.cam.tform.right + lsy*SPCam.cam.tform.forward;
				dirc.y=0;
				float dirmag = new Vector2(lsx,lsy).magnitude;

				Vector3 dirf = Vector3.RotateTowards(-tform.forward, dirc, 1,acceleration*Time.deltaTime);
				tform.LookAt(tform.position - dirf);
				speed = Mathf.Lerp(speed,dirmag*speedMax,acceleration*Time.deltaTime);

				rbody.velocity = speed*dirf;
				anim.SetBool(moveAnim,true);
				anim.SetFloat(speedFloat,speed/speedMax);
			}
		}
	}
	public void Pat(){
		
	}

	public int heldItem { get{return _heldItem;}}

	public void SetHold(int item){
		_heldItem = item;
		//Items.GetItem(item).SetHeld(Vector3.zero, attachBone);
		state = item == -1 ? PROTAGSTATE.NORMAL : PROTAGSTATE.HOLD;
	}

	void RemoveFromInventory(int slot){
		Items.RemoveItem(slot);
		inventory[slot]=-1;
	}

	public void UpdateInventory(){
		for(int i = 0; i < inventoryAccess.Length; ++i){
		inventoryAccess[i].Update();
		if(inventoryAccess[i].getButtonDown){
		if(_heldItem>-1&&!Items.GetItem(_heldItem).storable){
			AudioSource.PlayClipAtPoint(invalid, SPCam.cam.tform.position);
			ins=i;
			continue;
		}
		if(_heldItem>-1){
			if(inventory[i]==-1){
				inventory[i]=_heldItem;
				Items.GetItem(_heldItem).SetStored();
			}
			else{
				int sitm = inventory[i];
				inventory[i] = _heldItem;
				Items.GetItem(_heldItem).SetStored();
				_heldItem=sitm;
				//Items.GetItem(_heldItem).SetHeld(Vector3.zero, attachBone);
			}

		}
		else{
			if(inventory[i]!=-1){
				_heldItem=inventory[i];
				//Items.GetItem(_heldItem).SetHeld(Vector3.zero, attachBone);
				inventory[i]=-1;
			}
		}
		ins=i;
		}
		}		
	}
	public void DropItem(){
		Items.GetItem(_heldItem).body.Trigger(this, CONTROL.BBUTTON);
		_heldItem = -1;
	}

	public void AnimEnd(){
		state = _heldItem > -1 ? PROTAGSTATE.HOLD : PROTAGSTATE.NORMAL;
	}
	public bool MoveState(PROTAGSTATE st){
		return st == PROTAGSTATE.NORMAL;
	}
	[System.Serializable]
	public class AxisButton{
		public string axis;
		public bool positive;
		float val, lastval;
		float threshold=0.1f;

		public bool getButtonDown{get{return positive ? (val > threshold && lastval < threshold) : (val < -threshold && lastval > -threshold );}}
		public bool getButton{get{return positive ? (val > threshold) : (val <-threshold);}}
		public bool getButtonUp{get{return positive ? (val < threshold && lastval > threshold) : (val > -threshold && lastval < -threshold);}}

		public void Update(){
			lastval=val;
			val=Input.GetAxis(axis);
		}
	}
}
