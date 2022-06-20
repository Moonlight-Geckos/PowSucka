using UnityEngine;

public class DevilAura : MonoBehaviour
{
    [SerializeField]
    private float damagePerSecond = 10;

    IDamagable unit;
    Observer _observer;
    private void Awake()
    {
        _observer = Observer.Instance;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!_observer.Started)
            return;
        if (other.gameObject.layer == StaticValues.PlayerLayer)
        {
            if (unit == null)
                unit = other.GetComponent<IDamagable>();
            unit?.GetDamage(damagePerSecond, 1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        unit?.StopDamage();
    }
}
