using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    [SerializeField] private float initialImpulse = 8f;

    [Header("Rotation Settings")]
    [SerializeField] private float baseRotationSpeed = 720f;
    [SerializeField] private GameSettingsSO settings;

    [Header("References")]
    private Rigidbody playerRb;
    private Camera mainCamera;
    private PlayerTilt playerTilt;
    private InputSystem_Actions controls;
    private PlayerTurbo turbo;

    public static PlayerController Instance { get; private set; }

    [Header("Effects")]
    public ParticleSystem thrusterParticles;

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

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Disable();
    void OnDestroy() => controls.Dispose();

    void Start()
    {
        playerTilt = GetComponent<PlayerTilt>();
        playerRb = GetComponent<Rigidbody>();
        turbo = GetComponent<PlayerTurbo>();
        mainCamera = Camera.main;

        ApplyInitialImpulse();
    }

    private void ApplyInitialImpulse()
    {
        if (playerRb != null)
            playerRb.AddForce(transform.forward * initialImpulse, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        ConstrainPlayerPosition();
    }

    private void MovePlayer()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        HandleThrusterEffect(turbo != null && turbo.IsBoosting);

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        float multiplier = turbo != null ? turbo.SpeedMultiplier : 1f;
        playerRb.AddForce(moveDirection * speed * multiplier);
        if (playerTilt != null) playerTilt.ApplyTilt(moveDirection);
    }

    private void HandleThrusterEffect(bool isMoving)
    {
        if (thrusterParticles == null) return;
        ParticleSystem.EmissionModule emission = thrusterParticles.emission;
        emission.enabled = isMoving;
    }

    private void RotatePlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        float sensitivity = settings != null ? settings.MouseSensitivity : 1f;
        if (PointerUtils.TryGetAimInfo(controls, mainCamera, transform.position, sensitivity, false, out Vector3 direction, out _))
        {
            float targetAngleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);

            float currentTurnSpeed = baseRotationSpeed * sensitivity;
            Quaternion newRotation = Quaternion.RotateTowards(playerRb.rotation, targetRotation, currentTurnSpeed * Time.fixedDeltaTime);

            playerRb.MoveRotation(newRotation);
        }
    }

    // Prevents the player from leaving the screen 
    private void ConstrainPlayerPosition()
    {
        ScreenBounds bounds = ScreenBounds.Instance;
        if (bounds == null) return;

        Vector3 currentPos = playerRb.position;
        Vector3 currentVelocity = playerRb.linearVelocity;
        bool isOutOfBounds = false;

        if (currentPos.z > bounds.PlayerMaxZ)
        {
            currentPos.z = bounds.PlayerMaxZ;
            currentVelocity.z = 0f;
            isOutOfBounds = true;
        }
        else if (currentPos.z < bounds.PlayerMinZ)
        {
            currentPos.z = bounds.PlayerMinZ;
            currentVelocity.z = 0f;
            isOutOfBounds = true;
        }

        if (currentPos.x > bounds.PlayerMaxX)
        {
            currentPos.x = bounds.PlayerMaxX;
            currentVelocity.x = 0f;
            isOutOfBounds = true;
        }
        else if (currentPos.x < bounds.PlayerMinX)
        {
            currentPos.x = bounds.PlayerMinX;
            currentVelocity.x = 0f;
            isOutOfBounds = true;
        }

        if (isOutOfBounds)
        {
            playerRb.position = currentPos;
            playerRb.linearVelocity = currentVelocity;
        }
    }
}
