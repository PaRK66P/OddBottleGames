using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class IchorManager : MonoBehaviour
{
    public IchorData data;
    
    private float health;
    private int phase = 0;
    private GameObject[] weakPoints;

    private List<GameObject> weakPointSpawnPos;

    public void InsantiateComponent(ref ObjectPoolManager dPooler, ref List<GameObject> dWeakPos)
    {
        health = data.health;

        foreach(attackPaternsScript c in GetComponents<attackPaternsScript>())
        {
            if(c.phaseNo == phase)
            {
                c.enabled = true;
            }
            else
            {
                c.enabled = false;
            }
        }

        weakPoints = new GameObject[data.weakPointsNo[phase]];

        for (int i = 0; i <= data.weakPointsNo[phase]; i++)
        {
            weakPoints[i] = Instantiate(data.weakPontsPrefab, gameObject.transform);
            weakPoints[i].SetActive(false);
        }

        weakPointSpawnPos = dWeakPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= data.nextPhaseHpPoint[phase])
        {
            

            for(int i = 0; i < data.weakPointsNo[phase]; i++)
            {

            }

            phase++;
        }
    }

    public void takeDamage(float dmg)
    {
        health -= dmg;
    }

    public void setStun(bool b)
    {
        foreach (attackPaternsScript c in GetComponents<attackPaternsScript>())
        {
            c.stunned = b;
        }
    }

    public void changePhase()
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
}
