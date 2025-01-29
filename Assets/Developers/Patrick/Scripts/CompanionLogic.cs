using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionLogic : MonoBehaviour
{
    [SerializeField]
    private bool companion;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private LayerMask targetLayer;
    [SerializeField] private float explosionSize;

    [SerializeField]
    private Transform idlePosition;
    [SerializeField]
    private float maxHealth;
    private float currentHealth;

    [SerializeField]
    private GameObject[] currentTargets;
    private int targetIndex = -1;

    [SerializeField]
    private GameObject explosionObject;

    [SerializeField]
    private GameObject shockwaveObject;

    private bool selectedAction = false;
    private int currentAttackType = 1;
    private int shockwaveIterations = 0;

    private Vector3 selectedTargetPosition;
    private Vector3 startingPosition;
    private float timer;
    private bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        currentTargets = new GameObject[4];
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            return;
        }

        timer += Time.deltaTime;
        if (!selectedAction)
        {
            GameObject targetObject = GetClosestTarget();
            if(targetObject == null)
            {
                Vector3 direction = idlePosition.position - transform.position; 
                direction = direction.normalized;

                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                if(timer < 0)
                {
                    return;
                }

                Vector3 target = targetObject.transform.position;

                if (currentAttackType == 1)
                {
                    currentAttackType = -1;
                }

                currentAttackType++;
                selectedAction = true;
                selectedTargetPosition = target;
                startingPosition = transform.position;
                timer = 0;
                shockwaveIterations = 0;
            }
        }
        else
        {
            if (currentAttackType == 0)
            {
                transform.position = Vector3.Lerp(startingPosition, selectedTargetPosition - (selectedTargetPosition - startingPosition).normalized * explosionSize, Mathf.Min(timer / jumpTime, 1));

                if(timer > jumpTime)
                {
                    CreateExplosion(selectedTargetPosition);
                    selectedAction = false;
                    timer = -1.5f;
                }
            }
            else if (currentAttackType == 1)
            {
                if(timer > 2 + shockwaveIterations * 0.6)
                {
                    CreateShockwave(selectedTargetPosition);
                    shockwaveIterations++;
                    if(shockwaveIterations > 2)
                    {
                        selectedAction = false;
                        timer -= 2;
                    }
                }
            }
        }
    }

    private void CreateExplosion(Vector3 targetPosition)
    {
        Instantiate(explosionObject, targetPosition, Quaternion.identity).GetComponent<ExplosionLogic>().InitialiseEffect(targetLayer, 5, 20, 0.5f, 1);
    }

    private void CreateShockwave(Vector3 targetPosition)
    {
        Vector3 rotation = targetPosition - transform.position;
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        Instantiate(shockwaveObject, transform.position, Quaternion.Euler(0, 0, rotz)).GetComponent<ShockwaveLogic>().InitialiseEffect(targetLayer, 2, rotation.normalized, 8);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(1 << collision.gameObject.layer);
        if((1 << collision.gameObject.layer) == targetLayer.value)
        {
            Debug.Log("Detected");
            AddTarget(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == targetLayer.value)
        {
            RemoveTarget(collision.gameObject);
        }
    }

    private void AddTarget(GameObject target)
    {
        targetIndex++;
        if(targetIndex == currentTargets.Length)
        {
            GameObject[] newList = new GameObject[currentTargets.Length + 1];
            for(int i = 0; i < currentTargets.Length; i++)
            {
                newList[i] = currentTargets[i];
            }

            currentTargets = newList;
        }

        currentTargets[targetIndex] = target;
    }

    private void RemoveTarget(GameObject target)
    {
        int removalIndex = Array.IndexOf(currentTargets, target);
        if (removalIndex == -1)
        {
           return;
        }

        GameObject[] newList = new GameObject[currentTargets.Length - 1];

        currentTargets[removalIndex] = null;
        targetIndex--;

        int i = 0;
        foreach (GameObject obj in currentTargets)
        {
            if (obj == null)
            {
                continue;
            }

            newList[i] = obj;
            i++;
        }
    }

    private GameObject GetClosestTarget()
    {
        if (targetIndex < 0)
        {
            return null;
        }

        GameObject closestObject = null;
        Vector3 currentPosition = transform.position;
        float currentDistance = 10000.0f;
        float testingDistance;

        foreach (GameObject target in currentTargets)
        {
            if (target == null)
            {
                continue;
            }
            testingDistance = (target.transform.position - currentPosition).magnitude;
            if (testingDistance < currentDistance)
            {
                currentDistance = testingDistance;
                closestObject = target;
            }
        }

        return closestObject;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            Defeated();
        }
    }

    private void Defeated()
    {
        alive = false;
    }
}
