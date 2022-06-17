using System.Collections;
using UnityEngine;

public class GooProjectile : Projectile
{
    [SerializeField]
    private float onFlatScale = 8;

    [SerializeField]
    private float onGroundDuration = 5;

    private bool _active = false;
    private bool _shot = false;
    private Timer _activeTimer;
    protected override void HitUnit(Collider other)
    {
        other.GetComponent<IDamagable>()?.GetDamage(damage);
    }
    protected override void Triggered(Collider other)
    {
        if (_shot)
            return;
        if (_active)
        {
            HitUnit(other);
        }
        else
        {
            SitOnGround();
        }
    }
    protected override void ResetProjectile()
    {
        if(_activeTimer == null)
        {
            _activeTimer = TimersPool.Pool.Get();
            _activeTimer.Duration = onGroundDuration;
            _activeTimer.AddTimerFinishedEventListener(Disolve);
        }
        _active = false;
        base.ResetProjectile();
    }
    public override void Initialize(Vector3 position)
    {
        base.Initialize(position);
        transform.localScale = new Vector3(1, 1, 2);
    }
    protected override void TimerFinished()
    {
        SitOnGround();
    }
    private void SitOnGround()
    {
        if (_active)
            return;
        _rb.velocity = Vector3.zero;
        _shot = false;
        _active = true;

        IEnumerator getbig()
        {
            yield return null;
            while (transform.localScale.x < onFlatScale-0.1f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * onFlatScale, Time.deltaTime * 4);
                yield return new WaitForEndOfFrame();
            }
            _activeTimer.Run();
        }
        StartCoroutine(getbig());
    }
    private void Disolve()
    {
        Debug.Log("Dissolve??");
        IEnumerator disolve()
        {
            yield return null;
            while(transform.localScale.x > 0.05f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 6);
                yield return new WaitForEndOfFrame();
            }
            Expire();
        }
        StartCoroutine(disolve());

    }
}
