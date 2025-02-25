using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionCollisionDamage : MonoBehaviour
{
    enum CollisionDamageStates
    {
        NONE = 0,
        ACTIVE = 1,
        LEAPING = 2
    }

    private CollisionDamageStates collisionDamageStates;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 damageDirection = Vector2.left;

        collisionDamageStates = CollisionDamageStates.ACTIVE;

        switch (collisionDamageStates)
        {
            case CollisionDamageStates.NONE:
                break;
            case CollisionDamageStates.ACTIVE:
                if(collision.tag == "Player")
                {
                    damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                    collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection);
                }
                break;
            case CollisionDamageStates.LEAPING:

                break;
        }
    }
}
