using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodyBarrel : MonoBehaviour
{
    public ObjectPoolManager objectPoolManager;
    private GameObject currentExplosion;
    private GameObject currentExplosion1;
    [SerializeField] private LayerMask layers1;
    [SerializeField] private LayerMask layers2;
    private string[] collisionLayers = { "Projectile" };
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
        currentExplosion.GetComponent<ExplosionLogic>().InitialiseEffect(layers1, 6,8,0.1f,1,objectPoolManager);
        currentExplosion.transform.position = this.transform.position;
        currentExplosion1 = objectPoolManager.GetFreeObject("Explosion");
        currentExplosion1.GetComponent<ExplosionLogic>().InitialiseEffect(layers2, 6, 8, 0.1f, 1, objectPoolManager);
        currentExplosion1.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collLayers.value == LayerMask.GetMask("Projectile"))
        {
            if (collision.gameObject.GetComponent<ProjectileBehaviour>() != null)
            {
                TriggerExplosion();
            }
        }
    }
}
