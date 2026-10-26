using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolManager : MonoBehaviour
{
    [Header("Bullet Prefab")]
    [SerializeField] private Bullet bouncyBulletPrefab;
    [SerializeField] private Bullet homingBulletPrefab;

    [Header("Event Channels")]
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<Bullet> bouncyPool;
    private ObjectPool<Bullet> homingPool;

    private void Awake()
    {
        InitializePools();
    }

    private void OnEnable()
    {
        if (spawnBulletEventChannel != null) spawnBulletEventChannel.Subscribe(SpawnBullet);
    }

    private void OnDisable()
    {
        if (spawnBulletEventChannel != null) spawnBulletEventChannel.Unsubscribe(SpawnBullet);
    }

    private void InitializePools()
    {
        bouncyPool = CreatePool(bouncyBulletPrefab);
        homingPool = CreatePool(homingBulletPrefab);
    }

    private ObjectPool<Bullet> CreatePool(Bullet prefab)
    {
        ObjectPool<Bullet> pool = null;
        pool = new ObjectPool<Bullet>(
            createFunc: () => CreateBulletInstance(prefab, pool),
            actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        return pool;
    }

    private Bullet CreateBulletInstance(Bullet prefab, ObjectPool<Bullet> targetPool)
    {
        Bullet instance = Instantiate(prefab, transform);
        instance.Initialize(() => targetPool.Release(instance));
        return instance;
    }

    private void SpawnBullet(BulletType type, Vector3 position, Quaternion rotation)
    {
        Bullet poolBullet = type switch
        {
            BulletType.Bouncy => bouncyPool.Get(),
            BulletType.Homing => homingPool.Get(),
            _ => null
        };

        if (poolBullet != null)
        {
            poolBullet.Launch(position, rotation);
        }
    }
}
