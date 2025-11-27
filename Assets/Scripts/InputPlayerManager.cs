using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputPlayerManager : MonoBehaviour
{
    public static InputPlayerManager Instance { get; private set; }

    [Header("Input Actions")]
    public InputActionAsset InputActions; // Assign PlayerInputActions asset in inspector
    public string ActionMapName = "Player"; // name of the action map
    public string MoveActionName = "Move";
    public string LookActionName = "Look";
    public string SprintActionName = "Sprint";

    // Public properties to be read by other systems
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Sprint { get; private set; }

    // Internal actions (cached)
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _sprintAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        if (InputActions == null)
        {
            Debug.LogError("InputPlayerManager: InputActionAsset is not assigned.");
            enabled = false;
            return;
        }

        var map = InputActions.FindActionMap(ActionMapName, true);
        _moveAction = map.FindAction(MoveActionName, true);
        _lookAction = map.FindAction(LookActionName, true);
        _sprintAction = map.FindAction(SprintActionName, true);

        _moveAction.Enable();
        _lookAction.Enable();
        _sprintAction.Enable();

        // Optionally lock cursor at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        // tear down
        _moveAction?.Disable();
        _lookAction?.Disable();
        _sprintAction?.Disable();

        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        // Polling values each frame. Straightforward and predictable.
        Move = _moveAction.ReadValue<Vector2>();
        Look = _lookAction.ReadValue<Vector2>();
        Sprint = _sprintAction.ReadValue<float>() > 0.5f; // treat as button
    }
}
