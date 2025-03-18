using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "IchorData", menuName = "DataObject/IchorData", order = 10)]
public class IchorData : ScriptableObject
{
    [Header("Health")]
    public float health = 200;
    public GameObject healthbar;
    public Sprite healthbarNormalSprite;
    public Sprite healthbarArmoredSprite;

    [Space]

    [Header("Phases")]
    public float weakPointHealth = 15;
    public float weakFinishDamage = 30;

    [Space]

    [Header("Phases")]
    public int[] nextPhaseHpPoint = new int[3] { 80, 50, 20};


    [Space]

    [Header("Sprites")]
    public GameObject weakPontsPrefab;
    public float WeakPointsHP = 1;
    public int[] weakPointsNo = new int[3] { 3, 4, 5 };

    [Space]

    [Header("Sprites")]
    public Sprite Idle;
    public Sprite Hurt;
    public Sprite Armored;
    public Sprite Dead;

}