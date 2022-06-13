using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float damage;

    [SerializeField]
    protected float durationToExpire;

    [SerializeField]
    protected float arcRadius = 4;

    [SerializeField]
    protected ParticlesPool explosionParticlesPool;

    protected Rigidbody _rb;
    protected Animator _animator;
    protected TrailRenderer _trailRenderer;
    protected Timer _timer;
    protected Bezier _bezierCurve;
    protected bool _sucked = false;
    private IDisposable _disposable;

    #region Curving
    Vector3 nextPos;
    float elapsedTime;
    #endregion

    private float _velocityMagnitude;
    private float _suctionDistance;
    public virtual void Shoot(Vector3 velocity, Bezier bezierCurve = null)
    {
        transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        _rb.velocity = velocity;
        _velocityMagnitude = velocity.magnitude;
        _bezierCurve = bezierCurve;
        elapsedTime = 0;
        _timer.Run();
        ResetTrailer(true);
        if (_animator != null)
            _animator.speed = 1;
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
            _timer.AddTimerFinishedEventListener(TimerFinished);
        }
        ResetProjectile();
        transform.position = position;
    }
    protected virtual void ResetProjectile()
    {
        _rb.velocity = Vector3.zero;
        transform.localScale = Vector3.one;
        _sucked = false;
        _bezierCurve = null;
        ResetTrailer(false);
        if(_animator != null)
            _animator.speed = 0;
    }
    protected virtual void ResetTrailer(bool activation)
    {
        if (_trailRenderer == null)
            return;
        _trailRenderer.Clear();
        _trailRenderer.widthMultiplier = 1;
        _trailRenderer.emitting = activation;
    }
    protected virtual void Expire()
    {
        _timer?.Stop();
        _disposable?.Dispose();
        ResetProjectile();
    }
    protected virtual void TimerFinished()
    {
        Explode(true);
    }
    protected virtual void Explode(bool shouldDamage)
    {
        EmitParticles(transform.position, shouldDamage);
        Expire();
    }
    protected virtual void EmitParticles(Vector3 pos, bool shouldDamage)
    {
        if (explosionParticlesPool != null)
        {
            var ps = explosionParticlesPool.Pool.Get();
            ps.Initialize(shouldDamage ? damage : 0);
            ps.transform.position = pos;
        }
    }
    protected virtual void GetSucked()
    {
        if (_sucked || !_timer.Running)
            return;
        _sucked = true;
        _suctionDistance = Vector3.Distance(transform.position, GameManager.Instance.PlayerTransform.position);
    }
    protected abstract void HitPlayer(Collider other);
    protected virtual void ScaleWithVacuum()
    {
        transform.localScale = Vector3.Lerp(
            Vector3.one,
            Vector3.zero,
            Mathf.Clamp(
                1.2f - (Vector3.Distance(transform.position, GameManager.Instance.PlayerTransform.position) / _suctionDistance),
                0, 1)
            );
        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = transform.localScale.x;
    }
    protected virtual void FollowVacuum()
    {
        _rb.velocity = GameManager.Instance.SuctionVelocity * (GameManager.Instance.PlayerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized, Vector3.up);
    }
    protected virtual void CurveProjectile()
    {
        nextPos = _bezierCurve.GetPoint(elapsedTime);
        if (elapsedTime > 1)
        {
            _rb.velocity = _velocityMagnitude * transform.forward;
            _bezierCurve = null;
            return;
        }
        elapsedTime += Time.fixedDeltaTime;
        _rb.velocity = _velocityMagnitude * (nextPos - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized, Vector3.up);
    }
    protected virtual void Triggered(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            return;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Vacuum"))
        {
            GetSucked();
        }
        else
        {
            if (!_sucked)
            {
                HitPlayer(other);
            }
            else
            {
                EventsPool.PickedupProjectileEvent.Invoke(this);
                Expire();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Triggered(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vacuum"))
            GetSucked();
    }
    private void Update()
    {
        if (_sucked)
        {
            ScaleWithVacuum();
        }
    }
    private void FixedUpdate()
    {
        if (_rb == null)
            return;
        if (_sucked)
        {
            FollowVacuum();
        }
        else if(_bezierCurve != null)
        {
            CurveProjectile();
        }
    }
}