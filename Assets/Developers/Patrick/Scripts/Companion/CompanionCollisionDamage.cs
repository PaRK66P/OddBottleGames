using UnityEngine;

public class CompanionCollisionDamage : MonoBehaviour
{
    public enum CollisionDamageStates
    {
        None = 0,
        Player = 1,
        Enemy = 2
    }

    // Data
    private CompanionBossData _bossData;
    private CompanionFriendData _friendData;

    // Values
    private CollisionDamageStates _collisionDamageState;

    private bool _isNewLeap;

    public void InitialiseComponent(ref CompanionBossData bossData, ref CompanionFriendData friendData)
    {
        _bossData = bossData;
        _friendData = friendData;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (_collisionDamageState) // Determine what are valid targets
        {
            case CollisionDamageStates.None:
                break;
            case CollisionDamageStates.Player:
                if (collision.tag == "Player")
                {
                    Vector2 damageDirection = (collision.gameObject.transform.position - transform.position).normalized; // Direction from companion to player
                    if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                    {
                        collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection, 1, 30, _bossData.LeapDamage);
                    }
                    return;
                }
                break;
            case CollisionDamageStates.Enemy:
                if (((1 << collision.gameObject.layer) & _friendData.EnemyLayer) != 0)
                {
                    // Check all enemy types
                    if (collision.gameObject.tag == "Boss")
                    {
                        if(!_isNewLeap) { return; }
                        _isNewLeap = false;
                        collision.gameObject.GetComponent<IchorManager>().takeDamage((int)_friendData.LeapDamage);
                    }
                    else if (collision.gameObject.tag == "WeakPoint")
                    {
                        collision.gameObject.GetComponent<WeakPointScript>().takeDamage((int)_friendData.LeapDamage);
                    }
                    else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
                    {
                        collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(_friendData.LeapDamage, gameObject.transform.position - collision.gameObject.transform.position);
                    }
                }
                break;
        }
    }

    // In case damage happened during invulnerable state
    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 damageDirection = Vector2.left;

        switch (_collisionDamageState)
        {
            case CollisionDamageStates.None:
                break;
            case CollisionDamageStates.Player:
                if(collision.tag == "Player")
                {
                    damageDirection = (collision.gameObject.transform.position - transform.position).normalized; // Direction from companion to player
                    if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                    {
                        collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection);
                    }
                }
                break;
            case CollisionDamageStates.Enemy: // Enemies don't have an invulnerable state for damage so don't do anything
                break;
        }
    }

    public void ChangeState(CollisionDamageStates collisionDamageState)
    {
        _collisionDamageState = collisionDamageState;
    }

    public void SetNewLeap()
    {
        _isNewLeap = true;
    }
}
