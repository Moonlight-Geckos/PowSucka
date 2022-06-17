using UnityEngine.Events;

public static class EventsPool
{
    public static UnityEvent<FillType> PickedupObjectEvent = new UnityEvent<FillType>();
    public static UnityEvent ClearPoolsEvent = new UnityEvent();
    public static UnityEvent SpawnEnemyEvent = new UnityEvent();
    public static UnityEvent EnemyBlackholed = new UnityEvent();
    public static UnityEvent EnemyBlackholedFinished = new UnityEvent();
    public static UnityEvent ChangePhaseEvent = new UnityEvent();
}