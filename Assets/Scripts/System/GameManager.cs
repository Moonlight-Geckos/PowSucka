using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private float suctionVelocity = 30;

    #endregion

    #region Private

    private static Transform playerTransform;
    private static GameManager _instance;
    private static UnityEvent clearPoolsEvent = new UnityEvent();
    private static int collectedGems;
    private static bool started;
    #endregion

    static public Transform PlayerTransform
    {
        get { return playerTransform; }
    }
    static public bool Started
    {
        get { return started; }
        set { started = value; }
    }
    static public GameManager Instance
    {
        get { return _instance; }
    }
    static public UnityEvent ClearPoolsEvent
    {
        get { return clearPoolsEvent; }
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
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            started = false;
        }
    }

    void Update()
    {
        TimersPool.UpdateTimers(Time.deltaTime);
    }
    public void AddGem()
    {
        collectedGems++;
    }
}
