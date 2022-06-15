using UnityEngine;
public class Rocket : Projectile
{
    private bool _triggered;
    public bool HasTriggered
    {
        get { return _triggered; }
    }
    protected override void Triggered(Collider other)
    {
        Rocket otherRocket;
        if (other.transform.parent.TryGetComponent(out otherRocket))
        {
            if (_bezierCurve == null)
            {
                if (!otherRocket.HasTriggered)
                {
                    _triggered = true;
                    EmitParticles(other.ClosestPoint(transform.position), true);
                }
                Expire();
            }
        }
        else
            base.Triggered(other);
    }
    protected override void HitPlayer(Collider other)
    {
        Explode(true);
    }
    protected override void ResetProjectile()
    {
        base.ResetProjectile();
        _triggered = false;
    }
}