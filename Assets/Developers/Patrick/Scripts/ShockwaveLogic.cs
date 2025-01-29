using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShockwaveLogic : MonoBehaviour
{
    private float speed;
    private Vector2 directionMovement;
    private float damage;
    private LayerMask target;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + directionMovement * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == target.value)
        {
            if (collision.gameObject.GetComponent<PlayerManager>() != null)
            {
                collision.gameObject.GetComponent<PlayerManager>().TakeDamage();
            }
            else if (collision.gameObject.GetComponent<TempDamage>() != null)
            {
                collision.gameObject.GetComponent<TempDamage>().TakeDamage(damage);
            }
        }
    }

    public void InitialiseEffect(LayerMask damageLayer, float totalDamage, Vector2 direction, float speedMovement)
    {
        target = damageLayer;
        damage = totalDamage;
        directionMovement = direction;
        speed = speedMovement;
    }


}
