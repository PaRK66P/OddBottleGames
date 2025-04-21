using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum EN_STATES { NORMAL, DAMAGED, ARMORED, STUNNED, DEAD};

public class IchorManager : MonoBehaviour
{
    public IchorData data;

    private EN_STATES state;

    private float health;
    private GameObject healthbarNormal;
    private GameObject healthbarArmored;
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

        healthbarNormal = Instantiate(data.healthbarNormalPref, dUICanvas.transform);
        healthbarNormal.GetComponent<Slider>().maxValue = health;
        healthbarNormal.GetComponent<Slider>().value = health;

        healthbarArmored = Instantiate(data.healthbarArmoredPref, dUICanvas.transform);
        healthbarArmored.GetComponent<Slider>().maxValue = health;
        healthbarArmored.GetComponent<Slider>().value = health;
        healthbarArmored.SetActive(false);

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
                healthbarArmored.SetActive(false);
                healthbarNormal.SetActive(false);
                visualNovelManager.StartNovelSceneByName("Ichor1.0");
                visualNovelManager.onNovelFinish.AddListener(VNEnd);
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

            if(phase < 3)
            {
                if (health <= data.nextPhaseHpPoint[phase] && !isArmored)
                {
                    spawnWeakPoints();
                    isArmored = true;
                    state = EN_STATES.ARMORED;
                    healthbarNormal.SetActive(false);
                    healthbarArmored.SetActive(true);
                }
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
            healthbarArmored.GetComponent<Slider>().value = health;
            healthbarNormal.GetComponent<Slider>().value = health;
        }
    }

    public void setStun(bool b)
    {
        isStunned = b;

        if(isStunned)
        {
            stunTimer = data.stunTimer;
            state = EN_STATES.STUNNED;
            healthbarArmored.SetActive(false);
            healthbarNormal.SetActive(true);
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

    public void VNEnd()
    {
        switch(visualNovelManager.GetLastSelectionID())
        {
            //bad-----------------------------------------------------------
            case 1:
            case 2:
            case 4:
            case 5:
            case 7:
            case 8:
            case 10:
            case 11:
            case 13:
            case 14:
            case 16:
            case 18:
            case 22:
            case 26:
            case 28:
            case 29:
            case 31:
            case 32:
            case 34:
            case 35:
            case 36:
            case 38:
            case 39:
            case 40:
            case 42:
            case 43:
            case 44:
            case 45:
            case 46:
            case 47:
            case 48:
                SceneManager.LoadScene(3);
                break;

            //good------------------------------------------------------------
            case 0:
            case 3:
            case 6:
            case 9:
            case 12:
            case 15:
            case 17:
            case 19:
            case 20:
            case 21:
            case 23:
            case 24:
            case 25:
            case 27:
            case 30:
            case 33:
            case 37:
            case 41:
                SceneManager.LoadScene(2);
                break;
            default:
                SceneManager.LoadScene(3);
                Debug.Log("VisualNovel case not found.");                
                break;
        }
    }
}
