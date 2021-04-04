using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NVComponent : MonoBehaviour {

	static bool init=false;

	Transform _tform;
	public Transform tform {get{if(!_tform)_tform=transform;return _tform;}}
	
	// Update is called once per frame
	void Update () {
		if(!pause)
			NVUpdate();
	}

	protected virtual void NVAwake(){

	}

	protected virtual void NVUpdate(){

	}

	public static bool pause;
}
