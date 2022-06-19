using UnityEngine;

public class ProjectileAnimator : MonoBehaviour
{
    [HideInInspector]
    public float SuctionDistance;

    [HideInInspector]
    public bool ToVacuum;

    [HideInInspector]
    public bool ToBlackhole;

    private TrailRenderer _trailRenderer;
    private Vector3 _newScale;
    private Vector3 _direction;
    private float _curDis;
    private Observer _observer;
    private void Awake()
    {
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _observer = Observer.Instance;
        _curDis = GameManager.Instance.SuctionVelocity * Time.deltaTime;
        ToBlackhole = false;
        ToVacuum = false;
    }
    private void Update()
    {
        if (ToVacuum)
        {
            ScaleWithVacuum();
            MoveToVacuum();
        }
        else
        {
            ScaleWithBlackHole();
            MoveToBlackHole();
        }
    }
    private void ScaleWithVacuum()
    {
        _newScale = Vector3.Lerp(
            Vector3.one,
            Vector3.zero,
            Mathf.Clamp(
                1.2f - (Vector3.Distance(transform.position, _observer.VacuumTransform.position) / SuctionDistance),
                0, 1)
            );
        if (_newScale.x < transform.localScale.x) transform.localScale = _newScale;
        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = _newScale.x;
    }
    private void MoveToVacuum()
    {
        transform.Translate((_observer.VacuumTransform.position - transform.position).normalized * _curDis, Space.World);
        transform.rotation = Quaternion.LookRotation(_direction.normalized, Vector3.up);
    }
    private void ScaleWithBlackHole()
    {
        _newScale = Vector3.Lerp(
            transform.localScale,
            Vector3.zero,
            3 * Time.deltaTime
            );
        if (_newScale.x < transform.localScale.x) transform.localScale = _newScale;
        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = _newScale.x;
    }
    private void MoveToBlackHole()
    {
        _direction = Quaternion.Euler(0, -65, 0) * (_observer.PlayerTransform.position - transform.position);
        transform.Translate(_direction.normalized * _curDis, Space.World);
        transform.rotation = Quaternion.LookRotation(_direction.normalized, Vector3.up);
    }
}
