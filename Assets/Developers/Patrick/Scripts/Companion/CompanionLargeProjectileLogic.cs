using UnityEngine;

public class CompanionLargeProjectileLogic : MonoBehaviour
{
    // Objects
    private ObjectPoolManager _objectPoolManager;

    // Components
    private Rigidbody2D _rigidbody;

    // Values
    private string _name;
    private float _startTime;
    private float _lifespan;
    private float _damage;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private bool _isActive = false;

    public void Initialise(ref ObjectPoolManager poolManagerRef, string name, float lifespan, float size, float damage, Vector2 startPosition, Vector2 endPosition)
    {
        _startTime = Time.time;
        _lifespan = lifespan;

        _objectPoolManager = poolManagerRef;

        _name = name;

        _damage = damage;

        transform.localScale = new Vector3(size, size, 1.0f);

        transform.position = startPosition;
        _startPosition = startPosition;
        _endPosition = endPosition;

        //Right is default forwards
        Vector2 _direction = _endPosition - _startPosition;
        float AngleFromRight = Vector3.SignedAngle(Vector3.right, new Vector3(_direction.x, _direction.y, Vector3.right.z), new Vector3(0.0f, 0.0f, 1.0f));
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, AngleFromRight);

        _rigidbody = GetComponent<Rigidbody2D>();
        _isActive = true;
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            // Move projectile with movement method
            _rigidbody.MovePosition(InterpolateMethod());

            if(Time.time - _startTime >= _lifespan)
            {
                _isActive = false;
                _objectPoolManager.ReleaseObject(_name, gameObject);
            }
        }
    }

    // Currently linear interpolation
    private Vector2 InterpolateMethod()
    {
        return Vector2.Lerp(_startPosition, _endPosition, (Time.time - _startTime) / _lifespan);
    }

    // Damages player if in contact
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isActive)
        {
            if (collision.tag == "Player")
            {
                Vector2 damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                {
                    collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection, 1, 10, _damage);
                }
            }
        }
    }
}
