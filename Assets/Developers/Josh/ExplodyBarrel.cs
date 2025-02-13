using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodyBarrel : MonoBehaviour
{
    public ObjectPoolManager objectPoolManager;
    private GameObject currentExplosion;
    private string[] attackLayers = { "Player", "Enemy", "Enviroment" };
    private string[] collisionLayers = { "Projectile", "EnemyAttack" };
    LayerMask collLayers;

    // Start is called before the first frame update
    void Start()
    {
        objectPoolManager = GameObject.Find("ObjectPoolManager").GetComponent<ObjectPoolManager>();
        //Physics.IgnoreLayerCollision(LayerMask.GetMask(collisionLayers), true);
        collLayers = LayerMask.GetMask(collisionLayers);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void TriggerExplosion()
    {
        
        currentExplosion = objectPoolManager.GetFreeObject("Explosion");
        currentExplosion.GetComponent<ExplosionLogic>().InitialiseEffect(LayerMask.GetMask(attackLayers), 5,4,2,1,objectPoolManager);
        currentExplosion.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collLayers.value & (1 << collision.gameObject.layer)) > 0)
        {
            TriggerExplosion();
        }
    }
}
