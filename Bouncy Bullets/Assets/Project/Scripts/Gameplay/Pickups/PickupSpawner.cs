using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PickupSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnergyPickup energyPickupPrefab;
    [SerializeField] private ScorePickup scorePickupPrefab;
    [SerializeField] private ShockwavePickup shockwavePickupPrefab;

    [Header("Spawn Weights")]
    [SerializeField, Range(1, 100)] private int scoreWeight = 60;
    [SerializeField, Range(1, 100)] private int energyWeight = 45;
    [SerializeField, Range(1, 100)] private int shockwaveWeight = 30;

    [Header("Spawn Settings")]
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxSize = 15;
    [SerializeField] private float minDistanceFromPlayer = 4f;
    [SerializeField] private float edgePadding = 2f;

    [Header("Spawn Frequency")]
    [SerializeField] private float firstPhaseInterval = 8f;
    [SerializeField] private float reductionPerPhase = 1f;
    [SerializeField] private float minInterval = 4f;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO phaseChangedEventChannel;

    private ObjectPool<EnergyPickup> energyPool;
    private ObjectPool<ScorePickup> scorePool;
    private ObjectPool<ShockwavePickup> shockwavePool;

    private Coroutine spawnCoroutine;
    private WaitForSeconds wait;
    private int currentPhaseNumber = 1;

    private void Awake()
    {
        InitializePools();
        SetPhase(1);
    }
    
    private void OnEnable()
    {
        if (phaseChangedEventChannel != null) phaseChangedEventChannel.Subscribe(OnPhaseChanged);
    }

    private void OnDisable()
    {
        if (phaseChangedEventChannel != null) phaseChangedEventChannel.Unsubscribe(OnPhaseChanged);
    }

    private void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnPhaseChanged(int phaseNumber)
    {
        currentPhaseNumber = phaseNumber;
        SetPhase(phaseNumber);

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void InitializePools()
    {
        scorePool = CreatePool(scorePickupPrefab);
        energyPool = CreatePool(energyPickupPrefab);
        shockwavePool = CreatePool(shockwavePickupPrefab);
    }

    private ObjectPool<T> CreatePool<T>(T prefab) where T : Pickup
    {
        ObjectPool<T> pool = null;
        pool = new ObjectPool<T>(
            createFunc: () => CreatePickupInstance(prefab, pool),
            actionOnGet: (pickup) => pickup.gameObject.SetActive(true),
            actionOnRelease: (pickup) => pickup.gameObject.SetActive(false),
            actionOnDestroy: (pickup) => Destroy(pickup.gameObject),
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        return pool;
    }

    private T CreatePickupInstance<T>(T prefab, ObjectPool<T> targetPool) where T : Pickup
    {
        T instance = Instantiate(prefab, transform);
        instance.SetDespawnCallback(() => targetPool.Release(instance));
        return instance;
    }

    private void SetPhase(int phaseNumber)
    {
        float interval = Mathf.Max(minInterval, firstPhaseInterval - reductionPerPhase * (phaseNumber - 1));
        wait = new WaitForSeconds(interval);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return wait;

            int totalActive = scorePool.CountActive + energyPool.CountActive + shockwavePool.CountActive;
            if (totalActive < maxSize)
            {
                TrySpawn();
            }
        }
    }

    private void TrySpawn()
    {
        if (PlayerMovement.Instance == null || ScreenBounds.Instance == null) return;

        Rect view = ScreenBounds.Instance.GetVisibleArea();
        Vector3 pos = new Vector3(
            Random.Range(view.xMin + edgePadding, view.xMax - edgePadding),
            scorePickupPrefab.transform.position.y,
            Random.Range(view.yMin + edgePadding, view.yMax - edgePadding)
        );

        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        if (Vector3.Distance(pos, playerPos) < minDistanceFromPlayer) return;

        SpawnWeightedPickup(pos);
    }

    private void SpawnWeightedPickup(Vector3 spawnPosition)
    {
        bool isTurboFull = false;
        if (PlayerMovement.Instance.TryGetComponent(out PlayerTurbo turbo) && turbo.IsFull)
        {
            isTurboFull = true;
        }

        bool isShockwaveFull = false;
        if (PlayerMovement.Instance.TryGetComponent(out PlayerShockwave shockwaveSystem) && shockwaveSystem.IsFull)
        {
            isShockwaveFull = true;
        }

        int adjustedEnergyWeight = isTurboFull ? 0 : energyWeight;
        int adjustedShockwaveWeight = (currentPhaseNumber >= 3 && !isShockwaveFull) ? shockwaveWeight : 0;

        int totalWeight = scoreWeight + adjustedEnergyWeight + adjustedShockwaveWeight;
        if (totalWeight <= 0) totalWeight = scoreWeight;

        int randomValue = Random.Range(0, totalWeight);

        if (randomValue < scoreWeight)
        {
            scorePool.Get().transform.position = spawnPosition;
        }
        else if (randomValue < scoreWeight + adjustedEnergyWeight)
        {
            energyPool.Get().transform.position = spawnPosition;
        }
        else
        {
            shockwavePool.Get().transform.position = spawnPosition;
        }
    }
}
