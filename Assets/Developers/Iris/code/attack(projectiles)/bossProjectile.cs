using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossProjectile : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    ObjectPoolManager pooler;
    string prefabName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage();
            pooler.ReleaseObject(prefabName, gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        pooler.ReleaseObject(prefabName, gameObject);
    }

    public void InstantiateComponent(ref ObjectPoolManager poolMan, string prefName)
    {
        pooler = poolMan;
        prefabName = prefName;
        ///rotation and stuff
        rb.velocity = transform.right * speed;
    }
}
