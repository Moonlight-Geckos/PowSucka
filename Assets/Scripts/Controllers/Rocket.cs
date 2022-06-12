using UnityEngine;
public class Rocket : Projectile
{
    private bool _triggered;

    public bool HasTriggered
    {
        get { return _triggered; }
    }
    private void OnEnable()
    {
        _triggered = false;
    }
    public override void Triggered(Collider other)
    {
        Rocket otherRocket;
        if (other.transform.parent.TryGetComponent(out otherRocket))
        {
            if (_bezierCurve == null)
            {
                if (!otherRocket.HasTriggered)
                {
                    _triggered = true;
                    EmitParticles(other.ClosestPoint(transform.position));
                }
                Expire();
            }
        }
        base.Triggered(other);
    }
    public override void EmitParticles(Vector3 pos)
    {
        var ps = explosionParticlesPool.Pool.Get();
        ps.transform.position = pos;
        ps.Initialize(_triggered);
    }
}