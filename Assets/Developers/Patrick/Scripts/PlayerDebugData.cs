using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDebugData", menuName = "ScriptableObjects/PlayerDebugData", order = 2)]
public class PlayerDebugData : ScriptableObject
{
    [Header("Weapon Dropping")]
    public bool canDropWeapon = false;
    public GameObject weaponObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
