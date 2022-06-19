using UnityEngine;
public class Observer : MonoBehaviour
{
    private static Observer _instance;

    public static bool weaponMode = false;
    public static float weaponDuration = 1;
    public static bool playerMoving = false;

    private static Transform _playerTransform;
    private static Transform _vacuumTransform;
    private static int _leftEnemiesToKill;
    private static bool _started;
    private static bool _finished;

    public int LeftEnemiesToKill
    {
        get { return _leftEnemiesToKill; }
    }
    public bool Finished
    {
        get { return _finished; }
        set { _finished = value; }
    }
    public bool Started
    {
        get { return _started; }
        set { _started = value; }
    }
    public Transform VacuumTransform
    {
        get { return _vacuumTransform; }
    }
    public Transform PlayerTransform
    {
        get { return _playerTransform; }
    }

    public static Observer Instance
    {
        get { return _instance; }
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
            weaponMode = false;
            playerMoving = false;
            _leftEnemiesToKill = GameManager.Instance.EnemiesToKill;
            EventsPool.PlayerChangedMovementEvent.AddListener((bool m) => playerMoving = m);
            EventsPool.ChangePhaseEvent.AddListener((bool wm) => weaponMode = wm);
            EventsPool.EnemyDiedEvent.AddListener(EnemyDied);
            EventsPool.GameStartedEvent.AddListener(StartGame);
            EventsPool.GameFinishedEvent.AddListener(FinishGame);
        }
    }
    private void StartGame()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _vacuumTransform = GameObject.FindGameObjectWithTag("Vacuum").transform;
        _started = true;
        weaponMode = false;
    }
    private void FinishGame(bool w)
    {
        _started = false;
    }
    private void EnemyDied()
    {
        if (!_started)
            return;
        _leftEnemiesToKill--;
        EventsPool.UpdateUIEvent.Invoke();
        if (_leftEnemiesToKill <= 0)
            EventsPool.GameFinishedEvent.Invoke(true);
    }
}
