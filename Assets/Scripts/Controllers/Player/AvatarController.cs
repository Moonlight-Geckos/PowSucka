using UnityEngine;
public class AvatarController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float maxHealth = 100;

    private HealthBar _healthBar;
    private SkinController _skinController;
    private float _currentHP;
    private Observer _observer;

    private void Awake()
    {
        _currentHP = maxHealth;
        _observer = Observer.Instance;
        EventsPool.EnemyDiedEvent.AddListener(Heal);
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
        if (damage <= 0 || _currentHP <= 0)
            return;
        _currentHP -= damage;
        _healthBar.UpdateValue(_currentHP / 100f);
        _skinController.AnimateHit();
        if (_currentHP <= 0)
            EventsPool.GameFinishedEvent.Invoke(false);
    }
    private void Heal()
    {
        if (!_observer.Started)
            return;
        if(_currentHP < 100)
        {
            _currentHP+=4;
            _healthBar.UpdateValue(_currentHP / 100f);
        }
    }
    public void StopDamage() { }

}