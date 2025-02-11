using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodyBarrel : MonoBehaviour
{
    public ObjectPoolManager objectPoolManager;
    private GameObject currentExplosion;


    // Start is called before the first frame update
    void Start()
    {
        //TriggerExplosion();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void TriggerExplosion()
    {
        currentExplosion = objectPoolManager.GetFreeObject("Explosion");
        currentExplosion.GetComponent<ExplosionLogic>().InitialiseEffect(LayerMask.GetMask("Player"), 1,2,2,1,objectPoolManager);
        currentExplosion.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerExplosion();
    }
}
