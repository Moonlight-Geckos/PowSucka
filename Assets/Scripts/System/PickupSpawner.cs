
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{

    #region Serialized

    [SerializeField]
    private int maxPickupsOnGround = 50;

    [SerializeField]
    private PickupPool[] pickupPools;

    [SerializeField]
    private float spawnInterval = 2;

    [SerializeField]
    private float spawnOffsetFromCharacter = 40;

    #endregion

    private static PickupSpawner _instance;
    private static Timer _spawnTimer;
    private Pickup _pckup;
    private Vector3 _centerPosition;
    private Quaternion _newRot;
    private float _randomAngle;
    private int _lastPickup;
    private int _pickupsOnGround = 0;

    public static PickupSpawner Instance
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
            EventsPool.PickedupObjectEvent.AddListener((FillType f) =>
            {
                if (!_spawnTimer.Running)
                    _spawnTimer.Run();
                _pickupsOnGround--;
            });

        }
    }
    private void Start()
    {
        Spawn();
    }
    void Spawn()
    {
        if (_pickupsOnGround >= maxPickupsOnGround)
            return;
        _centerPosition = GameManager.Instance.PlayerTransform.position;
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

        if (_lastPickup >= pickupPools.Length)
            _lastPickup = 0;
        _pckup = pickupPools[_lastPickup++].Pool.Get();
        _pckup.transform.position = _centerPosition;
        _newRot = Random.rotation;
        _newRot.x = _newRot.z = 0;
        _pckup.transform.rotation = _newRot;
        _pickupsOnGround++;
        if (Observer.weaponMode)
        {
            _spawnTimer.Duration = spawnInterval / 4f;
        }
        else
            _spawnTimer.Duration = spawnInterval;
        _spawnTimer.Run();
    }
}
