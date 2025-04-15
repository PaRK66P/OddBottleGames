using System;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("NECESSARY SCENE OBJECTS")]
    [SerializeField]
    private ObjectPoolManager _objectPoolManager;
    [SerializeField]
    private GameObject _canvasUI;
    [SerializeField]
    private SoundManager _soundManager;
    [SerializeField]
    private TimeManager _timeManager;

    [Header("NECESSARY PREFABS")]
    [SerializeField]
    private PlayerData _playerData;
    [SerializeField]
    private PlayerDebugData _playerDebugData;

    [Header("NECESSARY INTERNAL OBJECTS")]
    [SerializeField]
    private GameObject _canvasPlayer;
    [SerializeField]
    private GameObject _evolveDashCollider;
    [SerializeField]
    private PlayerAnimationHandler _playerAnimationHandler;
    [SerializeField]
    private PlayerAimReticle _playerAimReticle;

    // Objects
    private GameObject _healthbar;
    private HealthBarScript _healthBarScript;
    private CompanionManager _companionManager;

    // Components
    private PlayerInputManager _playerInputManager;
    private PlayerMovement _playerMovement;
    private InteractComponent _playerInteract;
    private PlayerShooting _playerShooting;
    private Rigidbody2D _rigidbody;

    // Values
    private bool _hasCompanion = false;
    private bool _isDashing = false;
    private bool _isDamaged = false;

    private float _timeOfDamage = -10.0f;
    private float _invulnerableTime = 1.0f;
    private float _regenTimer = 0.0f;
    private float _health = 100;

    // Dialogue System
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private bool _fadeIn = false;
    [SerializeField]
    private bool _fadeOut = false;
    [SerializeField]
    private float _fadeTextTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _playerInputManager = gameObject.AddComponent<PlayerInputManager>();
        _playerMovement = gameObject.AddComponent<PlayerMovement>();
        _playerShooting = gameObject.AddComponent<PlayerShooting>();
        _playerInteract = gameObject.AddComponent<InteractComponent>();

        _evolveDashCollider.GetComponent<EvolveDashDamage>().InitialiseScript(ref _playerData);
        _evolveDashCollider.SetActive(false);

        _health = _playerData.Health;
        _healthbar = Instantiate(_playerData.Healthbar, _canvasUI.transform.Find("PlayerUI"));
        _healthBarScript = _healthbar.GetComponent<HealthBarScript>();
        _healthBarScript.SetMaxHealth(_health);
        _healthBarScript.SetValue(_health);

        PlayerManager manager = this; // Reference for the manager
        _playerMovement.InitialiseComponent(ref manager, ref _playerShooting, ref _playerAnimationHandler, ref _playerData, ref _playerDebugData, ref _soundManager, ref _healthBarScript, ref _evolveDashCollider);
        _playerShooting.InitialiseComponent(ref _playerData, ref _playerDebugData, ref _playerMovement, ref _playerAnimationHandler, ref _objectPoolManager, ref _timeManager, ref _soundManager, ref _canvasPlayer);
        _playerInputManager.InitialiseComponent(ref _playerMovement, ref _playerShooting, ref _playerAimReticle);

        _canvasGroup = _canvasPlayer.transform.Find("FadeInOutGroup").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDamaged)
        {
            if(Time.time - _timeOfDamage >= _playerData.ControlLossTime) // Exceeded the time for player to not have control
            {
                _playerAnimationHandler.EndDamageAnimation();
                _playerInputManager.EnableInput();
            }
            if(Time.time - _timeOfDamage > _invulnerableTime) // We've exceeded the time for being invulnerable
            {
                _rigidbody.excludeLayers = 0;
                _isDamaged = false;
            }
        }

        // Regeneration
        _regenTimer += Time.deltaTime;
        if(_regenTimer >= 1.0f)
        {
            Heal(1.0f);
            _regenTimer = 0; 
        }


        // Dialogue
        if (_fadeIn)
        {
            _canvasGroup.alpha += Time.deltaTime;
            if (_canvasGroup.alpha >= 1.0f)
            {
                _fadeIn = false;
            }
        }
        if (_fadeOut && !_fadeIn)
        {
            _canvasGroup.alpha -= Time.deltaTime;
            if (_canvasGroup.alpha <= 0.0f)
            {
                _fadeOut = false;
            }
        }
        if (_canvasGroup.alpha >= 1.0f)
        {
            _fadeTextTimer += Time.deltaTime;
        }

        if (_fadeTextTimer > 3.0f)
        {
            _fadeOut = true;
        }

    }

    #region Health
    // Damage function
    /*
     * damageDirection - Direction for knockback
     * damageTime = How long the invulnerable time will last
     * knockbackScalar - Scalar for the knockback force
     * ammount - Amount of damage
     */
    public void TakeDamage(Vector2 damageDirection, float damageTime = 1.0f, float knockbackScalar = 10.0f, float ammount = 10)
    {
        if (!CanBeDamaged()) { return; }

        // Update values
        _rigidbody.excludeLayers = _playerData.DamageLayers; // Prevent collisions whilst taking damage
        _isDamaged = true;
        _timeOfDamage = Time.time;
        _invulnerableTime = damageTime;
        _fadeOut = true;
        _playerMovement.KnockbackPlayer(damageDirection, knockbackScalar); // Knockback
        _playerAnimationHandler.StartDamageAnimation();

        // Remove control
        _playerInputManager.DisableInput();
        _playerShooting.InterruptFiring();

        // Apply damage
        _health -= ammount;
        if(_health <= 20.0f)
        {
            _health = 20.0f;
        }
        _healthBarScript.SetValue(_health);

        // Impact frames
        _timeManager.AddTimescaleForDuration(_playerData.DamageImpactFrameScale, _playerData.DamageImpactFrameDuration * (ammount / 10.0f));
    }

    // Restore health
    public void Heal(float ammount)
    {
        _health += ammount;
        if(_health >= _playerData.Health)
        {
            _health = _playerData.Health;
        }
        _healthBarScript.SetValue(_health);
    }

    // Updates condition value
    public void SetDashInvulnerability(bool invulnerability)
    {
        _isDashing = invulnerability;
    }

    public bool CanBeDamaged()
    {
        if (_isDamaged || _isDashing) { return false; }
        return true;
    }
    #endregion

    #region Input
    public void DisableInput()
    {
        _playerInputManager.DisableInput();
    }

    public void EnableInput()
    {
        _playerInputManager.EnableInput();
    }
    #endregion

    public void StartFadeInSpeech(string text)
    {
        _fadeIn = true;
        _fadeTextTimer = 0.0f;
        _canvasGroup.gameObject.GetComponentInChildren<TMP_Text>().text = text;
    }

    // Player Settings
    public void ChangeDashToggle(bool toggle)
    {
        _playerDebugData.CanDashTowardsMouse = toggle;
    }

    #region Movement
    public void EvolveDash(bool toggle)
    {
        _playerMovement.EvolveDash(toggle);
        _playerMovement.RechargeDashes();
    }

    public void GainDashCharges()
    {
        _playerMovement.RechargeDashes();
    }
    #endregion

    
    public bool isInteracting()
    {
        return _playerInteract.GetInteract();
    }

    #region Companion
    // Sets the ally companion in the player system
    public void SetAllyCompanion(bool isAdding, ref CompanionManager companionManager)
    {
        _hasCompanion = isAdding;
        _companionManager = companionManager;

        _playerShooting.UpdateCompanionData(_hasCompanion, ref _companionManager);
    }

    // Teleports ally companions to the player's position
    public void ReturnAllyCompanions()
    {
        if (_hasCompanion)
        {
            _companionManager.TeleportToPosition(transform.position);
            _companionManager.ChangeToIdleForTime(1.0f);
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _playerData.DashDistance);
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position + Vector3.right * _playerData.DashDistance, transform.position + Vector3.right * (_playerData.DashDistance + _playerData.EvolvedDashExtraDistance));
    }
    #endregion
}
