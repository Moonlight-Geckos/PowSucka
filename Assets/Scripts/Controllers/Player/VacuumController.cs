using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumController : MonoBehaviour
{
    [Serializable]
    class ShootingItem
    {
        public GameObject parent;
        public ShootingType shootType;
        public float duration;
    }
    [SerializeField]
    private GameObject suction;

    [SerializeField]
    private ShootingItem[] shootingItems;


    [SerializeField]
    private ParticleSystem collectedPRJParticleSystem;


    private byte _fillPercent = 0;
    private Timer _weaponTimer;
    private ShootingItem _currentWeapon;
    private Dictionary<FillType, int> _projectilesFilled;

    private void Awake()
    {
        _projectilesFilled = new Dictionary<FillType, int>();
        _fillPercent = 0;
        _weaponTimer = TimersPool.Pool.Get();
        _weaponTimer.AddTimerFinishedEventListener(DisposeWeapon);
        EventsPool.PickedupObjectEvent.AddListener(PickupProjectile);
    }
    private void PickupProjectile(FillType prj)
    {
        if (prj == FillType.Diamond || Observer.weaponMode)
            return;
        if (_fillPercent < 100)
        {
            _fillPercent += StaticValues.GetFillPercent(prj);
            collectedPRJParticleSystem.Play();

            if (!_projectilesFilled.ContainsKey(prj))
            {
                _projectilesFilled.Add(prj, 0);
            }
            _projectilesFilled[prj]++;
        }
        if (_fillPercent >= 100)
        {
            VacuumFull();
        }
    }
    private void VacuumFull()
    {
        int x = -1, y = -2;
        FillType x1 = FillType.Goo, x2 = FillType.Bullet;
        foreach(var item in _projectilesFilled)
        {
            if(y < x)
            {
                if (item.Value > y)
                {
                    y = item.Value;
                    x2 = item.Key;
                }
            }
            else if(item.Value > x)
            {
                x = item.Value;
                x1 = item.Key;
            }
        }
        Debug.Log(new Tuple<FillType, FillType>(x1, x2));
        ShootingType ds = StaticValues.Combinations[new Tuple<FillType, FillType>(x1, x2)];
        foreach(ShootingItem pair in shootingItems)
        {
            if (pair.shootType == ds)
                _currentWeapon = pair;
            else
                pair.parent.SetActive(false);
        }
        suction.SetActive(false);
        _currentWeapon.parent.SetActive(true);
        _weaponTimer.Duration = _currentWeapon.duration;
        _weaponTimer.Run();
        _projectilesFilled.Clear();
        _fillPercent = 0;
        Observer.weaponDuration = _currentWeapon.duration;
        EventsPool.ChangePhaseEvent.Invoke(true);
    }
    private void DisposeWeapon()
    {
        IEnumerator dispose()
        {
            yield return null;
            var cd = _currentWeapon.parent.GetComponent<Collider>();
            if(cd != null)
                cd.enabled = false;
            if (cd != null)
                cd.enabled = true;
            suction.SetActive(true);
            _currentWeapon.parent.SetActive(false);
            EventsPool.ChangePhaseEvent.Invoke(false);
        }
        StartCoroutine(dispose());
    }
    private void FinishGame(bool w)
    {

    }
}