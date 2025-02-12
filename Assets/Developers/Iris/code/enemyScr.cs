using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class enemyScr : MonoBehaviour
{
    enemyManager enemyManager;
    ObjectPoolManager pooler;
    string prefabName;

    public void InstantiateEnemy(ref ObjectPoolManager poolMan, string prefName, ref enemyManager enemyMan)
    {
        pooler = poolMan;
        prefabName = prefName;
        enemyManager = enemyMan;
        if(GetComponent<attackPaternsScript>())
        {
            GetComponent<attackPaternsScript>().InstantiateComponent(ref pooler);
        }
        //transform.position = pos;
        //transform.rotation = UnityEngine.Quaternion.Euler(rot);
        //timeElapsed = 0;
        //damage = false;
        //changeColor(new Color(1, 1, 0, 1));
    }

    public void releaseEnemy()
    {
        enemyManager.decreaseEnemyCount();
        pooler.ReleaseObject(prefabName, gameObject);
    }
}
