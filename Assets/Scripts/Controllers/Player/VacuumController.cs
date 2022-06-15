using System;
using System.Collections.Generic;
using UnityEngine;

public class VacuumController : MonoBehaviour
{
    [SerializeField]
    private Transform liquidFill;

    [SerializeField]
    private ParticleSystem collectedPRJPS;

    private byte _fillPercent = 0;
    private Dictionary<Type, int> _projectilesFilled;

    private void Awake()
    {
        liquidFill.localScale = new Vector3(1, 0, 1);
        EventsPool.PickedupObjectEvent.AddListener(PickupProjectile);
        _projectilesFilled = new Dictionary<Type, int>();
        _fillPercent = 0;
    }
    private void PickupProjectile(Type prj)
    {
        if (!prj.IsSubclassOf(typeof(Projectile)))
            return;
        if (_fillPercent < 100)
        {
            _fillPercent += StaticValues.GetFillPercent(prj);
            liquidFill.localScale = new Vector3(1, _fillPercent/100f, 1);
            collectedPRJPS.Play();

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
        foreach(Type type in _projectilesFilled.Keys)
        {
            Debug.Log(type.Name + " " + _projectilesFilled[type].ToString());
        }

        Debug.Log("Boom !!");
    }
}