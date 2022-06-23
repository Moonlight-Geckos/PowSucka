using System.Collections.Generic;
using UnityEngine;
public class AvatarController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float maxHealth = 100;

    private HealthBar _healthBar;
    private SkinController _skinController;
    private Observer _observer;
    private Timer _damageTimer;
    private float _currentHP;
    private List<float> _takingDamage;
    private float _extraHealth;

    private void Awake()
    {
        _takingDamage = new List<float>();
        _damageTimer = TimersPool.Pool.Get();
        _damageTimer.AddTimerFinishedEventListener(() => {
            foreach (float y in _takingDamage)
                GetDamage(y);
            _damageTimer.Run();
        });
        _currentHP = maxHealth;
        _observer = Observer.Instance;
        EventsPool.EnemyDiedEvent.AddListener(Heal);
        EventsPool.GameStartedEvent.AddListener(() =>
        {
            _extraHealth = PlayerStorage.HealthUpgradeLevel * 2;
            _currentHP += _extraHealth;
            maxHealth += _extraHealth;
        });
    }

    private void OnEnable()
    {
        Initialize();
    }
    private void Initialize()
    {
        _skinController = GetComponentInChildren<SkinController>();
        _healthBar = FindObjectOfType<HealthBar>();
    }
    public void GetDamage(float damage, float cooldown = -1)
    {
        if (damage <= 0 || _currentHP <= 0 || !_observer.Started)
            return;
        _currentHP -= damage;
        _healthBar?.UpdateValue(_currentHP / maxHealth);
        _skinController.AnimateHit();
        if (_currentHP <= 0)
        {
            EventsPool.GameFinishedEvent.Invoke(false);
            return;
        }
        else if (cooldown > -1)
        {
            _takingDamage.Add(damage);
            _damageTimer.Duration = cooldown;
            _damageTimer.Run();
        }
    }
    private void Heal()
    {
        if (!_observer.Started)
            return;
        if(_currentHP < maxHealth)
        {
            _currentHP+=4;
            _healthBar?.UpdateValue(_currentHP / maxHealth);
        }
    }
    public void StopDamage(float damage) 
    {
        _takingDamage.Remove(damage);
    }

}