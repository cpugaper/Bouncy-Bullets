using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private InputSystem_Actions controls;
    public static PlayerInputManager Instance { get; private set; }

    public Vector2 MoveInput => controls.Player.Move.ReadValue<Vector2>();
    public bool ShootTriggered => controls.Player.Shoot.triggered;
    public bool TurboPressed => controls.Player.Turbo.WasPressedThisFrame();
    public bool ShieldPressed => controls.Player.Shield.triggered;
    public bool ShockwavePressed => controls.Player.ShockWave.WasPressedThisFrame();

    public InputSystem_Actions Controls => controls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        controls = new InputSystem_Actions();
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();
    private void OnDestroy() => controls.Dispose();

}
