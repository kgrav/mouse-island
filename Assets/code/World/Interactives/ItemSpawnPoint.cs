using System;
using UnityEngine;

public class ItemSpawnPoint : NVComponent {
    public GameObject prefab;

    public void Trigger(){
        Instantiate(prefab, tform.position, tform.rotation);
    }
}