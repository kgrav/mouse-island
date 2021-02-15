using System;
using UnityEngine;

public class ItemSpawner : Interactive {
    public ItemSpawnPoint[] spawnPoints;

    public override void Trigger(ProtagController protag, CONTROL button){
        if(button == CONTROL.XBUTTON){
            foreach(ItemSpawnPoint i in spawnPoints){
                i.Trigger();
            }
        }
    }
}