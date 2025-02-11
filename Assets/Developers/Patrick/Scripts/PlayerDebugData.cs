using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDebugData", menuName = "ScriptableObjects/PlayerDebugData", order = 2)]
public class PlayerDebugData : ScriptableObject
{
    [Header("Input")]
    [Min(0.0f)] public float dashInputBuffer;
    [Min(0.0f)] public float firingInputBuffer;

    [Header("Weapon Dropping")]
    public bool canDropWeapon = false;
    public GameObject weaponObject;

    [Header("Dashing")]
    public bool dashTowardsMouse = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
