using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float shootingCooldown;

    [SerializeField]
    private float movementSpeed;

    private Animator _animator;
    private Transform _playerTransform;
    private bool _isShooting = false;
    private NavMeshAgent _navMeshAgent;
    private float countdownCooldown;

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = movementSpeed;
        _navMeshAgent.destination = _playerTransform.position;
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        transform.LookAt(_playerTransform.position);
        _navMeshAgent.destination = _playerTransform.position;
        if (countdownCooldown > 0)
            countdownCooldown -= Time.deltaTime;
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;
            _isShooting = true;
            _animator.SetBool("Idle", _isShooting);
        }
        else if(_isShooting && countdownCooldown <= 0)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.destination = _playerTransform.position;
            _isShooting = false;
            _animator.SetBool("Idle", _isShooting);
        }
        if (_isShooting)
        {
            if (countdownCooldown <= 0)
            {
                Shoot();
            }
        }
    }
    void Shoot()
    {
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
