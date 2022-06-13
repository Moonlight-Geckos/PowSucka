using UnityEngine;
using System.Collections;

public class Bullet : Projectile
{
    protected override void HitPlayer(Collider other)
    {
        Explode(true);
    }
}