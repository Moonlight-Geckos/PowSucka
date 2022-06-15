using System;
using UnityEngine.Events;

public static class EventsPool
{
    public static UnityEvent<Type> PickedupObjectEvent = new UnityEvent<Type>();
    public static UnityEvent ClearPoolsEvent = new UnityEvent();
    public static UnityEvent SpawnEnemyEvent = new UnityEvent();
}
