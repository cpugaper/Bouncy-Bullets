using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ExplosionPoolManager : MonoBehaviour
{
    [Header("Explosion Prefab")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 25;

    [Header("Event Channels")]
    [SerializeField] private PositionEventChannelSO enemyDeathEventChannel;
    [SerializeField] private PositionEventChannelSO playerDeathEventChannel;

    private ObjectPool<GameObject> explosionPool;

    private void Awake()
    {
        explosionPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(explosionPrefab, transform),
            actionOnGet: (explosion) => explosion.SetActive(true),
            actionOnRelease: (explosion) => explosion.SetActive(false),
            actionOnDestroy: (explosion) => Destroy(explosion),
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private void OnEnable()
    {
        if (enemyDeathEventChannel != null) enemyDeathEventChannel.Subscribe(SpawnExplosion);
        if (playerDeathEventChannel != null) playerDeathEventChannel.Subscribe(SpawnExplosion);
    }

    private void OnDisable()
    {
        if (enemyDeathEventChannel != null) enemyDeathEventChannel.Unsubscribe(SpawnExplosion);
        if (playerDeathEventChannel != null) playerDeathEventChannel.Unsubscribe(SpawnExplosion);
    }

    private void SpawnExplosion(Vector3 position)
    {
        GameObject explosion = explosionPool.Get();
        explosion.transform.position = position;

        float duration = 1.0f;
        if (explosion.TryGetComponent(out ParticleSystem particles))
        {
            duration = particles.main.duration;
        }

        StartCoroutine(ReturnToPoolAfterDelay(explosion, duration));
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject explosion, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (explosion.activeSelf)
        {
            explosionPool.Release(explosion);
        }
    }
}
