using System;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{

    [Serializable]
    class EnemySpawnInst
    {
        public float startProbablity;

        public float endProbablity;

        public EnemyPool enemyPool;
    }

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
            _currentProbablities = new float[spawnProbabilities.Length];
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
        _randomAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        _centerPosition.x += spawnOffsetFromCharacter * Mathf.Cos(_randomAngle);
        if (Mathf.Rad2Deg * _randomAngle > 60 && Mathf.Rad2Deg * _randomAngle < 130)
            _centerPosition.z += 2 * spawnOffsetFromCharacter * Mathf.Sin(_randomAngle);
        else
            _centerPosition.z += spawnOffsetFromCharacter * Mathf.Sin(_randomAngle);

        if (_centerPosition.x <= -WorldLimits.XLimits)
            _centerPosition.x += spawnOffsetFromCharacter * 2;
        else if(_centerPosition.x >= WorldLimits.XLimits)
            _centerPosition.x -= spawnOffsetFromCharacter * 2;
        if (_centerPosition.z <= -WorldLimits.ZLimits)
            _centerPosition.z += spawnOffsetFromCharacter * 2;
        else if (_centerPosition.z >= WorldLimits.ZLimits)
            _centerPosition.z -= spawnOffsetFromCharacter * 4;

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
        if (Observer.weaponMode)
        {
            _spawnTimer.Duration = spawnInterval / 4f;
        }
        else
            _spawnTimer.Duration = spawnInterval;
        _spawnTimer.Run();
    }
    private void OnValidate()
    {
        if(spawnProbabilities != null)
            _currentProbablities = new float[spawnProbabilities.Length];
    }
}
