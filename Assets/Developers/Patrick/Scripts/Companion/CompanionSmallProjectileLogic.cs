using UnityEngine;

public class CompanionSmallProjectileLogic : MonoBehaviour
{
    // Objects
    private ObjectPoolManager _objectPoolManager;

    // Components
    private Rigidbody2D _rigidbody;

    // Values
    private LayerMask _environmentMask;
    private LayerMask _playerMask;

    private string _name;
    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private bool _isActive = false;

    public void Initialise(ref ObjectPoolManager objectPoolManager, string name, Vector2 startPosition, Vector2 direction, float speed, float size, float damage, LayerMask environmentMask, LayerMask playerMask)
    {
        _objectPoolManager = objectPoolManager;

        _name = name;

        transform.position = startPosition;

        _direction = direction;
        //Right is default forwards
        float AngleFromRight = Vector3.SignedAngle(Vector3.right, new Vector3(direction.x, direction.y, Vector3.right.z), new Vector3(0.0f, 0.0f, 1.0f));
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, AngleFromRight);

        _speed = speed;
        transform.localScale = new Vector3(size, size, 1.0f);
        _damage = damage;

        _rigidbody = GetComponent<Rigidbody2D>();
        _isActive = true;

        _environmentMask = environmentMask;
        _playerMask = playerMask;
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            _rigidbody.MovePosition(_rigidbody.position + _direction * _speed * Time.fixedDeltaTime);
        }
    }

    // Checks for player or environmental collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isActive)
        {
            if (((1 << collision.gameObject.layer) & _playerMask) != 0)
            {
                Vector2 damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                {
                    collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection, 1, 10, _damage);

                    _isActive = false;
                    _objectPoolManager.ReleaseObject(_name, gameObject);
                }
            }
            else if(((1 << collision.gameObject.layer) & _environmentMask) != 0)
            {
                _isActive = false;
                _objectPoolManager.ReleaseObject(_name, gameObject);
            }
        }
    }
}
