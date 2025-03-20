using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class IchorManager : MonoBehaviour
{
    public IchorData data;
    
    private float health;
    private GameObject healthbar;
    private int phase = 0;
    private GameObject[] weakPoints;
    private bool isArmored = false;

    private List<GameObject> weakPointSpawnPos;
    private bool[] occupiedWeakPoints;

    private ObjectPoolManager pooler;
    private string prefabName;

    public void InsantiateComponent(ref ObjectPoolManager dPooler, string dPrefabName, ref List<GameObject> dWeakPos, ref Canvas dUICanvas)
    {
        health = data.health;
        pooler = dPooler;
        prefabName = dPrefabName;

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
        }

        weakPoints = new GameObject[data.weakPointsNo[2]];

        for (int i = 0; i < data.weakPointsNo[2]; i++)
        {
            weakPoints[i] = Instantiate(data.weakPontsPrefab);
            weakPoints[i].GetComponent<WeakPointScript>().InsantiateComponent(data.WeakPointsHP, data.weakPointSpriteNorm, data.weakPointSpriteHurt, data.weakPointDamageTimer);
            weakPoints[i].SetActive(false);
        }

        weakPointSpawnPos = dWeakPos;
        occupiedWeakPoints = new bool[weakPointSpawnPos.Count];

        healthbar = Instantiate(data.healthbar, dUICanvas.transform);
        healthbar.GetComponent<RectTransform>().Translate(new Vector3(Screen.width / 2 - 400, Screen.height - 240, 0));

        healthbar.GetComponent<Slider>().maxValue = health;
        healthbar.GetComponent<Slider>().value = health;

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= data.nextPhaseHpPoint[phase] && !isArmored)
        {
            spawnWeakPoints();
            isArmored = true;
        }
        if(isArmored)
        {
            int activeWeakPoints = 0;
            for(int i = 0; i < weakPoints.Count(); ++i)
            {
                if (weakPoints[i].activeSelf == true)
                {
                    activeWeakPoints++;
                }
            }
            if(activeWeakPoints == 0)
            {
                isArmored = false;
                phase++;
                changePhase();
            }
        }
    }

    public void takeDamage(float dmg)
    {
        if(!isArmored)
        {
            health -= dmg;

            if(health <= 0)
            {
                health = 0;
                pooler.ReleaseObject(prefabName, gameObject);
            }
            healthbar.GetComponent<Slider>().value = health;
        }
    }

    public void setStun(bool b)
    {
        foreach (attackPaternsScript c in GetComponents<attackPaternsScript>())
        {
            c.stunned = b;
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
}
