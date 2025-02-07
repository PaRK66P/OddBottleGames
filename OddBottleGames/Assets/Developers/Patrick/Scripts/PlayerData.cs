using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    [Min(0.0f)] public float speed;

    [Header("Dashing")]
    [Min(0.0f)] public float dashTime;
    [Min(0.0f)] public float dashDistance;
    [Min(0.0f)] public float dashCooldown;
    [Min(0.0f)] public float dashInputBuffer;

    [Header("Collisions")]
    public LayerMask damageLayers;

    [Header("Shooting")]
    public GameObject baseProjectileType;
    [Min(0.0f)] public float baseProjectileSpeed;

    [Header("Damage")]
    [Min(0.0f)] public float controlLossTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
