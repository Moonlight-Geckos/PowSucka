using UnityEngine;
using UnityEngine.AI;

public class Devil : Enemy
{
    protected override void StartChasing()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.destination = _observer.PlayerTransform.position;
    }
    protected override void Expire()
    {
        base.Expire();
        if(_observer.Started)
            EventsPool.GameFinishedEvent.Invoke(true);
        Destroy(gameObject);
    }
    protected override void Triggered(Collider other)
    {
        if (!_observer.Started)
            return;
        if (other.gameObject.layer == StaticValues.PlayerLayer)
        {
            other.GetComponent<IDamagable>().GetDamage(9999, 1);
        }
    }
    public override void GetDamage(float damage, float cooldown = -1)
    {
        base.GetDamage(damage, cooldown);
    }
    public override void StopDamage()
    {
        _takingDamage = -1;
    }

    protected override void StopChasing()
    {
        //Do nothing
    }

    protected override void TryShoot()
    {
        //Do nothing
    }
}
