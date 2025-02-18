using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class boss : MonoBehaviour
{
    public int health = 100;
    ObjectPoolManager pooler;
    string prefabName;
    enemyManager enemyMan;

    public void takeDamage(int dmg)
    {
        health -= dmg;
        if(health <= 0)
        {
            enemyMan.decreaseEnemyCount();
            pooler.ReleaseObject(prefabName, gameObject);
        }
    }

    public void InsantiateComponent(ref ObjectPoolManager objPooler, string prefName, ref enemyManager eneMan)
    {
        prefabName = prefName;
        pooler = objPooler;
        enemyMan = eneMan;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
