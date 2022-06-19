using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private int enemiesToKill = 30;

    [SerializeField]
    private float suctionVelocity = 30;

    #endregion

    #region Private

    private static Transform _playerTransform;
    private static Transform _vacuumTransform;
    private static GameManager _instance;
    private static MonoBehaviour[] _systemScripts;
    private static int _collectedGems;
    private static int _leftEnemiesToKill;
    private static bool _started;
    private static bool _finished;
    private static float _timeSinceStarted = 0;

    #endregion
    static public GameManager Instance
    {
        get { return _instance; }
    }
    public Transform PlayerTransform
    {
        get { return _playerTransform; }
    }
    public Transform VacuumTransform
    {
        get { return _vacuumTransform; }
    }
    public bool Started
    {
        get { return _started; }
        set { _started = value; }
    }
    public bool Finished
    {
        get { return _finished; }
        set { _finished = value; }
    }
    public float TimeSinceStarted
    {
        get { return _timeSinceStarted; }
    }
    public int EnemiesToKill
    {
        get { return enemiesToKill; }
    }
    public int LeftEnemiesToKill
    {
        get { return _leftEnemiesToKill; }
    }
    public int CollectedGems
    {
        get
        {
            return _collectedGems;
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
            _started = false;
            _timeSinceStarted = 0;
            _systemScripts = GetComponents<MonoBehaviour>();
            foreach(var script in _systemScripts)
            {
                if(script != this)
                    script.enabled = false;
            }
            _leftEnemiesToKill = enemiesToKill;
            _collectedGems = PlayerStorage.CoinsCollected;
            EventsPool.PickedupObjectEvent.AddListener(CollectGem);
            EventsPool.GameStartedEvent.AddListener(StartGame);
            EventsPool.EnemyDiedEvent.AddListener(EnemyDied);
        }
    }
    private void Start()
    {
        EventsPool.UpdateUIEvent.Invoke();
    }
    void Update()
    {
        if (!_started)
            return;
        TimersPool.UpdateTimers(Time.deltaTime);
        _timeSinceStarted += Time.deltaTime;
    }
    private void StartGame()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _vacuumTransform = GameObject.FindGameObjectWithTag("Vacuum").transform;
        foreach (var script in _systemScripts)
        {
            script.enabled = true;
        }
        _started = true;
    }
    private void CollectGem(FillType t)
    {
        if (t != FillType.Diamond)
            return;
        _collectedGems++;
        PlayerStorage.CoinsCollected = _collectedGems;
        EventsPool.UpdateUIEvent.Invoke();
    }
    private void EnemyDied()
    {
        _leftEnemiesToKill--;
        EventsPool.UpdateUIEvent.Invoke();
        if (_leftEnemiesToKill <= 0)
            EventsPool.GameFinishedEvent.Invoke(true);
    }
    public void AddGem()
    {
        _collectedGems++;
    }
}
