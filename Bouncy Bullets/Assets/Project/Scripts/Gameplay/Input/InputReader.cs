using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }

    private const float StickDeadzone = 0.2f;
    private const float GamepadCursorSpeed = 1.5f;

    [SerializeField] private GameSettingsSO settings;

    private InputSystem_Actions controls;
    private Vector2 aimScreenPosition;

    public Vector2 MoveInput => controls.Player.Move.ReadValue<Vector2>();
    public bool ShootTriggered => controls.Player.Shoot.triggered;
    public bool TurboPressed => controls.Player.Turbo.WasPressedThisFrame();
    public bool ShieldPressed => controls.Player.Shield.triggered;
    public bool ShockwavePressed => controls.Player.ShockWave.WasPressedThisFrame();

    public Vector2 AimScreenPosition => aimScreenPosition;
    public InputDeviceKind ActiveDevice { get; private set; } = InputDeviceKind.Mouse;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        controls = new InputSystem_Actions();
        aimScreenPosition = new Vector2(Screen.width, Screen.height) * 0.5f;
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        HandleGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        controls.Dispose();
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing) controls.Player.Enable();
        else controls.Player.Disable();
    }

    private void Update()
    {
        DetectActiveDevice();
        AdvanceAimCursor();
    }

    private void DetectActiveDevice()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null &&
            (gamepad.leftStick.ReadValue().sqrMagnitude > 0.1f ||
             gamepad.rightStick.ReadValue().sqrMagnitude > 0.1f ||
             gamepad.dpad.ReadValue().sqrMagnitude > 0.0f))
        {
            ActiveDevice = InputDeviceKind.Gamepad;
            return;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null &&
            (mouse.delta.ReadValue().sqrMagnitude > 1.0f ||
             mouse.leftButton.wasPressedThisFrame ||
             mouse.rightButton.wasPressedThisFrame))
        {
            ActiveDevice = InputDeviceKind.Mouse;
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.anyKey.isPressed)
        {
            ActiveDevice = InputDeviceKind.Keyboard;
        }
    }

    private void AdvanceAimCursor()
    {
        Vector2 delta;

        if (ActiveDevice == InputDeviceKind.Gamepad)
        {
            Vector2 stick = controls.Player.Aim.ReadValue<Vector2>();
            if (stick.sqrMagnitude < StickDeadzone * StickDeadzone) stick = Vector2.zero;

            delta = stick * (GamepadCursorSpeed * Screen.height * Time.deltaTime);
        }
        else
        {
            delta = controls.Player.Look.ReadValue<Vector2>();
        }

        aimScreenPosition += delta * settings.AimSensitivity;
        aimScreenPosition.x = Mathf.Clamp(aimScreenPosition.x, 0f, Screen.width);
        aimScreenPosition.y = Mathf.Clamp(aimScreenPosition.y, 0f, Screen.height);
    }

}
