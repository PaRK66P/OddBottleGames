using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum CompanionMode
{
    MINIBOSS,
    COMPANION
}


public class CompanionLogic : MonoBehaviour
{
    [SerializeField]
    private bool companion;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpTime;

    private LayerMask targetLayer;
    
    [SerializeField] 
    private float explosionSize;

    [SerializeField]
    private Transform idlePosition;

    [SerializeField]
    private float maxHealth;
    private float currentHealth = 15;

    [SerializeField]
    private List<GameObject> currentTargets;
    private int targetIndex = 0;

    [SerializeField]
    private GameObject explosionObject;

    [SerializeField]
    private GameObject shockwaveObject;

    [SerializeField]
    ObjectPoolManager objectPoolManager;

    private bool selectedAction = false;
    private int currentAttackType = 1;
    private int shockwaveIterations = 0;

    private Vector3 selectedTargetPosition;
    private Vector3 startingPosition;
    private float timer;
    private bool alive = true;

    private bool modeLayerSelected = false;

    [SerializeField]
    private CompanionMode companionMode = CompanionMode.MINIBOSS;

    // Start is called before the first frame update
    void Start()
    {
        currentTargets = new List<GameObject>();
        if (companionMode == CompanionMode.MINIBOSS)
        {
            targetLayer = LayerMask.GetMask("Player");
        }
        else if (companionMode == CompanionMode.COMPANION)
        {
            targetLayer = LayerMask.GetMask("Enemy");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            return;
        }

        if (!modeLayerSelected)
        {
            if (companionMode == CompanionMode.MINIBOSS)
            {
                targetLayer = LayerMask.GetMask("Player");
            }
            else if (companionMode == CompanionMode.COMPANION)
            {
                targetLayer = LayerMask.GetMask("Enemy");
            }
            modeLayerSelected = true;
        }
    

        timer += Time.deltaTime;
        if (!selectedAction)
        {
            GameObject targetObject = GetClosestTarget();
            if(targetObject == null)
            {
                if ((idlePosition.position - transform.position + Vector3.down * 1.5f).magnitude > 0.1f)
                {
                    Vector3 direction = idlePosition.position - transform.position + Vector3.down * 1.5f;
                    direction = direction.normalized;
                    transform.position += direction * speed * Time.deltaTime;
                }
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
       // Instantiate(explosionObject, targetPosition, Quaternion.identity).GetComponent<ExplosionLogic>().InitialiseEffect(targetLayer, 5, 20, 0.5f, 1);

        GameObject newExplosion = objectPoolManager.GetFreeObject("Explosion");

        newExplosion.transform.rotation = Quaternion.identity;
        newExplosion.transform.position = targetPosition;

        ExplosionLogic newShockwaveScript = newExplosion.GetComponent<ExplosionLogic>();
        newShockwaveScript.InitialiseEffect(targetLayer, 5, 20, 0.5f, 1, objectPoolManager);
    }

    private void CreateShockwave(Vector3 targetPosition)
    {
        Vector3 rotation = targetPosition - transform.position;
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        GameObject newShockwave = objectPoolManager.GetFreeObject("Shockwave");
        
        newShockwave.transform.rotation = Quaternion.Euler(0, 0, rotz);
        newShockwave.transform.position = transform.position;

        ShockwaveLogic newShockwaveScript = newShockwave.GetComponent<ShockwaveLogic>();
        newShockwaveScript.InitialiseEffect(targetLayer, 2, rotation.normalized, 8, objectPoolManager);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(1 << collision.gameObject.layer);
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
        currentTargets.Add(target);
    }

    private void RemoveTarget(GameObject target)
    {
        currentTargets.Remove(target);
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
            StartCoroutine(Defeated());
        }
    }

    private IEnumerator Defeated()
    {
        alive = false;
        VisualNovelScript visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
        visualNovelManager.StartNovelSceneByName("Miniboss tester");
        Debug.Log("defeated");
        selectedAction = false;
        currentAttackType = 0;
        shockwaveIterations = 0;
        timer = 0.0f;
        yield return WaitForNovel();
        if (visualNovelManager.GetLastSelectionID() == 0)
        {
            JoinPlayer();
        }
        else
        {
            JoinBoss();
        }
    }

    IEnumerator WaitForNovel()
    {
        VisualNovelScript visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
        yield return new WaitWhile(() => visualNovelManager.isNovelSection == true);
    }
    public void JoinBoss()
    {
        this.companionMode = CompanionMode.MINIBOSS;
        modeLayerSelected = false;
        alive = true;
        Transform bossTransform = GameObject.FindGameObjectWithTag("Boss").transform;
        bossTransform.position -= new Vector3(0, -2, 0);
        idlePosition = bossTransform;
        currentHealth = 15;

        GetComponent<CircleCollider2D>().enabled = false;
        currentTargets.Clear();
        GetComponent<CircleCollider2D>().enabled = true;
        GetClosestTarget();
    }

    public void JoinPlayer()
    {
        this.companionMode = CompanionMode.COMPANION;
        idlePosition = GameObject.Find("PlayerProto").GetComponent<Transform>();
        currentHealth = 15;
        //targetIndex = -1;
        modeLayerSelected = false;
        alive = true;

        GetComponent<CircleCollider2D>().enabled = false;
        currentTargets.Clear();
        GetComponent<CircleCollider2D>().enabled = true;
        GetClosestTarget();
    }
}
