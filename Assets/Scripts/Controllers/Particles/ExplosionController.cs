using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ExplosionController : ParticleSystemController
{
    private void OnTriggerEnter(Collider other)
    {
        var t = other.GetComponent<IDamagable>();
        t?.GetDamage(_damage);
        t?.StopDamage();
    }
}