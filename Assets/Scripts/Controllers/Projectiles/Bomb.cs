using UnityEngine;

public class Bomb : Projectile
{
    protected override void HitPlayer(Collider other)
    {
        Explode(true);
    }
}