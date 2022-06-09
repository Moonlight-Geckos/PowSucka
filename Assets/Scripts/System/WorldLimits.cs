using UnityEngine;

public class WorldLimits : MonoBehaviour
{
    [SerializeField]
    [Range(0f,1000f)]
    private float x_Limits = 200;

    [SerializeField]
    [Range(0f, 1000f)]
    private float z_Limits = 220;

    private static WorldLimits _instance;
    public static WorldLimits Instance
    {
        get { return _instance; }
    }
    public static float XLimits
    {
        get; private set; 
    }
    public static float ZLimits
    {
        get; private set;
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
            XLimits = x_Limits;
            ZLimits = z_Limits;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(2*x_Limits, 5, 2*z_Limits));
    }
}
