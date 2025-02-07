using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    float projectileTimer = 5.0f;
    public float bulletSpeed = 10.0f;
    public Vector3 direction = Vector3.zero;
    public GameObject owner;

    public bool toBeDestroyed;
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        projectileTimer -= Time.deltaTime;
        UpdateProjectile();

    }

    void UpdateProjectile()
    {
        if (projectileTimer <= 0)
        {
            toBeDestroyed = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized);
        }
    }

    public void SetBulletDirectionAndSpeed(Vector3 newDirection, float newBulletSpeed)
    {
        direction = newDirection.normalized;
        bulletSpeed = newBulletSpeed;
        this.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }
}
