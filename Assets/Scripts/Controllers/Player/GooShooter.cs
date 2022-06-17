public class GooShooter : PlayerShooter
{
    protected override void Shoot()
    {
        _projectile = projectilePool.Pool.Get();
        _projectile.Initialize(transform.position);
        _projectile.Shoot(transform.forward * projectileVelocity);

        _timer.Run();
    }
}
