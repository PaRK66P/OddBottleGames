using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionCollisionDamage : MonoBehaviour
{
    public enum CollisionDamageStates
    {
        NONE = 0,
        PLAYER = 1,
        ENEMY = 2
    }

    private CollisionDamageStates _collisionDamageState;

    private CompanionFriendData _friendData;


    public void InitialiseComponent(ref CompanionFriendData friendData)
    {
        _friendData = friendData;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 damageDirection = Vector2.left;

        _collisionDamageState = CollisionDamageStates.PLAYER;

        switch (_collisionDamageState)
        {
            case CollisionDamageStates.NONE:
                break;
            case CollisionDamageStates.PLAYER:
                if (collision.tag == "Player")
                {
                    damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                    collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection);
                    return;
                }
                break;
            case CollisionDamageStates.ENEMY:
                if (((1 << collision.gameObject.layer) & _friendData.enemyLayer) != 0)
                {
                    if (collision.gameObject.tag == "Boss")
                    {
                        collision.gameObject.GetComponent<boss>().takeDamage((int)_friendData.leapDamage);
                    }
                    else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
                    {
                        collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(_friendData.leapDamage, gameObject.transform.position - collision.gameObject.transform.position);
                    }
                }
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 damageDirection = Vector2.left;

        _collisionDamageState = CollisionDamageStates.PLAYER;

        switch (_collisionDamageState)
        {
            case CollisionDamageStates.NONE:
                break;
            case CollisionDamageStates.PLAYER:
                if(collision.tag == "Player")
                {
                    damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                    collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection);
                }
                break;
            case CollisionDamageStates.ENEMY:

                break;
        }
    }

    public void ChangeState(CollisionDamageStates collisionDamageState)
    {
        _collisionDamageState = collisionDamageState;
    }
}
