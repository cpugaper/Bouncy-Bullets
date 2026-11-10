using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
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
        private InputReader input;

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

        private void Start()
        {
            input = InputReader.Instance;
        }

        void Update()
        {
            HandleTurbo();
        }

        private void HandleTurbo()
        {
            if (!IsBoosting && IsFull && input.TurboPressed)
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
}
