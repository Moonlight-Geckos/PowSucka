using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class EnemySpawnInst
{
    [SerializeField]
    [Range(0f, 1f)]
    public float startProbablity;

    [SerializeField]
    [Range(0f, 1f)]
    public float endProbablity;

    [SerializeField]
    public EnemyPool enemyPool;
}

public class EnemySpawner : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private EnemySpawnInst[] spawnProbabilities;

    [SerializeField]
    private float spawnInterval = 2;

    [SerializeField]
    private float spawnOffsetFromCharacter = 40;

    #endregion

    private static EnemySpawner _instance;
    private static Timer _spawnTimer;
    private Enemy enemy;
    private Vector3 _centerPosition;
    private float _randomAngle;
    private float[] _currentProbablities;
    private List<int> canSpawn = new List<int>();

    public static EnemySpawner Instance
    {
        get { return _instance; }
    }
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            _spawnTimer = TimersPool.Pool.Get();
            _spawnTimer.Duration = spawnInterval;
            _spawnTimer.AddTimerFinishedEventListener(Spawn);
        }
    }
    private void Start()
    {
        Spawn();
    }
    void Spawn()
    {
        _centerPosition = GameManager.Instance.PlayerTransform.position;
        _randomAngle = UnityEngine.Random.Range(-180, 180);
        _centerPosition.x += spawnOffsetFromCharacter * Mathf.Sin(_randomAngle);
        _centerPosition.z += spawnOffsetFromCharacter * Mathf.Cos(_randomAngle);

        if (_centerPosition.x <= -WorldLimits.XLimits)
            _centerPosition.x += spawnOffsetFromCharacter * 2;
        else if(_centerPosition.x >= WorldLimits.XLimits)
            _centerPosition.x -= spawnOffsetFromCharacter * 2;
        if (_centerPosition.z <= -WorldLimits.ZLimits)
            _centerPosition.z += spawnOffsetFromCharacter * 2;
        else if (_centerPosition.z >= WorldLimits.ZLimits)
            _centerPosition.z -= spawnOffsetFromCharacter * 2;

        canSpawn.Clear();
        float cumulativeProb = 0;

        for (int i = 0;i < spawnProbabilities.Length; i++)
        {
            _currentProbablities[i] = Mathf.Lerp(spawnProbabilities[i].startProbablity, spawnProbabilities[i].endProbablity, GameManager.Instance.TimeSinceStarted / GameManager.Instance.TimeToSpawnBoss);
            cumulativeProb += _currentProbablities[i];
        }

        float probablity = UnityEngine.Random.Range(0.0f, cumulativeProb);
        cumulativeProb = 0;

        for (int i = 0;i < spawnProbabilities.Length; i++)
        {
            if (probablity <= _currentProbablities[i] + cumulativeProb)
            {
                canSpawn.Add(i);
                break;
            }
            cumulativeProb += _currentProbablities[i];
        }
        if(canSpawn.Count == 0)
            canSpawn.Add(UnityEngine.Random.Range(0, spawnProbabilities.Length));
        enemy = spawnProbabilities[canSpawn[UnityEngine.Random.Range(0, canSpawn.Count)]].enemyPool.Pool.Get();
        enemy.transform.position = _centerPosition;
        enemy.Initialize();
        _spawnTimer.Run();
    }
    private void OnValidate()
    {
        if(spawnProbabilities != null)
            _currentProbablities = new float[spawnProbabilities.Length];
    }
}
