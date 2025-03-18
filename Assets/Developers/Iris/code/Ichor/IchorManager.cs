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

    public void InsantiateComponent(ref ObjectPoolManager dPooler)
    {
        health = data.health;

        weakPoints = new GameObject[data.weakPontsPhase3];

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

        for (int i = 0; i <= data.weakPontsPhase3; i++)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
