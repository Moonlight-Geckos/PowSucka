using System.Collections.Generic;
using UnityEngine;

public class ColliderWeaponController : MonoBehaviour
{
    [SerializeField]
    private float damage = 1;

    [SerializeField]
    private float damageCooldown = 0.5f;

    private float _damageLevel;
    private void Awake()
    {
        _damageLevel = PlayerStorage.DamageUpgradeLevel;
    }
    private void OnTriggerEnter(Collider other)
    {
        var t = other.GetComponent<IDamagable>();
        t?.GetDamage(damage + _damageLevel/3f, damageCooldown);
    }
    private void OnTriggerExit(Collider other)
    {
        var t = other.GetComponent<IDamagable>();
        t?.StopDamage(damage + _damageLevel/3f);
    }
}
