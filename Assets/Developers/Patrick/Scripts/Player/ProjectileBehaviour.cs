using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Objects
    [SerializeField] private GameObject _bulletImage;
    [SerializeField] private GameObject _chargeImage;
    private ObjectPoolManager _objectPoolManager;
    private CompanionManager _companionManager;

    // Values
    private Vector3 _originalScale = new Vector3(1, 1, 1);
    private int _originalDamage = 1;

    private string _objectName;
    private bool _canBeReleased = false;
    private float _lifeSpan = 0.0f;
    private float _minLife = 0.05f;
    private int _damage = 1;

    private bool _hasCompanion = false;

    public void InitialiseComponent(Vector2 moveDirection, float speed, ref ObjectPoolManager dPoolManager, string prefabName, float damageMultiplier, int ammoUsed)
    {
        GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
        _objectPoolManager = dPoolManager;

        _objectName = prefabName;
        _lifeSpan = Time.time;
        _canBeReleased = false;

        _bulletImage.SetActive(true);

        transform.localScale = _originalScale;
        _damage = _originalDamage;
        if (damageMultiplier != 1) // Calculate damage modifications
        {
            Debug.Log(damageMultiplier);
            Debug.Log(ammoUsed);
            transform.localScale = _originalScale * (1 + (0.12f * ammoUsed));
            //transform.localScale = _originalScale * (1 + (damageMultiplier * 0.05f));
            _damage = (int)(_originalDamage * damageMultiplier);

            _bulletImage.SetActive(false);
            _chargeImage.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_canBeReleased)
        {
            if(Time.time - _lifeSpan > _minLife)
            {
                _chargeImage.SetActive(false);
                _objectPoolManager.ReleaseObject(_objectName, this.gameObject);
            }
        }
    }

    public void SetToRelease()
    {
        _canBeReleased = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9) // Enemy layer
        {
            if(collision.gameObject.tag == "Companion")
            {
                // Damages the companion if they're not friendly
                if (collision.gameObject.GetComponentInParent<CompanionManager>().IsFriendly())
                {
                    return;
                }
                collision.gameObject.GetComponentInParent<CompanionManager>().TakeDamage(_damage);
            }
            else if (collision.gameObject.tag == "Boss") // Damage Boss
            {
                collision.gameObject.GetComponent<IchorManager>().takeDamage(_damage);
            }
            else if (collision.gameObject.tag == "WeakPoint") // Damage weak point
            {
                collision.gameObject.GetComponent<WeakPointScript>().takeDamage(_damage);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null) // Damage basic enemy
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(_damage, gameObject.transform.position - collision.gameObject.transform.position);
            }

            // Set targetting for ally companion if exists
            if (_hasCompanion)
            {
                if (collision.gameObject.GetComponent<CompanionTargettingHandler>() != null)
                {
                    collision.gameObject.GetComponent<CompanionTargettingHandler>().InitialiseTargetting(ref _companionManager);
                }
            }

            // Set projectile to be released
            _lifeSpan = _minLife;
            _canBeReleased = true;
        }
    }

    // Setup for the projectile to carry out the targetting
    public void AddCompanionTargetting(ref CompanionManager companionManager)
    {
        _hasCompanion = true;
        _companionManager = companionManager;
    }
}
