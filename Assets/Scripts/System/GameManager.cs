using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private float timeToSpawnBoss = 30;

    [SerializeField]
    private float suctionVelocity = 30;

    #endregion

    #region Private

    private static Transform playerTransform;
    private static Transform vacuumTransform;
    private static GameManager _instance;
    private static int collectedGems;
    private static bool started;
    private static float _timeSinceStarted = 0;

    #endregion
    static public GameManager Instance
    {
        get { return _instance; }
    }
    public Transform PlayerTransform
    {
        get { return playerTransform; }
    }
    public Transform VacuumTransform
    {
        get { return vacuumTransform; }
    }
    public bool Started
    {
        get { return started; }
        set { started = value; }
    }
    public float TimeSinceStarted
    {
        get { return _timeSinceStarted; }
    }
    public float TimeToSpawnBoss
    {
        get { return timeToSpawnBoss; }
    }
    public int CollectedGems
    {
        get
        {
            return collectedGems;
        }
    }
    public float SuctionVelocity
    {
        get
        {
            return suctionVelocity;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            started = false;
            _timeSinceStarted = 0;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            vacuumTransform = GameObject.FindGameObjectWithTag("Vacuum").transform;
        }
    }
    void Update()
    {
        TimersPool.UpdateTimers(Time.deltaTime);
        _timeSinceStarted += Time.deltaTime;
    }
    public void AddGem()
    {
        collectedGems++;
    }
}
