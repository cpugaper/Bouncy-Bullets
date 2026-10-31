using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float spawnMargin = 1.5f;

    [Header("Event Channels")]
    [SerializeField] private GameEventChannelSO shootEventChannel;
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

    private float nextFireTime = 0f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.ShootTriggered && Time.time >= nextFireTime)
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
