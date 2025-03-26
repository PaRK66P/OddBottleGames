using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionSmallProjectileLogic : MonoBehaviour
{
    private Rigidbody2D rb;

    private ObjectPoolManager poolManager;

    private LayerMask _environmentMask;
    private LayerMask _playerMask;

    private string _name;
    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private bool _isActive = false;

    public void Initialise(ref ObjectPoolManager poolManagerRef, string name, Vector2 startPosition, Vector2 direction, float speed, float size, float damage, LayerMask environmentMask, LayerMask playerMask)
    {
        poolManager = poolManagerRef;

        _name = name;

        transform.position = startPosition;

        _direction = direction;
        //Right is default forwards
        float AngleFromRight = Vector3.SignedAngle(Vector3.right, new Vector3(direction.x, direction.y, Vector3.right.z), new Vector3(0.0f, 0.0f, 1.0f));
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, AngleFromRight);

        _speed = speed;
        transform.localScale = new Vector3(size, size, 1.0f);
        _damage = damage;

        rb = GetComponent<Rigidbody2D>();
        _isActive = true;
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            rb.MovePosition(rb.position + _direction * _speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isActive)
        {
            if (collision.gameObject.tag == "Player")
            {
                Vector2 damageDirection = (collision.gameObject.transform.position - transform.position).normalized;
                if (collision.gameObject.GetComponent<PlayerManager>().CanBeDamaged())
                {
                    collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection, 1, 10, _damage);

                    _isActive = false;
                    poolManager.ReleaseObject(_name, gameObject);
                }
            }
        }
    }
}
