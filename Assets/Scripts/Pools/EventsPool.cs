using UnityEngine.Events;

public static class EventsPool
{
    public static UnityEvent<Projectile> PickedupProjectileEvent = new UnityEvent<Projectile>();
    public static UnityEvent ClearPoolsEvent = new UnityEvent();
    public static UnityEvent SpawnEnemyEvent = new UnityEvent();
}
