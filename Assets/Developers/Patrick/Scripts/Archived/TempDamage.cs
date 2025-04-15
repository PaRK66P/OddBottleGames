using UnityEngine;

public class TempDamage : MonoBehaviour
{
    private float _health = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Collision
        if (collision.gameObject.layer == 7)
        {
            // Damage the player
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized);
        }
    }

    public void TakeDamage(float hurtValue)
    {
        // Reduce health by damage
        _health -= hurtValue;

        // Check if dead
        if(_health < 0)
        {
            Destroy(gameObject);
        }
    }
}
