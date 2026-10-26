using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PickupSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnergyPickup pickupPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxActive = 5;
    [SerializeField] private float minDistanceFromPlayer = 4f;
    [SerializeField] private float edgePadding = 2f;

    [Header("Spawn Frequency")]
    [SerializeField] private float firstPhaseInterval = 8f;
    [SerializeField] private float reductionPerPhase = 1f;
    [SerializeField] private float minInterval = 4f;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO phaseChangedEventChannel;

    private ObjectPool<EnergyPickup> pool;
    private WaitForSeconds wait;

    private void Awake()
    {
        pool = new ObjectPool<EnergyPickup>(
            createFunc: CreatePickup,
            actionOnGet: p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            defaultCapacity: maxActive,
            maxSize: maxActive * 2
        );

        SetPhase(1);
    }
    private void OnEnable()
    {
        if (phaseChangedEventChannel != null) phaseChangedEventChannel.Subscribe(SetPhase);
    }

    private void OnDisable()
    {
        if (phaseChangedEventChannel != null) phaseChangedEventChannel.Unsubscribe(SetPhase);
    }

    private void SetPhase(int phaseNumber)
    {
        float interval = Mathf.Max(minInterval, firstPhaseInterval - reductionPerPhase * (phaseNumber - 1));
        wait = new WaitForSeconds(interval);
    }

    private EnergyPickup CreatePickup()
    {
        EnergyPickup pickup = Instantiate(pickupPrefab, transform);
        pickup.SetDespawnCallback(() => pool.Release(pickup));
        return pickup;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return wait;
            if (pool.CountActive < maxActive) TrySpawn();
        }
    }

    private void TrySpawn()
    {
        if (PlayerController.Instance == null || ScreenBounds.Instance == null) return;

        Rect view = ScreenBounds.Instance.GetVisibleArea();
        Vector3 pos = new Vector3(
            Random.Range(view.xMin + edgePadding, view.xMax - edgePadding),
            pickupPrefab.transform.position.y,
            Random.Range(view.yMin + edgePadding, view.yMax - edgePadding)
        );

        Vector3 playerPos = PlayerController.Instance.transform.position;
        if (Vector3.Distance(pos, playerPos) < minDistanceFromPlayer) return;

        pool.Get().transform.position = pos;
    }
}
