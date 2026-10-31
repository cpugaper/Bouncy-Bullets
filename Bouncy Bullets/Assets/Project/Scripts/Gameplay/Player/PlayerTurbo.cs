using UnityEngine;

public class PlayerTurbo : MonoBehaviour
{
    [Header("Turbo Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float drainPerSecond = 50f;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float turboImpulseForce = 15f;

    [Header("Event Channels")]
    [SerializeField] private FloatEventChannelSO turboChangedEventChannel; 
    [SerializeField] private GameEventChannelSO runStartedEventChannel;      

    private float energy;

    public bool IsBoosting { get; private set; }
    public bool IsFull => energy >= maxEnergy;
    public float SpeedMultiplier => IsBoosting ? speedMultiplier : 1f;

    private void OnEnable()
    {
        if (runStartedEventChannel != null) runStartedEventChannel.Subscribe(NotifyEnergy);
    }

    private void OnDisable()
    {
        if (runStartedEventChannel != null) runStartedEventChannel.Unsubscribe(NotifyEnergy);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        HandleTurbo();
    }

    private void HandleTurbo()
    {
        if (PlayerInputManager.Instance == null) return;

        if (!IsBoosting && IsFull && PlayerInputManager.Instance.TurboPressed)
        {
            IsBoosting = true;
            ApplyTurboImpulse();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayTurbo();
            }
        }

        if (IsBoosting)
        {
            ApplyTurboDrain();
        }
    }

    private void ApplyTurboImpulse()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            Vector3 boostDir = transform.forward;
            rb.AddForce(boostDir * turboImpulseForce, ForceMode.Impulse);
        }
    }

    private void ApplyTurboDrain()
    {
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
