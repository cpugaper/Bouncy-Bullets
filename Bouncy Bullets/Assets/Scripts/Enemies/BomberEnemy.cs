using System.Collections;
using UnityEngine;

public class BomberEnemy : Enemy
{
    [Header("Explosion Settings")]
    [SerializeField] private float timeToExplode = 5f;
    [SerializeField] private int bulletCount = 4;

    [Header("References")]
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

    private BomberEffect visuals;
    private Coroutine explodeCoroutine;

    protected override void Move() { }

    private void Awake()
    {
        visuals = GetComponent<BomberEffect>();
    }

    private void OnEnable()
    {
        if (explodeCoroutine != null) StopCoroutine(explodeCoroutine);
        explodeCoroutine = StartCoroutine(ExplodeSequence());
    }

    private void OnDisable()
    {
        if (explodeCoroutine != null) StopCoroutine(explodeCoroutine);
    }

    private IEnumerator ExplodeSequence()
    {
        float timer = 0f;

        while (timer < timeToExplode)
        {
            timer += Time.deltaTime;
            if (visuals != null) visuals.SetFlashProgress(timer / timeToExplode);
            yield return null;
        }

        SpawnBulletsInX();
        Die();
    }

    private void SpawnBulletsInX()
    {
        if (spawnBulletEventChannel == null) return;

        float randomOffset = Random.Range(0f, 90f);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = randomOffset + (i * 90f);
            Quaternion bulletRotation = Quaternion.Euler(0f, angle, 0f);
            spawnBulletEventChannel.RaiseEvent(BulletType.Bouncy, transform.position, bulletRotation);
        }
    }

    protected override void CheckBounds() { }
}
