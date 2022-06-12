using UnityEngine;

public class Bomb : Projectile
{
    public override void Shoot(Vector3 velocity, Bezier _bezierCurve = null)
    {
        base.Shoot(velocity);
        _animator.speed = 1;
    }
    public override void Initialize(Vector3 position)
    {
        base.Initialize(position);
        _animator.speed = 0;
    }
}
