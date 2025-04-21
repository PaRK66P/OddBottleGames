using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTimer : MonoBehaviour
{
    public float switchTimer = 5;

    // Update is called once per frame
    void Update()
    {
        switchTimer -= Time.deltaTime;
        if(switchTimer <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
