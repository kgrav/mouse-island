using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpot : NVComponent {
    public GameObject prefab;
    public string input;

    public float chance;

    void Update(){
        if(Input.GetButtonDown(input)){
            
            float c = UnityEngine.Random.Range(0.0f,1.0f);
            if(c < chance)
                Instantiate(prefab).transform.position = tform.position;

        }

    }
}