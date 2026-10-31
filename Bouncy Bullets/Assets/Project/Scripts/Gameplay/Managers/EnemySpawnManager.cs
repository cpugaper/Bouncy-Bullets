using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject shooterEnemyPrefab;
    [SerializeField] private GameObject bomberEnemyPrefab;
    [SerializeField] private GameObject volatileEnemyPrefab;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private float maxDistanceFromPlayer = 10f;
    [SerializeField] private float ySpawn = 0.5f;
    [SerializeField] private float spawnInset = 0.5f; // So a moving camera cannot push the enemy out as it appears

    [Header("Anti-Clustering Settings")]
    [SerializeField] private float minDistanceBetweenEnemies = 2f;
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacityPerType = 10;
    [SerializeField] private int maxSizePerType = 20;

    [Header("Phases")]
    [SerializeField] private PhaseManager phaseManager;

    private readonly Dictionary<EnemyType, GameObject> prefabs = new Dictionary<EnemyType, GameObject>();
    private readonly Dictionary<EnemyType, ObjectPool<GameObject>> pools = new Dictionary<EnemyType, ObjectPool<GameObject>>();
    private readonly HashSet<GameObject> activeEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        Register(EnemyType.Normal, normalEnemyPrefab);
        Register(EnemyType.Shooter, shooterEnemyPrefab);
        Register(EnemyType.Bomber, bomberEnemyPrefab);
        Register(EnemyType.Volatile, volatileEnemyPrefab);
    }

    private void Register(EnemyType type, GameObject prefab)
    {
        prefabs[type] = prefab;
        pools[type] = CreatePool(prefab);
    }

    private ObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        ObjectPool<GameObject> pool = null;
        pool = new ObjectPool<GameObject>(
            createFunc: () => CreateEnemyInstance(prefab, pool),
            actionOnGet: (enemy) =>
            {
                enemy.SetActive(true);
                activeEnemies.Add(enemy);
            },
            actionOnRelease: (enemy) =>
            {
                activeEnemies.Remove(enemy);
                enemy.SetActive(false);
            },
            actionOnDestroy: (enemy) => Destroy(enemy),
            defaultCapacity: defaultCapacityPerType,
            maxSize: maxSizePerType
        );
        return pool;
    }

    private GameObject CreateEnemyInstance(GameObject prefab, ObjectPool<GameObject> targetPool)
    {
        GameObject instance = Instantiate(prefab, transform);

        if (instance.TryGetComponent(out Enemy enemy))
        {
            enemy.SetDespawnCallback(() => targetPool.Release(instance));
        }
        return instance;
    }

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!phaseManager.IsSpawning)
            {
                yield return null;
                continue;
            }

            SpawnEnemy(phaseManager.CurrentPhase.PickEnemy());
            yield return new WaitForSeconds(phaseManager.CurrentInterval);
        }
    }

    private void SpawnEnemy(EnemyType type)
    {
        if (!pools.TryGetValue(type, out ObjectPool<GameObject> pool)) return;

        Vector3 spawnPos = GetValidSpawnPosition(type);
        GameObject enemy = pool.Get();
        enemy.transform.SetPositionAndRotation(spawnPos, prefabs[type].transform.rotation);
    }

    private Vector3 GetValidSpawnPosition(EnemyType type)
    {
        Rect area = GetSpawnArea();

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 candidatePos = GenerateSpawnPosition(type, area);

            if (!IsTooCloseToOtherEnemies(candidatePos))
            {
                return candidatePos;
            }
        }

        return GenerateSpawnPosition(type, area);
    }

    private Vector3 GenerateSpawnPosition(EnemyType type, Rect area)
    {
        switch (type)
        {
            case EnemyType.Normal: return new Vector3(Random.Range(area.xMin, area.xMax), ySpawn, area.yMax);
            case EnemyType.Bomber: return GetRandomPositionAwayFromPlayer(area);
            default: return GetRandomEdgePosition(area);
        }
    }

    private bool IsTooCloseToOtherEnemies(Vector3 pos)
    {
        Vector3 flatPos = new Vector3(pos.x, 0f, pos.z);
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;

            Vector3 enemyFlatPos = new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z);

            if (Vector3.Distance(flatPos, enemyFlatPos) < minDistanceBetweenEnemies) return true;
        }
        return false; 
    }

    private Rect GetSpawnArea()
    {
        Rect view = ScreenBounds.Instance.GetVisibleArea();
        view.xMin += spawnInset;
        view.xMax -= spawnInset;
        view.yMin += spawnInset;
        view.yMax -= spawnInset;
        return view;
    }

    private Vector3 GetRandomPositionAwayFromPlayer(Rect area)
    {
        Vector3 playerPos = PlayerMovement.Instance.transform.position;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

        Vector3 direction = new Vector3(randomDir.x, 0f, randomDir.y);
        Vector3 spawnPos = playerPos + direction * randomDistance;

        // Limits enemy from spawing outside the visible area
        spawnPos.x = Mathf.Clamp(spawnPos.x, area.xMin, area.xMax);
        spawnPos.z = Mathf.Clamp(spawnPos.z, area.yMin, area.yMax);
        spawnPos.y = ySpawn;

        return spawnPos;
    }

    private Vector3 GetRandomEdgePosition(Rect area)
    {
        int side = Random.Range(0, 4); // 0 = top, 1 = bottom, 2 = left, 3 = right

        switch (side)
        {
            case 0: return new Vector3(Random.Range(area.xMin, area.xMax), ySpawn, area.yMax);
            case 1: return new Vector3(Random.Range(area.xMin, area.xMax), ySpawn, area.yMin);
            case 2: return new Vector3(area.xMin, ySpawn, Random.Range(area.yMin, area.yMax));
            default: return new Vector3(area.xMax, ySpawn, Random.Range(area.yMin, area.yMax));
        }
    }
}
