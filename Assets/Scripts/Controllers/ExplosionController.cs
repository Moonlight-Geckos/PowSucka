using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ExplosionController : ParticleSystemController
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamagable>()?.GetDamage(_damage);
    }
}
