using UnityEngine;

public class ShockwaveLogic : MonoBehaviour
{
    // Values
    private float _speed;
    private Vector2 _directionMovement;
    private float _damage;
    private LayerMask _target;

    // Components
    private Rigidbody2D _rb;

    // Managers
    private ObjectPoolManager _objectPoolManager;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _directionMovement * _speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Target collision
        if ((1 << collision.gameObject.layer) == _target.value)
        {
            if (collision.gameObject.GetComponent<PlayerManager>() != null) // Check for player
            {
                Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                    collision.gameObject.transform.position.y - transform.position.y);
                collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 10);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null) // Check for AI
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(_damage, gameObject.transform.position - collision.gameObject.transform.position);
            }
            else if (collision.gameObject.GetComponent<bossScript>() != null) // Check for boss
            {
                collision.gameObject.GetComponent<bossScript>().takeDamage(1);
            }
        }

        // Environment collision
        if(collision.gameObject.layer == 6)
        {
            _objectPoolManager.ReleaseObject("Shockwave", this.gameObject);
        }
    }

    // Sets up the shockwave
    public void InitialiseEffect(LayerMask damageLayer, float totalDamage, Vector2 direction, float speedMovement, ObjectPoolManager objMgr)
    {
        _target = damageLayer;
        _damage = totalDamage;
        _directionMovement = direction;
        _speed = speedMovement;
        _objectPoolManager = objMgr;
    }
}
