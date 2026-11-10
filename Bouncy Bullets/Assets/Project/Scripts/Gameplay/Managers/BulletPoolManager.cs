using UnityEngine;
using UnityEngine.Pool;
using Game.Core;

namespace Game.Gameplay
{
    public class BulletPoolManager : MonoBehaviour
    {
        [Header("Bullet Prefab")]
        [SerializeField] private BouncyBullet bouncyBulletPrefab;
        [SerializeField] private HomingBullet homingBulletPrefab;

        [Header("Event Channels")]
        [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

        [Header("Pool Settings")]
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 30;

        private ObjectPool<BouncyBullet> bouncyPool;
        private ObjectPool<HomingBullet> homingPool;

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

        private ObjectPool<T> CreatePool<T>(T prefab) where T : Bullet
        {
            ObjectPool<T> pool = null;
            pool = new ObjectPool<T>(
                createFunc: () => CreateBulletInstance(prefab, pool),
                actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
                actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
                actionOnDestroy: (bullet) => Destroy(bullet.gameObject),
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
            return pool;
        }

        private T CreateBulletInstance<T>(T prefab, ObjectPool<T> targetPool) where T : Bullet
        {
            T instance = Instantiate(prefab, transform);
            instance.SetDespawnCallback(() => targetPool.Release(instance));
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
}
