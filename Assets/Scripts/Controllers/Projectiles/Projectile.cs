using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ProjectileAnimator))]
public abstract class Projectile : MonoBehaviour, ISuckable
{
    [SerializeField]
    protected FillType fillType;

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
    protected ProjectileAnimator _prjAnimator;
    protected Timer _timer;
    protected Bezier _bezierCurve;
    private IDisposable _disposable;
    private Vector3 _newScale;

    protected bool _sucked = false;
    protected bool _blackholed = false;
    protected bool _shouldSuck = false;
    protected bool _shouldBlackhole = false;
    protected Observer _observer;
    protected float _damageLevel;

    #region Curving
    Vector3 nextPos;
    float elapsedTime;
    #endregion

    private float _velocityMagnitude;
    private float _suctionDistance;

    public bool Running
    {
        get { return _rb.velocity.sqrMagnitude > 0; }
    }


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
            _prjAnimator = GetComponent<ProjectileAnimator>();
}
        ResetProjectile();
        transform.position = position;
    }
    protected virtual void ResetProjectile()
    {
        _rb.velocity = Vector3.zero;
        transform.localScale = Vector3.one;
        _sucked = false;
        _blackholed = false;
        _shouldSuck = false;
        _shouldBlackhole = false;
        _bezierCurve = null;
        _prjAnimator.ToVacuum = false;
        _prjAnimator.ToBlackhole = false;
        _prjAnimator.enabled = false;
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
    public virtual void Expire()
    {
        if (!gameObject.activeSelf)
            return;
        _timer?.Stop();
        _disposable?.Dispose();
        ResetProjectile();
    }
    protected virtual void TimerFinished()
    {
        if(!_sucked && gameObject.activeSelf)
            Explode(true, gameObject.layer != StaticValues.ProjectileLayer);
    }
    protected virtual void Explode(bool shouldDamage, bool extraDamage = false)
    {
        EmitParticles(transform.position, shouldDamage, extraDamage);
        Expire();
    }
    protected virtual void EmitParticles(Vector3 pos, bool shouldDamage, bool extraDamage)
    {
        if (explosionParticlesPool != null)
        {
            var ps = explosionParticlesPool.Pool.Get();
            float dmg = shouldDamage ? damage : 0;
            dmg += extraDamage ? _damageLevel/3f : 0;
            ps.Initialize(shouldDamage ? damage : 0);
            ps.transform.position = pos;
        }
    }
    public void GetSucked()
    {
        if (_sucked)
            return;
        if (!_timer.Running)
        {
            if (!_shouldSuck)
                _shouldSuck = true;
            return;
        }
        _sucked = true;
        _shouldSuck = false;
        _timer.Stop();
        _rb.velocity = Vector3.zero;
        _bezierCurve = null;
        _prjAnimator.SuctionDistance = Vector3.Distance(transform.position, _observer.VacuumTransform.position);
        _prjAnimator.ToVacuum = true;
        _prjAnimator.ToBlackhole = false;
        _prjAnimator.enabled = true;
    }
    public void GetBlackholed()
    {
        if (_blackholed)
            return;
        if (!_timer.Running)
        {
            if (!_shouldBlackhole)
                _shouldBlackhole = true;
            return;
        }
        EventsPool.EnemyBlackholed.Invoke();
        _blackholed = true;
        _shouldBlackhole = false;
        _timer.Stop();
        _bezierCurve = null;
        _rb.velocity = Vector3.zero;
        _prjAnimator.SuctionDistance = Vector3.Distance(transform.position, _observer.PlayerTransform.position);
        _prjAnimator.ToBlackhole = true;
        _prjAnimator.ToVacuum = false;
        _prjAnimator.enabled = true;
    }
    protected abstract void HitUnit(Collider other);
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
        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }
        else if (other.gameObject.layer == StaticValues.VacuumLayer)
        {
            if (_sucked || _blackholed)
            {
                if(_sucked)
                    EventsPool.PickedupObjectEvent.Invoke(fillType);
                Expire();
            }
            else
                GetSucked();
        }
        else if(other.gameObject.layer == StaticValues.BlackHoleLayer)
        {
            GetBlackholed();
        }
        else if(other.gameObject.layer == StaticValues.PlayerLayer
            || other.gameObject.layer == StaticValues.EnemyLayer
            || other.gameObject.layer == StaticValues.DevilLayer)
        {
            if (_sucked || _blackholed || _shouldBlackhole || _shouldSuck)
            {
                if (_blackholed || _shouldBlackhole)
                    EventsPool.EnemyBlackholedFinished.Invoke();
                if(_sucked || _shouldSuck)
                    EventsPool.PickedupObjectEvent.Invoke(fillType);
                Expire();
            }
            else
                HitUnit(other);
        }
        else
        {
            Explode(false);
        }
    }
    private void Awake()
    {
        _observer = Observer.Instance;
        _damageLevel = PlayerStorage.DamageUpgradeLevel;
    }
    private void OnTriggerEnter(Collider other)
    {
        Triggered(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == StaticValues.VacuumLayer)
            _shouldSuck = false;
        else if (other.gameObject.layer == StaticValues.BlackHoleLayer)
            _shouldBlackhole = false;
    }
    private void FixedUpdate()
    {
        if (_rb == null)
            return;
        if (_shouldSuck)
            GetSucked();
        else if (_shouldBlackhole)
            GetBlackholed();
        else if (_bezierCurve != null)
            CurveProjectile();
    }
}