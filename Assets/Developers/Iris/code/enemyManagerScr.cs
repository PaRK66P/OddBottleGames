using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyManager : MonoBehaviour
{
    public ObjectPoolManager pooler;
    public int enemyNumber = 0;
    public List<TriggerScript> trigers;
    public List<GameObject> doors;
    public List<spawnerScript> spawners;
    public Canvas UICanvas;
    public GameObject player;
    public PathfindingManager pathfinder;
    public SoundManager SoundManagerObject;
    public List<GameObject> weakPointsList;
    public GameObject ProjectileDespawner;

    private enemyManager myself;

    public void lockDoors()
    {
        foreach (TriggerScript t in trigers)
        {
            t.isTriggered = true;
        }

        foreach (GameObject d in doors)
        {
            d.SetActive(true);
        }
        foreach (spawnerScript s in spawners)
        {
            s.setUp(ref pooler, ref myself, ref UICanvas, ref player, ref pathfinder, ref SoundManagerObject, ref weakPointsList, ref ProjectileDespawner);
            s.spawn();
            enemyNumber++;
        }
    }

    void unlockDoors()
    {
        foreach (GameObject d in doors)
        {
            d.GetComponentInChildren<DoorAnimationScript>().CloseDoorCommand();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject d in doors)
        {
            d.SetActive(false);
        }

        myself = GetComponent<enemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //provisional
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    decreaseEnemyCount();
        //}

        
    }

    public void decreaseEnemyCount()
    {
        enemyNumber--;


        if (enemyNumber <= 0)
        {
            unlockDoors();
        }
    }
}
