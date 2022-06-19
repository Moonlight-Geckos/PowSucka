using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleRocket : Enemy
{
    [SerializeField]
    private float arcDistance = 10f;

    [SerializeField]
    private Transform startTargetLeft;

    [SerializeField]
    private Transform startTargetRight;

    private Projectile _projectileLeft;
    private Projectile _projectileRight;
    private Vector3 _forwardPoint;
    protected override void StartChasing()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = _playerTransform.position;
        _isShooting = false;
        _animator.SetBool("Idle", _isShooting);
    }
    protected override void StopChasing()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _isShooting = true;
        _animator.SetBool("Idle", _isShooting);
    }
    protected override void TryShoot()
    {
        if (_countdownCooldown > 0 || _coroutineRunning)
            return;
        IEnumerator shoot()
        {
            _coroutineRunning = true;
            yield return new WaitForSeconds(_animator.GetNextAnimatorStateInfo(0).length);
            _animator.SetTrigger("Aim");
            yield return new WaitForSeconds(aimDuration + 0.2f);
            _animator.SetTrigger("Shoot");

            _rand = Random.Range(0, projectilePools.Length);

            _projectileLeft = projectilePools[_rand].Pool.Get();
            _projectileLeft.Initialize(startTargetLeft.position);

            _projectileRight = projectilePools[_rand].Pool.Get();
            _projectileRight.Initialize(startTargetRight.position);

            _projectileDirection = _playerTransform.position - transform.position;
            _projectileDirection.y = 0;

            _forwardPoint = transform.forward * (Vector3.Distance(_observer.PlayerTransform.position, transform.position) / 2);

            _projectileRight.Shoot((_projectileDirection + transform.right * 40).normalized * projectileSpeed,
                new Bezier(_projectileRight.transform.position,
                _projectileRight.transform.position + _forwardPoint + (transform.right * arcDistance),
                _observer.PlayerTransform.position));
            _projectileLeft.Shoot((_projectileDirection - transform.right * 40).normalized * projectileSpeed,
                new Bezier(_projectileRight.transform.position,
                _projectileRight.transform.position + _forwardPoint - (transform.right * arcDistance),
                _observer.PlayerTransform.position));

            _countdownCooldown = shootingCooldown;
            yield return new WaitForSeconds(_animator.GetNextAnimatorStateInfo(0).length);
            _coroutineRunning = false;
        }
        StartCoroutine(shoot());
    }
}
