using System.Collections;
using UnityEngine;

public class Shooter : Enemy
{
    [SerializeField]
    private Transform startBulletTransform;

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
        if (countdownCooldown > 0 || _coroutineRunning)
            return;
        IEnumerator shoot()
        {
            _coroutineRunning = true;
            yield return new WaitForSeconds(_animator.GetNextAnimatorStateInfo(0).length);
            _animator.SetTrigger("Aim");
            _projectile = projectilePools[rand].Pool.Get();
            _projectile.Initialize(startBulletTransform.position);
            yield return new WaitForSeconds(aimDuration + 0.2f);
            _animator.SetTrigger("Shoot");

            rand = Random.Range(0, projectilePools.Length);

            _projectileDirection = _playerTransform.position - startBulletTransform.position;
            _projectileDirection.y = 0;
            _projectile.Shoot(_projectileDirection.normalized * projectileSpeed);

            countdownCooldown = shootingCooldown;
            yield return new WaitForSeconds(_animator.GetNextAnimatorStateInfo(0).length);
            _coroutineRunning = false;
        }
        StartCoroutine(shoot());
    }
}
