using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraProbe : MonoBehaviour
{

    bool on = false;
    bool RETURNING = false;
    Transform whos;
    public Vector3 position;
    public Vector3 goal;
    public Vector3 direction;
    ProtagController watch;
    NVCam camera;
    public float speed;

    void Start()
    {
        watch = ProtagController.ptg;
        position = transform.position - whos.position;
        goal = position;
        whos = watch.tform;
        camera = NVCam.nvcam;
        direction = whos.transform.forward * (-1);
    }
    bool freeze;
    public void SetFreeze(bool f)
    {
        freeze = f;
    }
    void Update()
    {

        if (freeze)
            return;
            if (watch.rbody.velocity.magnitude > 0)
            { 
                goal = whos.position - 2 * whos.transform.forward + Vector3.up * 2;
            }
            else
            {
                goal = whos.position + whos.TransformDirection(position.normalized) + Vector3.up * 2;
            }
        direction = (goal - transform.position).normalized;
        float dista = Vector3.Distance(goal, transform.position);
        float smn = speed + watch.rbody.velocity.magnitude * 2;
        if (dista > 1)
        {
            transform.Translate(direction * (dista) * smn * Time.deltaTime);
        }
    }

    public bool colli = false;
    public void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<StaticGeometry>())
        {
            camera.SetDomain(CAMDOM.PTAG, CAMDOM.PTAG);
            colli = true;
        }

    }
    public void OnTriggerExit(Collider c)
    {
        if (c.GetComponent<StaticGeometry>())
        {
            camera.SetDomain(CAMDOM.PROBE, CAMDOM.PROBE);
            colli = false;
        }
    }
    public void SetActivity(bool b)
    {
        on = b;
    }
}

