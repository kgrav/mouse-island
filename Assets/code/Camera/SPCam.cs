using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CAMERAMODE {FOLLOW}
public class SPCam : NVComponent{
    static SPCam _cam;
	public static SPCam cam {get{if(!_cam)_cam = FindObjectOfType<SPCam>();return _cam;}}



    public Vector3 TransformInput(float h, float v){
        Vector3 hori = h*tform.right;
        Vector3 vert = v*tform.forward;
        Vector3 r = (hori + vert).normalized;
        return r;
    }

    CAMERAMODE mode = CAMERAMODE.FOLLOW;
    public Transform target;

    public float height;
    public float[] dists;
    int dist = 0;
    Vector3 targetDir;
    Vector3 angle;

    void Awake(){
        angle = Vector3.forward;
    }

    public float vmin, vmax, sensitivity;
    public string rstickh, rstickv;
    public string rstickb, lb;

    float xrot=0, yrot=0;

    protected override void NVUpdate(){
        if(!target)
            return;
        switch(mode){
            case CAMERAMODE.FOLLOW:
            if(Input.GetButtonDown(rstickb)){
                dist++;
                dist = dist >= dists.Length ? 0 : dist;
            }
            if(Input.GetButton(lb)){
                angle = -target.forward;

            }
            else{
                float rsx = Input.GetAxis(rstickh)*Time.deltaTime*sensitivity;
                float rsy = Input.GetAxis(rstickv)*Time.deltaTime*sensitivity;
                rsy = yrot + rsy > vmax ? 0 : (yrot + rsy < vmin ? 0 : rsy);
                xrot = rsx + xrot;
                xrot = xrot > 360 ? xrot - 360 : xrot;
                yrot = rsy + yrot;
                angle = Quaternion.Euler(yrot, xrot,0)*Vector3.forward;
                //Debug.DrawRay(target.position, angle*10, Color.red);
            }
            tform.position = target.position + height*Vector3.up + dists[dist]*angle;
            tform.LookAt(target.position + height*Vector3.up);
        break;
        }
    }

}