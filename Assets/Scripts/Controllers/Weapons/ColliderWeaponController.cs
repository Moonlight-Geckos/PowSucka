using System.Collections.Generic;
using UnityEngine;

public class ColliderWeaponController : MonoBehaviour
{
    [SerializeField]
    private float damage = 1;

    [SerializeField]
    private float damageCooldown = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        var t = other.GetComponent<IDamagable>();
        t?.GetDamage(damage, damageCooldown);
    }
    private void OnTriggerExit(Collider other)
    {
        var t = other.GetComponent<IDamagable>();
        t?.StopDamage();
    }

}
