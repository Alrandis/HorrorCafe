using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform cameraYawTarget;   // empty child on player used for yaw (usually player root)
    public Transform cameraPitchTarget; // child of yaw target, positioned at eye height - this is LookAt for vcam

    public float sensitivityX = 0.15f; // yaw (degrees per input unit)
    public float sensitivityY = 0.12f; // pitch

    public float minPitch = -60f;
    public float maxPitch = 75f;

    private float _pitch = 0f; // up/down
    private float _yaw = 0f;   // left/right

    private void Start()
    {
        if (cameraYawTarget == null) cameraYawTarget = transform; // default to player transform
        if (cameraPitchTarget == null) Debug.LogWarning("PlayerLook: cameraPitchTarget not assigned. Assign in inspector.");
        // Initial yaw/pitch from transforms
        _yaw = cameraYawTarget.eulerAngles.y;
        if (cameraPitchTarget != null) _pitch = cameraPitchTarget.localEulerAngles.x;
        // localEulerAngles may be >180; normalize
        if (_pitch > 180f) _pitch -= 360f;
    }

    private void Update()
    {
        if (InputPlayerManager.Instance == null) return;

        Vector2 look = InputPlayerManager.Instance.Look;
        // Scale look by sensitivity. Mouse input is usually larger than gamepad, so adjust externally if needed.
        float deltaYaw = look.x * sensitivityX;
        float deltaPitch = -look.y * sensitivityY; // invert Y for natural mouse-look (adjust as desired)

        _yaw += deltaYaw;
        _pitch += deltaPitch;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        // Apply yaw to the yaw target (rotate around Y)
        cameraYawTarget.rotation = Quaternion.Euler(0f, _yaw, 0f);

        // Apply pitch as local rotation of pitch target (so pitch doesn't tilt the body)
        if (cameraPitchTarget != null)
            cameraPitchTarget.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}
