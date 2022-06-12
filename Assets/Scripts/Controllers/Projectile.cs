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
        ResetTrailer(true);
        transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        _rb.velocity = velocity;
        _velocityMagnitude = velocity.magnitude;
        _bezierCurve = bezierCurve;
        elapsedTime = 0;
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
            _timer.AddTimerFinishedEventListener(Expire);
        }
        _rb.velocity = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.position = position;
        _sucked = false;
        _bezierCurve = null;

        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = 1;
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
    public virtual void EmitParticles(Vector3 pos)
    {
        if (explosionParticlesPool != null)
        {
            var ps = explosionParticlesPool.Pool.Get();
            ps.Initialize(true);
            ps.transform.position = pos;
        }
    }
    public virtual void GetSucked()
    {
        if (_sucked || !_timer.Running)
            return;
        _sucked = true;
        _suctionDistance = Vector3.Distance(transform.position, GameManager.PlayerTransform.position);
    }
    public virtual void Triggered(Collider other)
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
                other.GetComponent<IDamagable>()?.GetDamage(damage);
                EmitParticles(transform.position);
                _timer.Stop();
            }
            Expire();
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
            transform.localScale = Vector3.Lerp(
                Vector3.one,
                Vector3.zero,
                Mathf.Clamp(
                    1 - Vector3.Distance(transform.position, GameManager.PlayerTransform.position) / _suctionDistance,
                    0, 1)
                );
            if (_trailRenderer != null)
                _trailRenderer.widthMultiplier = transform.localScale.x;
        }
    }
    private void FixedUpdate()
    {
        if (_rb == null)
            return;
        if (_sucked)
        {
            _rb.velocity = GameManager.Instance.SuctionVelocity * (GameManager.PlayerTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized, Vector3.up);
        }
        else if(_bezierCurve != null)
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
    }
}
