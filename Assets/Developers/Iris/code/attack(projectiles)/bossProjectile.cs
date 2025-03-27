using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossProjectile : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public ObjectPoolManager pooler;
    string prefabName;
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isActive)
        {
            if (collision.gameObject.tag == "Player")
            {
                Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                        collision.gameObject.transform.position.y - transform.position.y);
                collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 15);
                _isActive = false;
                pooler.ReleaseObject(prefabName, gameObject);
            }
            else if (collision.gameObject.layer == 6)
            {
                _isActive = false;
                pooler.ReleaseObject(prefabName, gameObject);
            }
        }
    }

    public void InstantiateComponent(ref ObjectPoolManager poolMan, string prefName, Vector3 pos, Vector3 rot)
    {
        pooler = poolMan;
        prefabName = prefName;
        transform.position = pos;
        transform.rotation = UnityEngine.Quaternion.Euler(rot);
        rb.velocity = transform.right * speed;
        _isActive = true;
    }
}
