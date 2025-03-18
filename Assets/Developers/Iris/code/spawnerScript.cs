using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    ObjectPoolManager pooler;
    enemyManager enemyMan;
    Canvas UICanvas;
    GameObject player;
    PathfindingManager pathfinder;
    List<GameObject> weakPointPos;


    public void spawn()
    {
        GameObject obj = pooler.GetFreeObject(enemyPrefab.name);
        obj.GetComponent<enemyScr>().InstantiateEnemy(ref pooler, enemyPrefab.name, ref enemyMan, ref UICanvas, ref player, ref pathfinder, ref weakPointPos);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
    }

    public void setUp(ref ObjectPoolManager objPooler, ref enemyManager eneMan, ref Canvas dUICanvas, ref GameObject dPlayer, ref PathfindingManager dPathfinder, ref List<GameObject> dWeakPos)
    {
        pooler = objPooler;
        enemyMan = eneMan;
        UICanvas = dUICanvas;
        player = dPlayer;
        pathfinder = dPathfinder;
        weakPointPos = dWeakPos;
    }
}
