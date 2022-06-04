using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Serialized

    #endregion

    #region Private

    private static Transform playerPos;
    private static GameManager _instance;
    private static UnityEvent clearPoolsEvent = new UnityEvent();
    private static int collectedGems;
    private static bool started;
    #endregion

    static public Vector3 PlayerPos
    {
        get { return playerPos.position; }
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

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
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
