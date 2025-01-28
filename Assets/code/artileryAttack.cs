using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class artileryAttack : MonoBehaviour
{
    public float delay = 1;
    public float activeTime = 1;

    float timeElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= delay)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
        }

        if(timeElapsed >= activeTime + delay)
        {
            Destroy(gameObject);
        }

        //collision with player function
            //player take damage
    }
}
