using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    ObjectPoolManager pooler;
    enemyManager enemyMan;

    public void spawn()
    {
        GameObject obj = pooler.GetFreeObject(enemyPrefab.name);
        Debug.Log(obj);
        Debug.Log(pooler);
        Debug.Log(enemyPrefab);
        Debug.Log(enemyMan);
        obj.GetComponent<enemyScr>().InstantiateEnemy(ref pooler, enemyPrefab.name, ref enemyMan);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
    }

    public void setUp(ref ObjectPoolManager objPooler, ref enemyManager eneMan)
    {
        pooler = objPooler;
        enemyMan = eneMan;
    }
}
