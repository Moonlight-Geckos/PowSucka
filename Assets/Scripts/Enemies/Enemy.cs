using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour
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
    protected Mesh gizmosMesh;

    protected bool _isShooting = false;
    protected bool _coroutineRunning = false;
    protected float countdownCooldown;
    protected int rand;
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;
    protected Transform _playerTransform;
    protected Vector3 _projectileDirection;
    protected Vector3 _position;

    private float _radius;
    private void Start()
    {
        Initialize();
    }
    private void Update()
    {
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
    protected abstract void StartChasing();
    protected abstract void StopChasing();
    protected abstract void TryShoot();
    public virtual void Initialize()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _isShooting = false;
        countdownCooldown = 0;
        _navMeshAgent.isStopped = false;
        _playerTransform = GameManager.PlayerTransform;
        _navMeshAgent.destination = _playerTransform.position;
    }
    private void OnDrawGizmosSelected()
    {
        if (gizmosMesh == null)
            return;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Gizmos.DrawWireMesh(gizmosMesh, transform.position, Quaternion.identity, 2 * Vector3.one * _navMeshAgent.stoppingDistance + new Vector3(0, 0, zAdditionalRadius));
    }
}
