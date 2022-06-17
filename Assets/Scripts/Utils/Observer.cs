using UnityEngine;
using System.Collections.Generic;
public class Observer : MonoBehaviour
{
    public static bool weaponMode = false;

    private void Awake()
    {
        EventsPool.ChangePhaseEvent.AddListener(() => weaponMode = !weaponMode);
    }
}
