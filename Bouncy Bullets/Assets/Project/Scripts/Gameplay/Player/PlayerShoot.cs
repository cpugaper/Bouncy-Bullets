using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float fireInterval = 1f;
    [SerializeField] private float spawnMargin = 1.5f;

    [Header("Event Channels")]
    [SerializeField] private GameEventChannelSO shootEventChannel;
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

    private float nextFireTime = 0f;
    private InputReader input;

    private void Start()
    {
        input = InputReader.Instance;
    }

    void Update()
    {
        if (input.ShootTriggered && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireInterval;
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
