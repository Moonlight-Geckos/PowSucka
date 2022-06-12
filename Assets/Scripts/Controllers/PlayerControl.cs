using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    #region Serialized

    [Range(1f, 20f)]
    [SerializeField]
    private float characterSpeed = 7;

    [Range(1f, 20f)]
    [SerializeField]
    private float rotationSpeed = 4;

    [SerializeField]
    private Transform characterTransform;

    #endregion

    private Joystick _joystick;
    private Rigidbody _mainRigidbody;
    private CharacterCustomizer _characterCustomizer;
    private float _angle;
    private bool _isRunning;
    private Vector3 newVelocity;
    private float horizontalMove;
    private float verticalMove;
    private void Awake()
    {
        _joystick = FindObjectOfType<Joystick>();
        _mainRigidbody = GetComponent<Rigidbody>();
        _characterCustomizer = characterTransform.GetComponentInChildren<CharacterCustomizer>();
    }

    private void Update()
    {
        horizontalMove = _joystick.Horizontal;
        verticalMove = _joystick.Vertical;

        if (_joystick.Horizontal == 0 && _joystick.Vertical == 0)
        {
            if (_isRunning)
            {
                _isRunning = false;
                _characterCustomizer.Run(_isRunning);
            }
            _mainRigidbody.velocity = Vector3.zero;
            return;
        }
        newVelocity = new Vector3(horizontalMove * characterSpeed,0,verticalMove * characterSpeed);

        Vector3.ClampMagnitude(newVelocity, characterSpeed);
        _mainRigidbody.velocity = newVelocity;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -WorldLimits.XLimits, WorldLimits.XLimits),
            transform.position.y,
            Mathf.Clamp(transform.position.z, -WorldLimits.ZLimits, WorldLimits.ZLimits)
            );

        if (!_isRunning)
        {
            _isRunning = true;
            _characterCustomizer.Run(_isRunning);
        }
        _angle = Vector3.Angle(_joystick.Direction, new Vector2(0, 1)) * ((_joystick.Direction.x < new Vector2(0, 1).x) ? -1.0f : 1.0f);

        characterTransform.rotation = Quaternion.Slerp(
            characterTransform.rotation,
            Quaternion.AngleAxis(_angle, Vector3.up),
            Time.deltaTime * rotationSpeed);
    }
}
