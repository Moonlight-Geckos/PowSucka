using UnityEngine;

public class PickupAnimator : MonoBehaviour
{
    public float SuctionDistance;

    private TrailRenderer _trailRenderer;
    private Vector3 _direction;
    private Vector3 _newScale;
    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    private void Update()
    {
        ScaleWithVacuum();
        MoveToVacuum();
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
        _direction = (GameManager.Instance.VacuumTransform.position - transform.position);
        transform.Translate(_direction.normalized * GameManager.Instance.SuctionVelocity * Time.deltaTime, Space.World);
        if (_trailRenderer != null)
            _trailRenderer.widthMultiplier = transform.localScale.x;
    }
}
