using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private GameObject enemy;

    #endregion


    private static EnemySpawner _instance;

    private Transform _playerTransform;
    private Vector3 _centerPosition;
    private float _randomAngle;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    void Spawn()
    {
        _centerPosition = _playerTransform.position;
        _randomAngle = Random.Range(-180, 180);
        _centerPosition.x += 35 * Mathf.Sin(_randomAngle);
        _centerPosition.z += 35 * Mathf.Cos(_randomAngle);
        Instantiate(enemy, _centerPosition, Quaternion.identity);
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Spawn"))
        {
            Spawn();
        }
    }
}
