
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MetroPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float gravity = -20f;

    [Header("Camera")]
    public Camera playerCamera;
    public float cameraDistance = 4f;
    public float cameraHeight = 2f;
    public float sensitivity = 0.12f;

    private CharacterController controller;
    private float verticalVelocity;
    private float yaw;
    private float pitch = 15f;
    private Vector2 movement;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        ReadControls();
        MoveCharacter();
        FollowCamera();
    }

    private void ReadControls()
    {
        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.position.x < Screen.width / 2f)
            {
                if (touch.phase == TouchPhase.Began)
                    movement = Vector2.zero;
                else
                    movement = Vector2.ClampMagnitude(
                        touch.deltaPosition / 40f, 1f
                    );
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                yaw += touch.deltaPosition.x * sensitivity;
                pitch -= touch.deltaPosition.y * sensitivity;
                pitch = Mathf.Clamp(pitch, -10f, 55f);
            }
        }
    }

    private void MoveCharacter()
    {
        Vector3 forward =
            Quaternion.Euler(0, yaw, 0) * Vector3.forward;

        Vector3 right =
            Quaternion.Euler(0, yaw, 0) * Vector3.right;

        Vector3 direction =
            forward * movement.y + right * movement.x;

        direction = Vector3.ClampMagnitude(direction, 1f);

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private void FollowCamera()
    {
        if (playerCamera == null) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 offset = rotation *
            new Vector3(0, cameraHeight, -cameraDistance);

        playerCamera.transform.position =
            transform.position + offset;

        playerCamera.transform.LookAt(
            transform.position + Vector3.up * 1.2f
        );
    }
}
