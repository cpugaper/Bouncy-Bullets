using System.Collections;
using UnityEngine;

public class BomberEnemy : Enemy
{
    [Header("Phase Settings")]
    [SerializeField] private float phaseTimeReduction = 0.5f;

    [Header("Explosion Settings")]
    [SerializeField] private float baseTimeToExplode = 5f;
    [SerializeField] private float minTimeToExplode = 2f;
    [SerializeField] private int bulletCount = 2;

    [Header("References")]
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;
    [SerializeField] private IntEventChannelSO phaseChangedEventChannel;

    private BomberEffect visuals;
    private Coroutine explodeCoroutine;

    private int currentPhase = 1; 

    protected override void Move() { }

    private void Awake()
    {
        visuals = GetComponent<BomberEffect>();
    }

    private void OnEnable()
    {
        if (explodeCoroutine != null) StopCoroutine(explodeCoroutine);
        if (phaseChangedEventChannel != null) phaseChangedEventChannel.Subscribe(UpdatePhase);

        currentPhase = PhaseManager.CurrentPhaseNumber > 0 ? PhaseManager.CurrentPhaseNumber : 1;
        explodeCoroutine = StartCoroutine(ExplodeSequence());
    }

    private void OnDisable()
    {
        if (explodeCoroutine != null) StopCoroutine(explodeCoroutine);
        if (phaseChangedEventChannel != null) phaseChangedEventChannel.Unsubscribe(UpdatePhase);
    }

    private void UpdatePhase(int newPhase)
    {
        currentPhase = newPhase;
    }

    private IEnumerator ExplodeSequence()
    {
        float dynamicTimeToExplode = Mathf.Max(minTimeToExplode, baseTimeToExplode - ((currentPhase - 1) * phaseTimeReduction));
        float timer = 0f;

        while (timer < dynamicTimeToExplode)
        {
            timer += Time.deltaTime;
            if (visuals != null) visuals.SetFlashProgress(timer / dynamicTimeToExplode);
            yield return null;
        }

        SpawnBulletsInX();
        Die();
    }

    private void SpawnBulletsInX()
    {
        if (spawnBulletEventChannel == null) return;

        float step = 360f / bulletCount;
        float randomOffset = Random.Range(0f, step);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = randomOffset + (i * step);
            Quaternion bulletRotation = Quaternion.Euler(0f, angle, 0f);
            spawnBulletEventChannel.RaiseEvent(BulletType.Bouncy, transform.position, bulletRotation);
        }
    }

    protected override void CheckBounds() { }
}
