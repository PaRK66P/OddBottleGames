using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificEnemyTestScript : MonoBehaviour
{
    float tim = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tim += Time.deltaTime;
        if(tim >= 5)
        {
            GetComponent<enemyScr>().releaseEnemy();
        }
    }
}
