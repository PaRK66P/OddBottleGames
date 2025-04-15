using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "IchorData", menuName = "DataObject/IchorData", order = 10)]
public class IchorData : ScriptableObject
{
    [Header("Health")]
    public float health = 200;
    public float initialStunTimer = 2;
    public float damageTimer = 0.5f;
    public float deathTimer = 0.5f;
    public GameObject healthbarNormalPref;
    public GameObject healthbarArmoredPref;

    [Space]

    [Header("Weak Points")]
    public GameObject weakPontsPrefab;
    public int[] weakPointsNo = new int[3] { 3, 4, 5 };
    public float weakPointHealth = 15;
    public float weakFinishDamage = 30;
    public float weakPointDamageTimer = 0.5f;
    public Sprite weakPointSpriteNorm;
    public Sprite weakPointSpriteHurt;

    [Space]

    [Header("Phases")]
    public int[] nextPhaseHpPoint = new int[3] { 80, 50, 20};
    public float stunTimer = 3;

    [Space]

    [Header("Sprites")]
    public Sprite Idle;
    public Sprite Hurt;
    public Sprite Armored;
    public Sprite Stunned;
    public Sprite Dead;

}