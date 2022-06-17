using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyAnimator))]
public abstract class Enemy : MonoBehaviour, IDamagable
{
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

    protected bool _isShooting = false;
    protected bool _coroutineRunning = false;
    protected float countdownCooldown;
    protected int rand;
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;
    protected EnemyAnimator _enemyAnimator;
    protected Transform _playerTransform;
    protected Vector3 _projectileDirection;
    protected Vector3 _position;

    protected bool _blackholed = false;

    private float _radius;
    private void Start()
    {
        Initialize();
    }
    private void Update()
    {
        if (_blackholed)
            return;
        transform.LookAt(_playerTransform.position);
        if (countdownCooldown > 0)
            countdownCooldown -= Time.deltaTime;

        _navMeshAgent.destination = _playerTransform.position;
        _radius = _navMeshAgent.stoppingDistance +
            Mathf.Min(Mathf.Abs(_playerTransform.position.z - transform.position.z) , zAdditionalRadius);
        if (_navMeshAgent.hasPath && _navMeshAgent.remainingDistance <= _radius)
        {
            StopChasing();
        }
        else if (_isShooting && countdownCooldown <= 0 && !_coroutineRunning)
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
        if (other.gameObject.layer == StaticValues.MinigunLayer
            || other.gameObject.layer == StaticValues.LaserLayer
            || other.gameObject.layer == StaticValues.FlamesLayer)
        {
            GetDamage(0);
        }
        else if(other.gameObject.layer == StaticValues.BlackHoleLayer)
        {
            GetBlackholed();
        }
        else if(other.gameObject.layer == StaticValues.VacuumLayer && _blackholed)
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
    }
    protected virtual void Explode()
    {
        explosionParticlesPool?.Pool.Get().transform.SetPositionAndRotation(transform.position, transform.rotation);
        Expire();
    }
    public virtual void Initialize()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _enemyAnimator = GetComponent<EnemyAnimator>();
        _enemyAnimator.enabled = false;
        _isShooting = false;
        _blackholed = false;
        countdownCooldown = 0;
        _coroutineRunning = false;
        _navMeshAgent.isStopped = false;
        _playerTransform = GameManager.Instance.PlayerTransform;
        _navMeshAgent.destination = _playerTransform.position;
    }
    protected virtual void GetBlackholed()
    {
        if (_blackholed || _enemyAnimator == null)
            return;

        StopAllCoroutines();
        EventsPool.EnemyBlackholed.Invoke();
        _blackholed = true;
        _enemyAnimator.SuctionDistance = Vector3.Distance(transform.position, GameManager.Instance.PlayerTransform.position);
        _enemyAnimator.enabled = true;
    }
    public void GetDamage(float damage)
    {
        Explode();
    }
    private void OnDrawGizmosSelected()
    {
        if (gizmosMesh == null)
            return;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Gizmos.DrawWireMesh(gizmosMesh, transform.position, Quaternion.identity, 2 * Vector3.one * _navMeshAgent.stoppingDistance + new Vector3(0, 0, zAdditionalRadius));
    }
}
