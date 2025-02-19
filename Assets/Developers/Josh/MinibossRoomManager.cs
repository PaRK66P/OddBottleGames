using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossRoomManager : MonoBehaviour
{
    public enemyManager enemyManager;
    private bool roomStart = false;
    private bool roomEnd = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyManager.enemyNumber > 0)
        {
            roomStart = true;
        }
        if (roomStart)
        {
             if (enemyManager.enemyNumber == 0)
            {
                roomEnd = true;
            }
        }
        if (roomEnd)
        {
            roomStart = false;
            roomEnd = false;
        }
    }
}
