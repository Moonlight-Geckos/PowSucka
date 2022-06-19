using UnityEngine;
using System.Collections.Generic;
public class Observer : MonoBehaviour
{
    public static bool weaponMode = false;
    public static float weaponDuration = 1;
    public static bool playerMoving = false;

    private void Awake()
    {
        EventsPool.PlayerChangedMovementEvent.AddListener((bool m) => playerMoving = m);
        EventsPool.ChangePhaseEvent.AddListener((bool wm) => weaponMode = wm);
    }
}
