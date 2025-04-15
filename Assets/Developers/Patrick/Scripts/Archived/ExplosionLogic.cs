using System;
using UnityEngine;

public class ExplosionLogic : MonoBehaviour
{
    // Explosion
    private LayerMask _target;
    private float _damage;
    private float _delay;
    private float _removal;

    // Delay
    private float _timer;
    private bool _isFiredDamage;

    // Damage detection
    private GameObject[] _objectsToDamage;
    private int _targetIndex = -1;

    // Managers
    private ObjectPoolManager _objectPoolManager;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
        _isFiredDamage = false;

        _objectsToDamage = new GameObject[1];
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for the delay time
        _timer += Time.deltaTime;
        if (_timer > _delay && !_isFiredDamage)
        {
            // Damage all objects in the explosion radius
            foreach (GameObject obj in _objectsToDamage)
            {
                if(obj == null)
                {
                    continue;
                }

                if (obj.GetComponent<PlayerManager>() != null)
                {
                    Vector2 damageDirection = new Vector2(obj.transform.position.x - transform.position.x,
                        obj.transform.position.y - transform.position.y);
                    obj.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 10);
                }
                else if (obj.GetComponent<AISimpleBehaviour>() != null)
                {
                    obj.GetComponent<AISimpleBehaviour>().TakeDamage(_damage, gameObject.transform.position - obj.transform.position);
                }
                else if (obj.GetComponent<bossScript>() != null)
                {
                    obj.GetComponent<bossScript>().takeDamage(1);
                }
            }
            _isFiredDamage = true;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (_timer > _removal)
        {
            // Remove after removal delay
            _objectPoolManager.ReleaseObject("Explosion", this.gameObject);
        }

    }

    // Sets up the explosion
    public void InitialiseEffect(LayerMask damageLayer, float totalDamage, float explosionRadius, float explosionDelay, float removalTime, ObjectPoolManager objMgr)
    {
        _target = damageLayer;
        _damage = totalDamage;
        gameObject.transform.localScale = Vector3.one * explosionRadius;
        _delay = explosionDelay;
        _removal = removalTime;
        _timer = 0;
        _isFiredDamage = false;
        _objectPoolManager = objMgr;

        GetComponent<SpriteRenderer>().color = new Color(222.0f / 256.0f, 170.0f / 256.0f, 65.0f / 256.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add targets that enter the collision
        if((1 << collision.gameObject.layer) == _target.value)
        {
            AddTarget(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove targets that leave the collision
        if ((1 << collision.gameObject.layer) == _target.value)
        {
            RemoveTarget(collision.gameObject);
        }
    }

    // Adds the target game object to the damage list
    private void AddTarget(GameObject target)
    {
        _targetIndex++;
        if (_targetIndex == _objectsToDamage.Length)
        {
            GameObject[] newList = new GameObject[_objectsToDamage.Length + 1];
            for (int i = 0; i < _objectsToDamage.Length; i++)
            {
                newList[i] = _objectsToDamage[i];
            }

            _objectsToDamage = newList;
        }

        _objectsToDamage[_targetIndex] = target;
    }

    // Removes the target game object from the damage list
    private void RemoveTarget(GameObject target)
    {
        int removalIndex = Array.IndexOf(_objectsToDamage, target);
        if (removalIndex == -1)
        {
            return;
        }

        GameObject[] newList = new GameObject[_objectsToDamage.Length - 1];

        _objectsToDamage[removalIndex] = null;
        _targetIndex--;

        int i = 0;
        foreach (GameObject obj in _objectsToDamage)
        {
            if (obj == null)
            {
                continue;
            }

            newList[i] = obj;
            i++;
        }
    }
}
