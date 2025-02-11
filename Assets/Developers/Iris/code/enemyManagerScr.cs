using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyManager : MonoBehaviour
{
    public int enemyNumber;
    public List<TriggerScript> trigers;
    public List<GameObject> doors;
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
