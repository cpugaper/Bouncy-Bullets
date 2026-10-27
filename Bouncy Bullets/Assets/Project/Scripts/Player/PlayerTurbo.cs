using UnityEngine;

public class PlayerTurbo : MonoBehaviour
{
    [Header("Turbo Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float drainPerSecond = 50f;
    [SerializeField] private float speedMultiplier = 2f;

    [Header("Event Channels")]
    [SerializeField] private FloatEventChannelSO turboChangedEventChannel; 
    [SerializeField] private GameEventChannelSO runStartedEventChannel;      

    private InputSystem_Actions controls;
    private float energy;

    public bool IsBoosting { get; private set; }
    public bool IsFull => energy >= maxEnergy;
    public float SpeedMultiplier => IsBoosting ? speedMultiplier : 1f;

    private void Awake() => controls = new InputSystem_Actions();

    private void OnEnable()
    {
        controls.Player.Enable();
        if (runStartedEventChannel != null) runStartedEventChannel.Subscribe(NotifyEnergy);
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        if (runStartedEventChannel != null) runStartedEventChannel.Unsubscribe(NotifyEnergy);
    }


    private void OnDestroy() => controls.Dispose();

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        if (!IsBoosting)
        {
            if (IsFull && controls.Player.Turbo.WasPressedThisFrame())
            {
                IsBoosting = true;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayTurbo();
                }
            }
            return;
        }

        energy = Mathf.Max(0f, energy - drainPerSecond * Time.deltaTime);
        NotifyEnergy();
        if (energy <= 0f) IsBoosting = false;
    }

    public bool TryAddEnergy(float amount)
    {
        if (IsFull || IsBoosting) return false;

        energy = Mathf.Min(maxEnergy, energy + amount);
        NotifyEnergy();
        return true;
    }


    private void NotifyEnergy()
    {
        if (turboChangedEventChannel != null) turboChangedEventChannel.RaiseEvent(energy / maxEnergy);
    }
}
