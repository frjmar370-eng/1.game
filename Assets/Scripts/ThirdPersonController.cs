using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public float walkSpeed = 5.5f;
    public float sprintSpeed = 8f;
    public float acceleration = 18f;
    public float turnSpeed = 12f;
    public float gravity = -24f;
    public float jumpHeight = 1.25f;
    public Transform cameraTarget;

    CharacterController controller;
    Vector3 planarVelocity;
    float verticalVelocity;
    float yaw;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.height = 3.8f;
        controller.radius = .55f;
        controller.center = new Vector3(0, 1.9f, 0);
        controller.stepOffset = .45f;
        controller.slopeLimit = 48f;
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        Vector2 input = MobileControls.Move;
        Vector3 forward = Camera.main ? Camera.main.transform.forward : Vector3.forward;
        Vector3 right = Camera.main ? Camera.main.transform.right : Vector3.right;
        forward.y = right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 desired = forward * input.y + right * input.x;
        if (desired.sqrMagnitude > 1f) desired.Normalize();
        float targetSpeed = input.magnitude > .75f ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = desired * targetSpeed * Mathf.Clamp01(input.magnitude);
        planarVelocity = Vector3.MoveTowards(planarVelocity, targetVelocity, acceleration * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = -2f;
            if (input.y > .85f && MobileControls.JumpPressed)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        verticalVelocity += gravity * Time.deltaTime;

        CollisionFlags flags = controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f) verticalVelocity = 0f;

        if (desired.sqrMagnitude > .02f)
        {
            Quaternion target = Quaternion.LookRotation(desired, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, 1f - Mathf.Exp(-turnSpeed * Time.deltaTime));
            yaw = transform.eulerAngles.y;
        }

        Vector2 look = MobileControls.Look;
        if (look.sqrMagnitude > .001f) yaw += look.x * 150f * Time.deltaTime;
        if (cameraTarget) cameraTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        MobileControls.JumpPressed = false;
    }
}
