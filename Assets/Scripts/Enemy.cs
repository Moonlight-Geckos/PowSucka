using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float shootingCooldown;

    private Animator _animator;
    private Transform _playerTransform;
    private bool _isShooting = false;
    private NavMeshAgent _navMeshAgent;
    private float countdownCooldown;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _isShooting = false;
        countdownCooldown = 0;
        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = _playerTransform.position;
    }
    private void Update()
    {
        transform.LookAt(_playerTransform.position);
        if (countdownCooldown > 0)
            countdownCooldown -= Time.deltaTime;

        _navMeshAgent.destination = _playerTransform.position;
        if (_navMeshAgent.hasPath && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            StopChasing();
        }
        else if(_isShooting && countdownCooldown <= 0)
        {
            StartChasing();
        }
        if (_isShooting)
        {
            TryShoot();
        }
    }
    void StartChasing()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = _playerTransform.position;
        _isShooting = false;
        _animator.SetBool("Idle", _isShooting);
    }
    void StopChasing()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _isShooting = true;
        _animator.SetBool("Idle", _isShooting);
    }

    void TryShoot()
    {
        if (countdownCooldown > 0)
            return;
        _animator.SetTrigger("Shoot");
        Debug.DrawLine(_playerTransform.position, transform.position, Color.red, 0.5f);
        countdownCooldown = shootingCooldown;
    }
    private void OnDrawGizmos()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Gizmos.DrawWireSphere(transform.position, _navMeshAgent.stoppingDistance);
    }
}
