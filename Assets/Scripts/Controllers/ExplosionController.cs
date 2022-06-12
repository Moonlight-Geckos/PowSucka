using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ExplosionController : ParticleSystemController
{
    [SerializeField]
    private float explosionDamage = 4;
    private void OnTriggerEnter(Collider other)
    {
        if (!_canBehave)
            return;
        other.GetComponent<IDamagable>()?.GetDamage(explosionDamage);
    }
}
