using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField]
    protected float maxHealth = 5;

    [SerializeField]
    protected ProjectilePool[] projectilePools;

    [SerializeField]
    protected float projectileSpeed;

    [SerializeField]
    protected float shootingCooldown;

    [SerializeField]
    protected float aimDuration;

    [SerializeField]
    protected float zAdditionalRadius;

    [SerializeField]
    protected ParticlesPool explosionParticlesPool;

    [SerializeField]
    protected Mesh gizmosMesh;

    protected float _currenthealth;
    protected bool _isShooting = false;
    protected float _takingDamage = -1;
    protected bool _coroutineRunning = false;
    protected float _countdownCooldown;
    protected int _rand;
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;
    protected Transform _playerTransform;
    protected Vector3 _projectileDirection;
    protected Vector3 _position;
    protected Vector3 _originalScale;

    private float _suctionDistance;
    private float _curDis;
    private Vector3 _newScale;
    private Vector3 _direction;
    private Timer _damageTimer;

    private List<Material> _materials;
    private List<Color> _originalColors;

    protected bool _blackholed = false;

    private float _radius;
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _materials = new List<Material>();
        _originalColors = new List<Color>();

        foreach (Renderer rend in _animator.GetComponentsInChildren<Renderer>())
        {
            _materials.AddRange(rend.materials);
            foreach (Material material in rend.materials)
            {
                _originalColors.Add(material.color);
            }
        }
        _damageTimer = TimersPool.Pool.Get();
        _damageTimer.Duration = 0.5f;
        _damageTimer.AddTimerFinishedEventListener(() =>
        {
            GetDamage(_takingDamage);
        });

        _originalScale = transform.localScale;
        _curDis = GameManager.Instance.SuctionVelocity * Time.deltaTime;
    }
    private void Start()
    {
        Initialize();
    }
    private void Update()
    {
        if (_blackholed)
        {
            ScaleWithBlackhole();
            MoveToBlackhole();
            return;
        }
        if(Observer.playerMoving)
            _navMeshAgent.destination = _playerTransform.position;
        transform.LookAt(_playerTransform.position);
        if (_countdownCooldown > 0)
            _countdownCooldown -= Time.deltaTime;
        _radius = _navMeshAgent.stoppingDistance +
            Mathf.Min(Mathf.Abs(_playerTransform.position.z - transform.position.z) , zAdditionalRadius);
        if (_navMeshAgent.hasPath && _navMeshAgent.remainingDistance <= _radius)
        {
            StopChasing();
        }
        else if (_isShooting && _countdownCooldown <= 0 && !_coroutineRunning)
        {
            StartChasing();
        }
        if (_isShooting)
        {
            TryShoot();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Triggered(other);
    }
    protected void Triggered(Collider other)
    {
        if (other.gameObject.layer == StaticValues.BlackHoleLayer)
        {
            GetBlackholed();
        }
        else if (other.gameObject.layer == StaticValues.VacuumLayer && _blackholed)
        {
            EventsPool.EnemyBlackholedFinished.Invoke();
            Expire();
        }
    }
    protected abstract void StartChasing();
    protected abstract void StopChasing();
    protected abstract void TryShoot();
    protected virtual void Expire()
    {
        StopAllCoroutines();
        GetComponent<IDisposable>()?.Dispose();
        EventsPool.EnemyDiedEvent.Invoke();
    }
    protected virtual void Explode()
    {
        explosionParticlesPool?.Pool.Get().transform.SetPositionAndRotation(transform.position, transform.rotation);
        Expire();
    }
    public virtual void Initialize()
    {
        _isShooting = false;
        _blackholed = false;
        _currenthealth = maxHealth;
        _takingDamage = -1;
        _countdownCooldown = 0;
        _coroutineRunning = false;
        _navMeshAgent.isStopped = false;
        _playerTransform = GameManager.Instance.PlayerTransform;
        _navMeshAgent.destination = _playerTransform.position;
        transform.localScale = _originalScale;
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].color = _originalColors[i];
        }
    }
    protected virtual void GetBlackholed()
    {
        if (_blackholed)
            return;

        StopAllCoroutines();
        EventsPool.EnemyBlackholed.Invoke();
        _blackholed = true;
        _suctionDistance = Vector3.Distance(transform.position, GameManager.Instance.PlayerTransform.position);
    }
    public void GetDamage(float damage, float cooldown = -1)
    {
        if (damage == -1 || _currenthealth <= 0)
            return;
        _currenthealth-=damage;

        _countdownCooldown = 0.5f;
        _takingDamage = damage;

        if (_currenthealth <= 0)
            Explode();
        else
        {
            AnimateHit();
            if(cooldown >= 0)
                _damageTimer.Duration = cooldown;
            _damageTimer.Run();
        }
    }
    public void StopDamage()
    {
        _takingDamage = -1;
    }
    protected void ScaleWithBlackhole()
    {
        _newScale = Vector3.Lerp(
            Vector3.one,
            Vector3.zero,
            Mathf.Clamp(
                1.2f - (Vector3.Distance(transform.position, GameManager.Instance.VacuumTransform.position) / _suctionDistance),
                0, 1)
            );
        if (_newScale.x < transform.localScale.x) transform.localScale = _newScale;
        if (_newScale.x < 0.1f)
            Expire();
    }
    protected void MoveToBlackhole()
    {
        _direction = (GameManager.Instance.PlayerTransform.position - transform.position);
        _direction.y = 0;
        _direction = (Quaternion.Euler(0, -55, 0) * _direction).normalized;
        transform.Translate(_direction * _curDis, Space.World);
        transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }
    protected IEnumerator GetHitCoroutine()
    {
        yield return null;
        float duration = 0.3f;
        float elapsed = 0;
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].color = Color.white;
        }
        while (elapsed < duration)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                _materials[i].color = Color.Lerp(Color.white, _originalColors[i], elapsed / duration);
            }
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].color = _originalColors[i];
        }
    }
    protected void AnimateHit()
    {
        StopCoroutine(GetHitCoroutine());
        StartCoroutine(GetHitCoroutine());
    }
    private void OnDrawGizmosSelected()
    {
        if (gizmosMesh == null)
            return;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Gizmos.DrawWireMesh(gizmosMesh, transform.position, Quaternion.identity, 2 * Vector3.one * _navMeshAgent.stoppingDistance + new Vector3(0, 0, zAdditionalRadius));
    }
}
