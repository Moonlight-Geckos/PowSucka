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
    private void Awake()
    {
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
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
                1.2f - (Vector3.Distance(transform.position, GameManager.Instance.VacuumTransform.position) / SuctionDistance),
                0, 1)
            );
        if (_newScale.x < transform.localScale.x) transform.localScale = _newScale;
        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = transform.localScale.x;
    }
    private void MoveToVacuum()
    {
        _direction = (GameManager.Instance.VacuumTransform.position - transform.position).normalized;
        transform.position = Vector3.Lerp(
            transform.position,
            GameManager.Instance.VacuumTransform.position,
            Time.deltaTime * (GameManager.Instance.SuctionVelocity / SuctionDistance)
            );
        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = transform.localScale.x;

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
            _trailRenderer.widthMultiplier = transform.localScale.x;
    }
    private void MoveToBlackHole()
    {
        _direction = (GameManager.Instance.PlayerTransform.position - transform.position);
        _direction.y = 0;
        _direction = Quaternion.Euler(0, -80, 0) * _direction;
        _curDis = GameManager.Instance.SuctionVelocity * Time.deltaTime;
        transform.Translate(_direction.normalized * _curDis, Space.World);
        transform.rotation = Quaternion.LookRotation(_direction.normalized, Vector3.up);
    }
}
