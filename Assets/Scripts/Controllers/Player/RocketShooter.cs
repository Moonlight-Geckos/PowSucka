using UnityEngine;

public class RocketShooter : PlayerShooter
{
    protected override void Shoot()
    {
        _projectile = projectilePool.Pool.Get();
        _projectile.Initialize(transform.position);
        _projectile.Shoot((transform.forward + (transform.right * Mathf.Sin(Time.timeSinceLevelLoad * Random.Range(-0.03f, 0.03f)))) * projectileVelocity);

        _timer.Run();
    }
}
