using System.Collections;
using UnityEngine;

public class Bomber : Enemy
{
    [SerializeField]
    private Transform startBombTransform;

    private Projectile _projectile;
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
            _projectile = projectilePools[_rand].Pool.Get();
            _projectile.Initialize(startBombTransform.position);
            yield return new WaitForSeconds(aimDuration + 0.2f);
            _animator.SetTrigger("Shoot");

            _rand = Random.Range(0, projectilePools.Length);

            _projectileDirection = _playerTransform.position - transform.position;
            _projectileDirection.y = 0;
            _projectile.Shoot(_projectileDirection.normalized * projectileSpeed);

            _countdownCooldown = shootingCooldown;
            yield return new WaitForSeconds(_animator.GetNextAnimatorStateInfo(0).length);
            _coroutineRunning = false;
        }
        StartCoroutine(shoot());
    }
    protected override void Expire()
    {
        if ( _projectile != null && _projectile.gameObject.activeSelf && !_projectile.Running)
            _projectile.Expire();
        base.Expire();
    }
    protected override void GetBlackholed()
    {
        base.GetBlackholed();
        if (_projectile != null && _projectile.gameObject.activeSelf && !_projectile.Running)
            _projectile.Expire();
    }
}
