using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PoisonPoolScript : MonoBehaviour
{
    private GameObject[] objectsToDamage;
    private int targetIndex = -1;
    [SerializeField]
    private LayerMask target;

    public float tickRate;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        objectsToDamage = new GameObject[1];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > tickRate)
        {
            timer = 0.0f;
            foreach (GameObject obj in objectsToDamage)
            {
                if (obj == null)
                {
                    continue;
                }

                if (obj.GetComponent<PlayerManager>() != null)
                {
                    obj.GetComponent<PlayerManager>().TakeDamage(Vector2.zero);
                }
                else if (obj.GetComponent<AISimpleBehaviour>() != null)
                {
                    obj.GetComponent<AISimpleBehaviour>().TakeDamage(1, gameObject.transform.position - obj.transform.position);
                }
                else if (obj.GetComponent<bossScript>() != null)
                {
                    obj.GetComponent<bossScript>().takeDamage(1);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == target.value)
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
