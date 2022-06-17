using UnityEngine;

public abstract class PlayerShooter : MonoBehaviour
{
    [SerializeField]
    protected float projectileVelocity;

    [SerializeField]
    protected float shootCooldown;

    [SerializeField]
    protected ProjectilePool projectilePool;

    protected Timer _timer;
    protected Projectile _projectile;
    protected Bezier _bezier;
    private void Awake()
    {
        _timer = TimersPool.Pool.Get();
        _timer.Duration = shootCooldown;
        _timer.AddTimerFinishedEventListener(Shoot);
    }
    private void OnDisable()
    {
        _timer.Stop();
    }
    private void OnEnable()
    {
        if (_timer != null && !_timer.Running)
            _timer.Run();
    }
    protected abstract void Shoot();
}
