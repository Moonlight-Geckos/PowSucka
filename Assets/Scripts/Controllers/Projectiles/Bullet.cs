using UnityEngine;
using System.Collections;

public class Bullet : Projectile
{
    protected override void HitUnit(Collider other)
    {
        Explode(true);
    }
}