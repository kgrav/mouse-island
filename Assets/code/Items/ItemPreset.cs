using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MicePreset")]
public class ItemPreset : ScriptableObject {
    public ITEMTYPE type;
    public GameObject bodyPrefab;
    public Vector3 baseRotation;
}