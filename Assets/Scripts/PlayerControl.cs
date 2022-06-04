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

    private Joystick joystick;
    private Rigidbody mainRigidbody;
    float angle;
    private void Awake()
    {
        joystick = FindObjectOfType<Joystick>();
        mainRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontalMove = joystick.Horizontal;
        float verticalMove = joystick.Vertical;
        Vector3 newVelocity = new Vector3(horizontalMove * characterSpeed,0,verticalMove * characterSpeed);

        Vector3.ClampMagnitude(newVelocity, characterSpeed);
        mainRigidbody.velocity = newVelocity;

        float sign = (joystick.Direction.x < new Vector2(0, 1).x) ? -1.0f : 1.0f;
        angle = Vector3.Angle(joystick.Direction, new Vector2(0, 1)) * sign;

        characterTransform.rotation = Quaternion.Slerp(
            characterTransform.rotation,
            Quaternion.AngleAxis(angle, Vector3.up),
            Time.deltaTime * rotationSpeed);

        if (joystick.Horizontal == 0 && joystick.Vertical == 0)
            return;

    }
}
