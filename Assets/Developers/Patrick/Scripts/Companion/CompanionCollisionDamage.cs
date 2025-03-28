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
    private CompanionBossData _bossData;
    private CompanionFriendData _friendData;


    public void InitialiseComponent(ref CompanionBossData bossData, ref CompanionFriendData friendData)
    {
        _bossData = bossData;
        _friendData = friendData;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 damageDirection = Vector2.left;

        switch (_collisionDamageState)
        {
            case CollisionDamageStates.NONE:
                break;
            case CollisionDamageStates.PLAYER:
                if (collision.tag == "Player")
                {
                    damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                    if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                    {
                        collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection, 1, 30, _bossData.leapDamage);
                    }
                    return;
                }
                break;
            case CollisionDamageStates.ENEMY:
                if (((1 << collision.gameObject.layer) & _friendData.enemyLayer) != 0)
                {
                    if (collision.gameObject.tag == "Boss")
                    {
                        collision.gameObject.GetComponent<IchorManager>().takeDamage((int)_friendData.leapDamage);
                    }
                    else if (collision.gameObject.tag == "WeakPoint")
                    {
                        collision.gameObject.GetComponent<WeakPointScript>().takeDamage((int)_friendData.leapDamage);
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

        switch (_collisionDamageState)
        {
            case CollisionDamageStates.NONE:
                break;
            case CollisionDamageStates.PLAYER:
                if(collision.tag == "Player")
                {
                    damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                    if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                    {
                        collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection);
                    }
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
