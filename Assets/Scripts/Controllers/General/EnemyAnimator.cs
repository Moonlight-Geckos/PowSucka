using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public float SuctionDistance;

    private float _curDis;
    private Vector3 _newScale;
    private Vector3 _direction;
    private void Update()
    {
        ScaleWithBlackhole();
        MoveToBlackhole();
    }
    private void ScaleWithBlackhole()
    {
        _newScale = Vector3.Lerp(
            Vector3.one,
            Vector3.zero,
            Mathf.Clamp(
                1.2f - (Vector3.Distance(transform.position, GameManager.Instance.VacuumTransform.position) / SuctionDistance),
                0, 1)
            );
        if (_newScale.x < transform.localScale.x) transform.localScale = _newScale;
    }
    private void MoveToBlackhole()
    {
        _direction = (GameManager.Instance.PlayerTransform.position - transform.position);
        _direction.y = 0;
        _direction = Quaternion.Euler(0, -80, 0) * _direction;
        _curDis = GameManager.Instance.SuctionVelocity * Time.deltaTime;
        transform.Translate(_direction.normalized * _curDis, Space.World);
        transform.rotation = Quaternion.LookRotation(_direction.normalized, Vector3.up);
    }
}
