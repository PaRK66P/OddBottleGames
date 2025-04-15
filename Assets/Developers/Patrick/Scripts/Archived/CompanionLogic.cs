using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CompanionMode
{
    Miniboss,
    Companion
}


public class CompanionLogic : MonoBehaviour
{
    [SerializeField]
    private bool _companion;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpTime;

    [SerializeField]
    private GameObject _hitBoxObject;

    private LayerMask _targetLayer;
    
    [SerializeField] 
    private float _explosionSize;

    [SerializeField]
    private Transform _idlePosition;

    private float _currentHealth = 15;

    [SerializeField]
    private List<GameObject> _currentTargets;
    private int _targetIndex = 0;

    [SerializeField]
    ObjectPoolManager _objectPoolManager;

    private bool _selectedAction = false;
    private int _currentAttackType = 1;
    private int _shockwaveIterations = 0;

    private Vector3 _selectedTargetPosition;
    private Vector3 _startingPosition;
    private float _timer;
    private bool _alive = true;

    private bool _modeLayerSelected = false;

    [SerializeField]
    private CompanionMode _companionMode = CompanionMode.Miniboss;

    private bool _displayVN = true;

    private Slider _healthSlider;

    private MinibossRoomManager _room2;

    // Start is called before the first frame update
    void Start()
    {
        _currentTargets = new List<GameObject>();
        if (_companionMode == CompanionMode.Miniboss)
        {
            _targetLayer = LayerMask.GetMask("Player");
        }
        else if (_companionMode == CompanionMode.Companion)
        {
            _targetLayer = LayerMask.GetMask("Enemy");
        }
        _idlePosition = new GameObject("mimiTrans").transform;
        _idlePosition.position = this.transform.position;
        _objectPoolManager = GameObject.Find("ObjectPoolManager").GetComponent<ObjectPoolManager>();

        _displayVN = true;

        _healthSlider = GetComponentInChildren<Slider>();
        _healthSlider.maxValue = _currentHealth;
        _healthSlider.minValue = 0;
        _healthSlider.value = _currentHealth;

        _room2 = GameObject.Find("Room2").GetComponent<MinibossRoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_alive)
        {
            return;
        }

        // Set up target layer if one isn't selected
        if (!_modeLayerSelected)
        {
            if (_companionMode == CompanionMode.Miniboss)
            {
                _targetLayer = LayerMask.GetMask("Player");
            }
            else if (_companionMode == CompanionMode.Companion)
            {
                _targetLayer = LayerMask.GetMask("Enemy");
            }
            _modeLayerSelected = true;
        }


        _timer += Time.deltaTime;
        // Select an action if none is selected
        if (!_selectedAction)
        {
            GameObject targetObject = GetClosestTarget();
            if(targetObject == null)
            {
                if ((_idlePosition.position - transform.position + Vector3.down * 1.5f).magnitude > 0.1f)
                {
                    Vector3 direction = _idlePosition.position - transform.position + Vector3.down * 1.5f;
                    float distance = direction.magnitude;
                    direction = direction.normalized;
                    if (distance > 4)
                    {
                        _speed = Mathf.Lerp(_speed, 6, 0.3f);
                    }
                    else if (distance > 3)
                    {
                        _speed = Mathf.Lerp(_speed, 5, 0.3f);
                    }
                    else if (distance > 2)
                    {
                        _speed = Mathf.Lerp(_speed, 4, 0.3f);
                    }
                    else
                    {
                        _speed = Mathf.Lerp(_speed, 3, 0.3f);
                    }
                    transform.position += direction * _speed * Time.deltaTime;
                }
            }
            else
            {
                if(_timer < 0)
                {
                    return;
                }

                Vector3 target = targetObject.transform.position;

                if (_currentAttackType == 1)
                {
                    _currentAttackType = -1;
                }

                _currentAttackType++;
                _selectedAction = true;
                _selectedTargetPosition = target;
                _startingPosition = transform.position;
                _timer = 0;
                _shockwaveIterations = 0;
            }
        }
        else
        {
            if (_currentAttackType == 0)
            {

                transform.position = Vector3.Lerp(_startingPosition, _selectedTargetPosition - (_selectedTargetPosition - _startingPosition).normalized * _explosionSize, Mathf.Min(_timer / _jumpTime, 1));

                if(_timer > _jumpTime)
                {
                    CreateExplosion(_selectedTargetPosition);
                    _selectedAction = false;
                    //Delay before next action
                    _timer = -0.5f;
                }
            }
            else if (_currentAttackType == 1)
            {
                if(_timer > 1 + _shockwaveIterations * 0.15f)
                {
                    CreateShockwave(_selectedTargetPosition);
                    _shockwaveIterations++;
                    if(_shockwaveIterations > 2)
                    {
                        _selectedAction = false;
                        //Delay before next action
                        _timer -= 0.5f;
                    }
                }
            }
        }
    }

    private void CreateExplosion(Vector3 targetPosition)
    {
       // Instantiate(explosionObject, targetPosition, Quaternion.identity).GetComponent<ExplosionLogic>().InitialiseEffect(targetLayer, 5, 20, 0.5f, 1);

        GameObject newExplosion = _objectPoolManager.GetFreeObject("Explosion");

        newExplosion.transform.rotation = Quaternion.identity;
        newExplosion.transform.position = targetPosition;

        ExplosionLogic newShockwaveScript = newExplosion.GetComponent<ExplosionLogic>();
        newShockwaveScript.InitialiseEffect(_targetLayer, 3, 3, 0.5f, 0.7f, _objectPoolManager);
    }

    private void CreateShockwave(Vector3 targetPosition)
    {
        Vector3 rotation = targetPosition - transform.position;
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        GameObject newShockwave = _objectPoolManager.GetFreeObject("Shockwave");
        
        newShockwave.transform.rotation = Quaternion.Euler(0, 0, rotz);
        newShockwave.transform.position = transform.position;

        ShockwaveLogic newShockwaveScript = newShockwave.GetComponent<ShockwaveLogic>();
        newShockwaveScript.InitialiseEffect(_targetLayer, 1, rotation.normalized, 12, _objectPoolManager);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((1 << collision.gameObject.layer) == _targetLayer.value)
        {
            AddTarget(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == _targetLayer.value)
        {
            RemoveTarget(collision.gameObject);
        }
    }

    private void AddTarget(GameObject target)
    {
        _currentTargets.Add(target);
    }

    private void RemoveTarget(GameObject target)
    {
        _currentTargets.Remove(target);
    }

    private GameObject GetClosestTarget()
    {
        if (_targetIndex < 0)
        {
            return null;
        }

        GameObject closestObject = null;
        Vector3 currentPosition = transform.position;
        float currentDistance = 10000.0f;
        float testingDistance;

        foreach (GameObject target in _currentTargets)
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
        _currentHealth -= damage;
        _healthSlider.value = _currentHealth;
        StartCoroutine(DamageColor());
        if (_currentHealth < 0)
        {
            _alive = false;
            GetComponent<enemyScr>().DecreaseEnemyCount();
            StartCoroutine(WaitForRoomEnd());
        }
    }

    private IEnumerator WaitForRoomEnd()
    {
        yield return new WaitUntil(() => _room2.roomStart == false);
        yield return Defeated();
    }

    private IEnumerator Defeated()
    {
        if (!_displayVN)
        {
            _objectPoolManager.ReleaseObject("Mercy", gameObject);
            yield break;
        }

        _alive = false;
        VisualNovelScript visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
        visualNovelManager.StartNovelSceneByName("Miniboss tester");
        _selectedAction = false;
        _currentAttackType = 0;
        _shockwaveIterations = 0;
        _timer = 0.0f;
        yield return WaitForNovel();
        if (visualNovelManager.GetLastSelectionID() == 0)
        {
            JoinPlayer();
        }
        else
        {
            JoinBoss();
        }
        _displayVN = false;
    }

    IEnumerator WaitForNovel()
    {
        VisualNovelScript visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
        yield return new WaitWhile(() => visualNovelManager.isNovelSection == true);
    }

    public void JoinBoss()
    {
        this._companionMode = CompanionMode.Miniboss;
        _modeLayerSelected = false;
        _alive = true;
        // STOP USING FIND FUNCTIONS
        transform.position = new Vector3(95.0f, 0.0f, 0.0f);
        _idlePosition.position = new Vector3(95.0f, 0.0f, 0.0f);
        _currentHealth = 15;
        _healthSlider.value = _currentHealth;

        GetComponent<CircleCollider2D>().enabled = false;
        _currentTargets.Clear();
        GetComponent<CircleCollider2D>().enabled = true;
        GetClosestTarget();
    }

    public void JoinPlayer()
    {
        gameObject.layer = 0;
        _hitBoxObject.layer = 0;
        this._companionMode = CompanionMode.Companion;
        _idlePosition = GameObject.Find("PlayerProto").GetComponent<Transform>();
        _currentHealth = 15;
        _healthSlider.value = _currentHealth;
        //targetIndex = -1;
        _modeLayerSelected = false;
        _alive = true;
        _selectedAction = false;

        GetComponent<CircleCollider2D>().enabled = false;
        _currentTargets.Clear();
        GetComponent<CircleCollider2D>().enabled = true;
        GetClosestTarget();
    }

    IEnumerator DamageColor()
    {
        SpriteRenderer spriteRenderer = this.gameObject.transform.Find("Image").GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
