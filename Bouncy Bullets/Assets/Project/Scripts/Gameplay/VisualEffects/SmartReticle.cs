using UnityEngine;

public class SmartReticle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer reticleRenderer;

    [Header("Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material lockedMaterial;

    [Header("Aim Settings")]
    [SerializeField] private float sphereCastRadius = 1.0f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual Settings")]
    [SerializeField] private float smoothSpeed = 25f;
    [SerializeField] private float normalHeightOffset = 0.05f;
    [SerializeField] private float lockedHeightOffset = 3f;

    [Header("Settings")]
    [SerializeField] private GameSettingsSO settings;

    private Camera mainCamera;
    private Transform playerTransform;
    private bool isPlaying = true;

    private void Start()
    {
        mainCamera = Camera.main;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        SetMaterial(normalMaterial);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        isPlaying = state == GameState.Playing;
        if (reticleRenderer != null) reticleRenderer.enabled = isPlaying;
    }

    private void LateUpdate()
    {
        if (!isPlaying || mainCamera == null) return;

        if (playerTransform == null)
        {
            if (PlayerMovement.Instance == null) return;
            playerTransform = PlayerMovement.Instance.transform;
        }

        Vector3 playerPos = playerTransform.position;
        float sensitivity = settings != null ? settings.MouseSensitivity : 1f;

        if (!PointerUtils.TryGetAimInfo(PlayerInputManager.Instance.Controls, mainCamera, playerPos, sensitivity, true, out Vector3 direction, out float distance))
            return;

        bool isEnemyTargeted = EvaluateTarget(playerPos, direction, distance);
        UpdateReticlePosition(playerPos, direction, distance, isEnemyTargeted);
    }

    private bool EvaluateTarget(Vector3 playerPos, Vector3 direction, float distance)
    {
        bool isEnemyTargeted = Physics.SphereCast(playerPos, sphereCastRadius, direction, out _, distance, enemyLayer);

        SetMaterial(isEnemyTargeted ? lockedMaterial : normalMaterial);

        return isEnemyTargeted;
    }

    private void UpdateReticlePosition(Vector3 playerPos, Vector3 direction, float distance, bool isEnemyTargeted)
    {
        Vector3 targetReticlePos = playerPos + direction * distance;

        float heightOffset = isEnemyTargeted ? lockedHeightOffset : normalHeightOffset;
        targetReticlePos.y = playerPos.y + heightOffset;

        transform.position = Vector3.Lerp(transform.position, targetReticlePos, Time.deltaTime * smoothSpeed);
    }

    private void SetMaterial(Material material)
    {
        if (reticleRenderer == null || material == null) return;
        if (reticleRenderer.sharedMaterial != material) reticleRenderer.sharedMaterial = material;
    }
}