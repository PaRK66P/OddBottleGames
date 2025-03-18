using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class enemyScr : MonoBehaviour
{
    enemyManager enemyManager;
    ObjectPoolManager pooler;
    string prefabName;

    public void InstantiateEnemy(ref ObjectPoolManager poolMan, string prefName, ref enemyManager enemyMan, ref Canvas UICanvas, ref GameObject dPlayer, ref PathfindingManager dPathfinder)
    {
        pooler = poolMan;
        prefabName = prefName;
        enemyManager = enemyMan;
        if(GetComponent<attackPaternsScript>())
        {
            GetComponent<attackPaternsScript>().InstantiateComponent(ref pooler);
        }
        if (GetComponent<boss>())
        {
            GetComponent<boss>().InsantiateComponent(ref pooler, prefName, ref enemyManager, ref UICanvas);
        }
        if (GetComponent<IchorManager>())
        {
            GetComponent<IchorManager>().InsantiateComponent(ref pooler);
        }
        if (GetComponent<CompanionManager>())
        {
            GetComponent<CompanionManager>().InitialiseEnemy(ref dPlayer, ref poolMan, ref dPathfinder, ref UICanvas);
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

    //for companions since we dont want to release them when they die
    public void DecreaseEnemyCount()
    {
        enemyManager.decreaseEnemyCount();
    }
}
