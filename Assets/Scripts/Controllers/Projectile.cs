using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float damage;

    [SerializeField]
    protected float durationToExpire;

    protected Rigidbody _rb;
    protected Animator _animator;
    protected TrailRenderer _trailRenderer;

    private IDisposable _disposable;
    private Timer _timer;
    public virtual void Shoot(Vector3 velocity)
    {
        ResetTrailer(true);
        transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        _rb.velocity = velocity;
        _timer.Run();
    }
    public virtual void Initialize(Vector3 position)
    {
        if (!_rb)
        {
            _rb = GetComponent<Rigidbody>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _animator = GetComponentInChildren<Animator>();
            _disposable = GetComponent<IDisposable>();
            _timer = TimersPool.Pool.Get();
            _timer.Duration = durationToExpire;
            _timer.AddTimerFinishedEventListener(() =>
            {
                GetComponent<IDisposable>()?.Dispose();
                ResetTrailer(false);
            });
        }
        _rb.velocity = Vector3.zero;
        transform.position = position;
    }

    public virtual void ResetTrailer(bool activation)
    {
        if (_trailRenderer == null)
            return;
        if(!activation)
            _trailRenderer.Clear();
        _trailRenderer.emitting = activation;
    }
    public virtual void Expire()
    {
        _timer?.Stop();
        ResetTrailer(false);
        _disposable?.Dispose();
    }
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamagable>()?.GetDamage(damage);
        Expire();
    }
}
