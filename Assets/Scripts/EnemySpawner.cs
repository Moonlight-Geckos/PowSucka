using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class ProbabilitySlider
{
    [SerializeField]
    [Range(0f, 1f)]
    public float probablity;
}

public class EnemySpawner : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private GameObjectPool[] enemyPools;

    [SerializeField]
    private ProbabilitySlider[] spawnProbabilities;

    #endregion

    private static EnemySpawner _instance;
    private Transform _playerTransform;
    private Vector3 _centerPosition;
    private float _randomAngle;
    private GameObject enemy;
    private float probablity;

    private List<int> canSpawn = new List<int>();
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
        _randomAngle = UnityEngine.Random.Range(-180, 180);
        _centerPosition.x += 35 * Mathf.Sin(_randomAngle);
        _centerPosition.z += 35 * Mathf.Cos(_randomAngle);

        if (_centerPosition.x <= -WorldLimits.XLimits)
            _centerPosition.x += 35 * 2;
        else if(_centerPosition.x >= WorldLimits.XLimits)
            _centerPosition.x -= 35 * 2;
        if (_centerPosition.z <= -WorldLimits.ZLimits)
            _centerPosition.z += 35 * 2;
        else if (_centerPosition.z >= WorldLimits.ZLimits)
            _centerPosition.z -= 35 * 2;

        canSpawn.Clear();

        float cumulativeProb = 0;
        foreach(ProbabilitySlider slider in spawnProbabilities)
        {
            cumulativeProb += slider.probablity;

        }

        float probablity = UnityEngine.Random.Range(0.0f, cumulativeProb);
        cumulativeProb = 0;

        for (int i = 0;i < enemyPools.Length; i++)
        {
            if (probablity <= spawnProbabilities[i].probablity + cumulativeProb)
            {
                canSpawn.Add(i);
                break;
            }
            cumulativeProb += spawnProbabilities[i].probablity;
        }
        if(canSpawn.Count == 0)
            canSpawn.Add(UnityEngine.Random.Range(0, enemyPools.Length));
        Debug.Log(UnityEngine.Random.Range(0, canSpawn.Count));
        enemy = enemyPools[canSpawn[UnityEngine.Random.Range(0, canSpawn.Count)]].Pool.Get();
        enemy.transform.position = _centerPosition;
        enemy.GetComponent<Enemy>().Initialize();
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Spawn"))
        {
            Spawn();
        }
    }
}
