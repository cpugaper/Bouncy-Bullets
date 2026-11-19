using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float initialImpulse = 8f;

        [Header("Rotation Settings")]
        [SerializeField] private float baseRotationSpeed = 720f;

        [Header("Effects")]
        public ParticleSystem thrusterParticles;

        [Header("References")]
        [SerializeField] private TransformReferenceSO playerReference;

        private Rigidbody playerRb;
        private Camera mainCamera;
        private PlayerTilt playerTilt;
        private PlayerTurbo turbo;
        private InputReader input;

        private void OnEnable() => playerReference.Register(transform);
        private void OnDisable() => playerReference.Unregister(transform);

        void Start()
        {
            playerTilt = GetComponent<PlayerTilt>();
            playerRb = GetComponent<Rigidbody>();
            turbo = GetComponent<PlayerTurbo>();
            mainCamera = Camera.main;
            input = InputReader.Instance;

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
            Vector2 moveInput = input.MoveInput;

            HandleThrusterEffect(turbo != null && turbo.IsBoosting);

            Vector3 moveDirection = new(moveInput.x, 0f, moveInput.y);
            float multiplier = turbo != null ? turbo.SpeedMultiplier : 1f;
            playerRb.AddForce(moveDirection * speed * multiplier);

            if (playerTilt != null) playerTilt.ApplyTilt(moveDirection);
        }

        private void HandleThrusterEffect(bool isTurboOn)
        {
            if (thrusterParticles == null) return;
            ParticleSystem.EmissionModule emission = thrusterParticles.emission;
            emission.enabled = isTurboOn;
        }

        private void RotatePlayer()
        {
            if (PointerUtils.TryGetAimInfo(mainCamera, input.AimScreenPosition, transform.position, out Vector3 direction, out _))
            {
                float targetAngleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngleY, 0f);
                Quaternion newRotation = Quaternion.RotateTowards(playerRb.rotation, targetRotation, baseRotationSpeed * Time.fixedDeltaTime);

                playerRb.MoveRotation(newRotation);
            }
        }

        private void ConstrainPlayerPosition()
        {
            ScreenBounds bounds = ScreenBounds.Instance;

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
}