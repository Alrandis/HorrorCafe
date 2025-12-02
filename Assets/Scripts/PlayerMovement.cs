using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform CameraTransform; // used to align move direction with camera forward (assign Main Camera or camera target)
    private CharacterController _cc;

    public float BaseSpeed = 3.5f;
    public float SprintMultiplier = 1.8f;
    [Range(0.4f, 1f)] public float BackwardMultiplier = 0.75f;
    [Range(0.6f, 1f)] public float StrafeMultiplier = 0.85f;

    public float AccelTime = 0.08f;   // how quickly we reach target speed
    public float DecelTime = 0.06f;   // braking slightly faster

    public float Gravity = -9.81f;
    public float GroundedCheckDist = 0.15f;
    public LayerMask GroundMask;

    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _velocitySmoothDampVel;
    private float _verticalVelocity = 0f;
    private float _currentSpeed = 0f;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        if (CameraTransform == null)
            CameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (InputPlayerManager.Instance == null) return;

        Vector2 input = InputPlayerManager.Instance.Move;
        bool sprint = InputPlayerManager.Instance.Sprint;

        // Convert input vector (x = left/right, y = forward/back) to world space direction relative to camera yaw
        Vector3 forward = Vector3.Scale(new Vector3(CameraTransform.forward.x, 0, CameraTransform.forward.z), Vector3.one).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector3 desiredMove = (forward * input.y + right * input.x);
        if (desiredMove.sqrMagnitude > 1f) desiredMove.Normalize();

        // Determine desired speed based on direction
        float targetSpeed = BaseSpeed;
        if (sprint) targetSpeed *= SprintMultiplier;

        // Reduce speed when moving backward or strafing
        if (input.y < -0.01f) targetSpeed *= BackwardMultiplier;
        // For strafing: if significant x component but small y
        if (Mathf.Abs(input.x) > 0.01f && Mathf.Abs(input.y) <= Mathf.Abs(input.x))
            targetSpeed *= StrafeMultiplier;

        // Compute scalar current speed smoothing
        float targetSpeedScalar = desiredMove.magnitude * targetSpeed;
        float smoothTime = (targetSpeedScalar > _currentSpeed) ? AccelTime : DecelTime;
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeedScalar, ref _velocitySmoothDampVel.x, smoothTime);

        Vector3 horizontalVelocity = desiredMove * _currentSpeed;

        // Gravity & grounding
        if (_cc.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f; // small downward force to keep grounded
        }
        _verticalVelocity += Gravity * Time.deltaTime;

        Vector3 finalVelocity = horizontalVelocity + Vector3.up * _verticalVelocity;

        _cc.Move(finalVelocity * Time.deltaTime);
    }
}
