using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : NVComponent {
	public EyeTarget target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(target){
			Vector3 dir = target.tform.position-tform.position;
			dir=Quaternion.Euler(0,90,0)*dir.normalized;
			tform.LookAt(dir);
		}
	}
}
