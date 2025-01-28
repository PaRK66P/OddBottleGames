using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class bossScript : MonoBehaviour
{
    public int health = 100;
    public float restPeriod = 3;
    float restTimer = 0;
    bool pauseRest = false;
    int attackNo = 0;

    [Space]
    [Header("Attacks variables")]
    public GameObject artileryPrefab;
    [Header("Random artilery strike")]
    public int randomArtileryProjectileNo = 10;
    [Header("Targeted artilery strike")]
    public int targetedArtileryProjectileNo = 5;
    public float timeBetweenStrikes = 0.5f;
    int strikesSpawned = 0;
    float strikesTimer;


    public void takeDamage(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            die();
        }
    }

    void die()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseRest)
        {
            restTimer += Time.deltaTime;
            if (restTimer >= restPeriod)
            {
                //attackNo = 1;
                attackNo = UnityEngine.Random.Range(0, 2);
                //attackNo = 1;

                strikesSpawned = 0;
                strikesTimer = timeBetweenStrikes;

                Debug.Log("strikes reset");

                Attack();
                restTimer = 0;
            }
        }
        else
        {
            Attack();
        }
    }

    void Attack()
    {
        switch (attackNo)
        {
            case 0:
                {
                    RandomArtileryAttack();
                    break;
                }
            case 1:
                {
                    pauseRest = true;
                    TargetedArtileryStrike();
                    break;
                }
            default:
                break;
        }
    }

    void RandomArtileryAttack()
    {
        for (int i = 0; i<= randomArtileryProjectileNo; ++i)
        {
            //UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), 0);

            Instantiate(artileryPrefab, pos, UnityEngine.Quaternion.Euler(0, 0, 0));
        }
    }

    void TargetedArtileryStrike()
    {
        strikesTimer += Time.deltaTime;
        if(strikesTimer >= timeBetweenStrikes)
        {
            strikesTimer = 0;
            if (strikesSpawned < 5)
            {
                UnityEngine.Vector3 pos = new UnityEngine.Vector3(0, 0, 0);

                if (GameObject.FindGameObjectWithTag("Player"))
                {
                    pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                }

                Instantiate(artileryPrefab, pos, UnityEngine.Quaternion.Euler(0, 0, 0));

                Debug.Log(strikesSpawned);
                strikesSpawned++;
            }
            else
            {
                pauseRest = false;
                Debug.Log(pauseRest);
            }
        }

        
    }
}
