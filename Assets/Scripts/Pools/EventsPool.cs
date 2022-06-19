using UnityEngine.Events;

public static class EventsPool
{
    public readonly static UnityEvent GameStartedEvent = new UnityEvent();
    public readonly static UnityEvent<bool> GameFinishedEvent = new UnityEvent<bool>();


    public readonly static UnityEvent<FillType> PickedupObjectEvent = new UnityEvent<FillType>();
    public readonly static UnityEvent EnemyDiedEvent = new UnityEvent();

    public readonly static UnityEvent ClearPoolsEvent = new UnityEvent();
    public readonly static UnityEvent SpawnEnemyEvent = new UnityEvent();
    public readonly static UnityEvent EnemyBlackholed = new UnityEvent();
    public readonly static UnityEvent EnemyBlackholedFinished = new UnityEvent();
    public readonly static UnityEvent<bool> ChangePhaseEvent = new UnityEvent<bool>();
    public readonly static UnityEvent<bool> PlayerChangedMovementEvent = new UnityEvent<bool>();

    public readonly static UnityEvent<SkinItem> UpdateSkinEvent = new UnityEvent<SkinItem>();
    public readonly static UnityEvent UpdateUIEvent = new UnityEvent();
}