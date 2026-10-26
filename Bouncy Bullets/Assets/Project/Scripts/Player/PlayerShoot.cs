using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class PlayerShoot : MonoBehaviour
{
    private InputSystem_Actions controls;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 1f;
    private float nextFireTime = 0f;
    private float spawnMargin = 1.5f;

    [Header("References")]
    [SerializeField] private BulletPoolManager bulletPoolManager;

    [Header("Event Channels")]
    [SerializeField] private GameEventChannelSO shootEventChannel;
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

    private void Awake()
    {
        controls = new InputSystem_Actions();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        if (controls.Player.Shoot.triggered && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (shootEventChannel != null)
        {
            shootEventChannel.RaiseEvent();
        }

        Vector3 spawnPosition = transform.position + transform.forward * spawnMargin;

        if (spawnBulletEventChannel != null)
        {
            spawnBulletEventChannel.RaiseEvent(BulletType.Bouncy, spawnPosition, transform.rotation);
        }
    }
}
