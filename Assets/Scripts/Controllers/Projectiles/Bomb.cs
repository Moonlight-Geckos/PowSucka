using UnityEngine;

public class Bomb : Projectile
{
    protected override void HitUnit(Collider other)
    {
        Explode(true);
    }
}