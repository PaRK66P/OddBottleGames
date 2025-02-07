using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLogic : MonoBehaviour
{
    private LayerMask target;
    private float damage;
    private float radius;
    private float delay;
    private float removal;

    private float timer;
    private bool firedDamage;

    private GameObject[] objectsToDamage;
    private int targetIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        firedDamage = false;

        objectsToDamage = new GameObject[1];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay && !firedDamage)
        {
            foreach (GameObject obj in objectsToDamage)
            {
                if(obj == null)
                {
                    continue;
                }

                if (obj.GetComponent<PlayerManager>() != null)
                {
                    Vector2 damageDirection = new Vector2(obj.transform.position.x - transform.position.x,
                        obj.transform.position.y - transform.position.y);
                    obj.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized);
                }
                else if (obj.GetComponent<AISimpleDetectionScript>() != null)
                {
                    obj.GetComponent<AISimpleDetectionScript>().TakeDamage(damage);
                }
                else if (obj.GetComponent<bossScript>() != null)
                {
                    obj.GetComponent<bossScript>().takeDamage(1);
                }
            }
            firedDamage = true;
        }
        else if (timer > removal)
        {
            Destroy(gameObject);
        }

    }

    public void InitialiseEffect(LayerMask damageLayer, float totalDamage, float explosionRadius, float explosionDelay, float removalTime)
    {
        target = damageLayer;
        damage = totalDamage;
        radius = explosionRadius;
        delay = explosionDelay;
        removal = removalTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((1 << collision.gameObject.layer) == target.value)
        {
            AddTarget(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == target.value)
        {
            RemoveTarget(collision.gameObject);
        }
    }

    private void AddTarget(GameObject target)
    {
        targetIndex++;
        if (targetIndex == objectsToDamage.Length)
        {
            GameObject[] newList = new GameObject[objectsToDamage.Length + 1];
            for (int i = 0; i < objectsToDamage.Length; i++)
            {
                newList[i] = objectsToDamage[i];
            }

            objectsToDamage = newList;
        }

        objectsToDamage[targetIndex] = target;
    }

    private void RemoveTarget(GameObject target)
    {
        int removalIndex = Array.IndexOf(objectsToDamage, target);
        if (removalIndex == -1)
        {
            return;
        }

        GameObject[] newList = new GameObject[objectsToDamage.Length - 1];

        objectsToDamage[removalIndex] = null;
        targetIndex--;

        int i = 0;
        foreach (GameObject obj in objectsToDamage)
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
