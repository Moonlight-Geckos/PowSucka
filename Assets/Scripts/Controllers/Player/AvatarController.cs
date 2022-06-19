using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AvatarController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float maxHealth = 100;

    private HealthBar _healthBar;
    private SkinController _skinController;
    private float _currentHP;

    private void Awake()
    {
        Initialize();
        _currentHP = maxHealth;
        EventsPool.EnemyDiedEvent.AddListener(Heal);
        EventsPool.GameFinishedEvent.AddListener(FinishGame);
    }

    private void OnEnable()
    {
        Initialize();
    }
    private void Initialize()
    {
        _skinController = GetComponentInChildren<SkinController>();
        _healthBar = FindObjectOfType<HealthBar>();
    }
    public void GetDamage(float damage, float cooldown = -1)
    {
        if (damage <= 0)
            return;
        _currentHP -= damage;
        _healthBar.UpdateValue(_currentHP / 100f);
        StopAllCoroutines();
        _skinController.AnimateHit();
    }
    private void Heal()
    {
        if(_currentHP < 100)
        {
            _currentHP+=4;
            _healthBar.UpdateValue(_currentHP / 100f);
        }
    }
    private void FinishGame(bool win)
    {
        
    }
    public void StopDamage() { }

}