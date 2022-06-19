using UnityEngine;

[RequireComponent(typeof(PickupAnimator))]
public class Pickup : MonoBehaviour
{
    [SerializeField]
    protected FillType fillType;

    protected bool _sucked;
    protected PickupAnimator _animator;
    protected Projector _projector;
    protected Observer _observer;
    public void GetSucked()
    {
        if (_sucked)
            return;
        _sucked = true;
        _animator.SuctionDistance = Vector3.Distance(transform.position, _observer.VacuumTransform.position);
        _animator.enabled = true;
        if (_projector != null)
            _projector.enabled = false;
    }
    protected virtual void GetPicked(Collider other)
    {
        EventsPool.PickedupObjectEvent.Invoke(fillType);
    }
    protected virtual void Expire()
    {
        _animator.enabled = false;
        GetComponent<IDisposable>()?.Dispose();
    }
    protected virtual void Triggered(Collider other)
    {
        if (other.gameObject.layer == StaticValues.VacuumLayer
            || other.gameObject.layer == StaticValues.BlackHoleLayer
            || other.gameObject.layer == StaticValues.PlayerLayer
            )
        {
            if (_sucked)
            {
                Expire();
            }
            else
            {
                GetSucked();
                GetPicked(other);
            }
        }
    }
    private void Awake()
    {
        _observer = Observer.Instance;
        _animator = GetComponent<PickupAnimator>();
        _projector = GetComponentInChildren<Projector>();
        _animator.enabled = false;
        if(_projector != null)
            _projector.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        Triggered(other);
    }
    private void OnEnable()
    {
        _animator.enabled = false;
        if (_projector != null)
            _projector.enabled = true;
    }
}
