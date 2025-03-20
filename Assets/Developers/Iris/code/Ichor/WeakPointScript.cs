using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPointScript : MonoBehaviour
{
    private float maxHealth;
    private float health;

    private bool damaged = false;
    private float damagedTimer = 0;
    private float maxDamagedTimer = 0.2f;

    private Sprite normalSprite;
    private Sprite damagedSprite;

    public void InsantiateComponent(ref ObjectPoolManager dPooler, float dHealth, Sprite dNormalSprite, Sprite dDamagedSprite, float dDamagedTimer)
    {
        maxHealth = dHealth;

        normalSprite = dNormalSprite;
        damagedSprite = dDamagedSprite;

        maxDamagedTimer = dDamagedTimer;
    }

    public void spawn()
    {
        health = maxHealth;
        damaged = false;
        damagedTimer = 0;

        GetComponentInChildren<SpriteRenderer>().sprite = normalSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(damaged)
        {
            damagedTimer -= Time.deltaTime;
            if(damagedTimer <= 0)
            {
                damaged = false;
                GetComponentInChildren<SpriteRenderer>().sprite = normalSprite;
            }
        }

        if(health <= 0)
        {
            //gameObject.SetActive(false);
        }
    }

    public void takeDamage(float dmg)
    {
        health -= dmg;
        damaged = true;

        damagedTimer = maxDamagedTimer;
        GetComponentInChildren<SpriteRenderer>().sprite = damagedSprite;
    }
}
