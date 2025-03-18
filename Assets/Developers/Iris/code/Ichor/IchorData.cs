using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "IchorData", menuName = "DataObject/IchorData", order = 10)]
public class IchorData : ScriptableObject
{
    [Header("Health")]
    public float health = 100;
    public GameObject healthbar;
    public int hpPhase2 = 80;
    public int hpPhase3 = 60;

    [Space]

    [Header("Sprites")]
    public GameObject weakPontsPrefab;
    public float WeakPointsHP = 1;
    public int weakPontsPhase1 = 3;
    public int weakPontsPhase2 = 4;
    public int weakPontsPhase3 = 5;

    [Space]

    [Header("Sprites")]
    public Sprite Idle;
    public Sprite Hurt;
    public Sprite Armored;
    public Sprite Dead;

}