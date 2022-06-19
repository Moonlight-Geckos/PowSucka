using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private int enemiesToKill = 30;

    [SerializeField]
    private float suctionVelocity = 30;

    [SerializeField]
    private GameObject spawnersObject;

    #endregion

    #region Private

    private static GameManager _instance;

    private static int _collectedGems;
    private static float _timeSinceStarted = 0;
    private static Observer _observer;

    #endregion
    static public GameManager Instance
    {
        get { return _instance; }
    }
    public int EnemiesToKill
    {
        get { return enemiesToKill; }
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
    public float TimeSinceStarted
    {
        get { return _timeSinceStarted; }
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
            _collectedGems = PlayerStorage.CoinsCollected;
            EventsPool.ClearPoolsEvent.Invoke();
            EventsPool.PickedupObjectEvent.AddListener(CollectGem);
            EventsPool.GameStartedEvent.AddListener(StartGame);
            EventsPool.GameFinishedEvent.AddListener(FinishGame);
        }
    }
    private void Start()
    {
        EventsPool.UpdateUIEvent.Invoke();
        _observer = Observer.Instance;

    }
    void Update()
    {
        if (!_observer.Started)
            return;
        TimersPool.UpdateTimers(Time.deltaTime);
        _timeSinceStarted += Time.deltaTime;
    }
    private void StartGame()
    {
        spawnersObject.SetActive(true);
    }
    private void FinishGame(bool w)
    {
        spawnersObject.SetActive(false);
        EventsPool.ClearPoolsEvent.Invoke();
    }
    private void CollectGem(FillType t)
    {
        if (t != FillType.Diamond)
            return;
        _collectedGems++;
        PlayerStorage.CoinsCollected = _collectedGems;
        EventsPool.UpdateUIEvent.Invoke();
    }
    public void AddGem()
    {
        _collectedGems++;
    }
}
