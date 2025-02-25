using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CompanionLargeProjectileLogic : MonoBehaviour
{
    private Rigidbody2D rb;

    private ObjectPoolManager poolManager;

    private string _name;
    private float _startTime;
    private float _lifespan;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private bool _isActive = false;

    public void Initialise(ref ObjectPoolManager poolManagerRef, string name, float lifespan, Vector2 startPosition, Vector2 endPosition)
    {
        _startTime = Time.time;
        _lifespan = lifespan;

        poolManager = poolManagerRef;

        _name = name;

        transform.position = startPosition;
        _startPosition = startPosition;
        _endPosition = endPosition;

        rb = GetComponent<Rigidbody2D>();
        _isActive = true;
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            rb.MovePosition(InterpolateMethod());

            if(Time.time - _startTime >= _lifespan)
            {
                _isActive = false;
                poolManager.ReleaseObject(_name, gameObject);
            }
        }
    }

    private Vector2 InterpolateMethod()
    {
        return Vector2.Lerp(_startPosition, _endPosition, (Time.time - _startTime) / _lifespan);
    }
}
