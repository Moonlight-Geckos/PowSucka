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
    private int maxEnemiesOnGround = 25;

    [SerializeField]
    private EnemySpawnInst[] spawnProbabilities;

    [SerializeField]
    private GameObject bossPrefab;

    [SerializeField]
    private float startSpawnInterval = 2;

    [SerializeField]
    private float endSpawnInterval = 0.75f;

    [SerializeField]
    private float spawnOffsetFromCharacter = 40;

    #endregion

    private static EnemySpawner _instance;
    private static Timer _spawnTimer;
    private int _enemiesOnGround;
    private float _randomAngle;
    private float[] _currentProbablities;
    private float _currentInterval;
    private Enemy _enemy;
    private Vector3 _centerPosition;
    private List<int> canSpawn = new List<int>();
    private Observer _observer;

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
            _observer = Observer.Instance;
            _spawnTimer = TimersPool.Pool.Get();
            _spawnTimer.Duration = startSpawnInterval;
            _currentProbablities = new float[spawnProbabilities.Length];
            _spawnTimer.AddTimerFinishedEventListener(Spawn);
            EventsPool.PickedupObjectEvent.AddListener((FillType f) =>
            {
                if (!_spawnTimer.Running)
                    _spawnTimer.Run();
                _enemiesOnGround--;
            });
            EventsPool.SpawnBossEvent.AddListener(BossSpawn);
        }
    }
    private void Start()
    {
        Spawn();
    }
    void Spawn()
    {
        if (_enemiesOnGround >= maxEnemiesOnGround)
            return;

        _currentInterval = Mathf.Lerp(startSpawnInterval, endSpawnInterval, 1 - _observer.LeftEnemiesToKill / GameManager.Instance.EnemiesToKill);
        _centerPosition = _observer.PlayerTransform.position;
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
            _currentProbablities[i] = Mathf.Lerp(spawnProbabilities[i].startProbablity, spawnProbabilities[i].endProbablity, _currentInterval);
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
        _enemy = spawnProbabilities[canSpawn[UnityEngine.Random.Range(0, canSpawn.Count)]].enemyPool.Pool.Get();
        _enemy.transform.position = _centerPosition;
        _enemy.Initialize();
        if (Observer.weaponMode)
        {
            _spawnTimer.Duration = _currentInterval / 2f;
        }
        else
            _spawnTimer.Duration = _currentInterval;

        _enemiesOnGround++;
        _spawnTimer.Run();
    }

    void BossSpawn()
    {
        if(bossPrefab == null && _observer.Started)
        {
            EventsPool.GameFinishedEvent.Invoke(true);
        }
        else
        {
            _centerPosition = _observer.PlayerTransform.position;
            _randomAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
            _centerPosition.x += spawnOffsetFromCharacter * Mathf.Cos(_randomAngle);
            if (Mathf.Rad2Deg * _randomAngle > 60 && Mathf.Rad2Deg * _randomAngle < 130)
                _centerPosition.z += 2 * spawnOffsetFromCharacter * Mathf.Sin(_randomAngle);
            else
                _centerPosition.z += spawnOffsetFromCharacter * Mathf.Sin(_randomAngle);

            if (_centerPosition.x <= -WorldLimits.XLimits)
                _centerPosition.x += spawnOffsetFromCharacter * 2;
            else if (_centerPosition.x >= WorldLimits.XLimits)
                _centerPosition.x -= spawnOffsetFromCharacter * 2;
            if (_centerPosition.z <= -WorldLimits.ZLimits)
                _centerPosition.z += spawnOffsetFromCharacter * 2;
            else if (_centerPosition.z >= WorldLimits.ZLimits)
                _centerPosition.z -= spawnOffsetFromCharacter * 4;

            Instantiate(bossPrefab, _centerPosition, Quaternion.identity);

        }
    }
    private void OnValidate()
    {
        if(spawnProbabilities != null)
            _currentProbablities = new float[spawnProbabilities.Length];
    }
}
