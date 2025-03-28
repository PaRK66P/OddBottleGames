using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

enum EN_STATES { NORMAL, DAMAGED, ARMORED, STUNNED, DEAD};

public class IchorManager : MonoBehaviour
{
    public IchorData data;

    private EN_STATES state;

    private float health;
    private GameObject healthbar;
    private float damageTimer = 0;
    private float deathTimer = 0;

    private bool isStunned = false;
    private float stunTimer = 0;

    private int phase = 0;
    private GameObject[] weakPoints;
    private bool isArmored = false;

    private List<GameObject> weakPointSpawnPos;
    private bool[] occupiedWeakPoints;

    private ObjectPoolManager pooler;
    private string prefabName;

    private VisualNovelScript visualNovelManager;

    public void InsantiateComponent(ref ObjectPoolManager dPooler, string dPrefabName, ref List<GameObject> dWeakPos, ref Canvas dUICanvas)
    {
        health = data.health;
        pooler = dPooler;
        prefabName = dPrefabName;

        stunTimer = data.initialStunTimer;
        isStunned = true;

        state = EN_STATES.NORMAL;

        foreach(attackPaternsScript c in GetComponents<attackPaternsScript>())
        {
            c.InstantiateComponent(ref dPooler);
            if(c.phaseNo == phase)
            {
                c.enabled = true;
            }
            else
            {
                c.enabled = false;
            }
            c.stunned = isStunned;
        }


        weakPoints = new GameObject[data.weakPointsNo[2]];

        for (int i = 0; i < data.weakPointsNo[2]; i++)
        {
            weakPoints[i] = Instantiate(data.weakPontsPrefab);
            weakPoints[i].GetComponent<WeakPointScript>().InsantiateComponent(data.weakPointHealth, data.weakPointSpriteNorm, data.weakPointSpriteHurt, data.weakPointDamageTimer);
            weakPoints[i].SetActive(false);
        }

        weakPointSpawnPos = dWeakPos;
        occupiedWeakPoints = new bool[weakPointSpawnPos.Count];

        healthbar = Instantiate(data.healthbar, dUICanvas.transform);
        healthbar.GetComponent<RectTransform>().Translate(new Vector3(Screen.width / 2 - 400, Screen.height - 240, 0));

        healthbar.GetComponent<Slider>().maxValue = health;
        healthbar.GetComponent<Slider>().value = health;

        visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if(state == EN_STATES.DEAD)
        {
            deathTimer -= Time.deltaTime;
            if(deathTimer <= 0)
            {
                visualNovelManager.StartNovelSceneByName("Ichor1.0");
                pooler.ReleaseObject(prefabName, gameObject);
            }
        }
        else
        {
            if (isStunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0)
                {
                    setStun(false);
                }
            }

            if (state == EN_STATES.DAMAGED)
            {
                damageTimer -= Time.deltaTime;
                if (damageTimer <= 0)
                {
                    damageTimer = 0;
                    if(isStunned)
                    {
                        state = EN_STATES.STUNNED;
                    }
                    else
                    {
                        state = EN_STATES.NORMAL;
                    }
                }
            }

            if (health <= data.nextPhaseHpPoint[phase] && !isArmored)
            {
                spawnWeakPoints();
                isArmored = true;
                state = EN_STATES.ARMORED;
            }
            if (isArmored)
            {
                int activeWeakPoints = 0;
                for (int i = 0; i < weakPoints.Count(); ++i)
                {
                    if (weakPoints[i].activeSelf == true)
                    {
                        activeWeakPoints++;
                    }
                }
                if (activeWeakPoints == 0)
                {
                    isArmored = false;
                    phase++;
                    setStun(true);
                    takeDamage(data.weakFinishDamage);
                    changePhase();
                }
            }
        }
        UpdateSprite();
    }

    public void takeDamage(float dmg)
    {
        if(!isArmored)
        {
            health -= dmg;
            damageTimer = data.damageTimer;
            state = EN_STATES.DAMAGED;

            if(health <= 0)
            {
                GetComponent<CompanionTargettingHandler>().ReleaseAsTarget();
                health = 0;
                state = EN_STATES.DEAD;
                deathTimer = data.deathTimer;
            }
            healthbar.GetComponent<Slider>().value = health;
        }
    }

    public void setStun(bool b)
    {
        isStunned = b;

        if(isStunned)
        {
            stunTimer = data.stunTimer;
            state = EN_STATES.STUNNED;
        }
        else
        {
            stunTimer = 0;
            state = EN_STATES.NORMAL;
        }

        foreach (attackPaternsScript c in GetComponents<attackPaternsScript>())
        {
            c.stunned = isStunned;
        }
    }

    private void changePhase()
    {
        foreach (attackPaternsScript c in GetComponents<attackPaternsScript>())
        {
            if (c.phaseNo == phase)
            {
                c.enabled = true;
            }
            else
            {
                c.enabled = false;
            }
        }
    }

    private void spawnWeakPoints()
    {
        for (int i = 0; i < data.weakPointsNo[phase]; ++i)
        {
            int x = UnityEngine.Random.Range(0, weakPointSpawnPos.Count);

            while (occupiedWeakPoints[x])
            {
                x++;
                if(x >= weakPointSpawnPos.Count)
                {
                    x = 0;
                }
            }

            occupiedWeakPoints[x] = true;

            weakPoints[i].transform.position = weakPointSpawnPos[x].transform.position;
            weakPoints[i].GetComponent<WeakPointScript>().spawn();
            weakPoints[i].SetActive(true);
        }

        for(int i = 0; i <occupiedWeakPoints.Count(); ++i)
        {
            occupiedWeakPoints[i] = false;
        }
    }

    private void UpdateSprite()
    {
        switch (state)
        {
            case EN_STATES.NORMAL:
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = data.Idle;
                    break;
                }
            case EN_STATES.ARMORED:
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = data.Armored;
                    break;
                }
            case EN_STATES.DAMAGED:
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = data.Hurt;
                    break;
                }
            case EN_STATES.STUNNED:
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = data.Stunned;
                    break;
                }
            case EN_STATES.DEAD:
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = data.Dead;
                    break;
                }
            default:
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = data.Idle;
                    break;
                }
        }
    }
}
