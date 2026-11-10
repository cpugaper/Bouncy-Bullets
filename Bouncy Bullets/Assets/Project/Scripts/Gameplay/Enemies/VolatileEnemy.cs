using System.Collections;
using UnityEngine;

public class VolatileEnemy : Enemy
{
    [Header("Volatile Movement")]
    [SerializeField] private float speed = 4f;

    [Header("Explosion Mechanics")]
    [SerializeField] private float overloadDuration = 1.2f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private LayerMask destructionMask;
    [SerializeField, Range(0.7f, 1f)] private float damageRadiusMultiplier = 0.9f;

    private Vector3 direction;
    private bool hasCalculatedDirection;
    private bool isOverloading = false;

    private VolatileEffect visuals;
    private Coroutine overloadCoroutine;

    private void Awake()
    {
        visuals = GetComponent<VolatileEffect>();
    }

    private void OnEnable()
    {
        hasCalculatedDirection = false;
        isOverloading = false;
    }

    private void OnDisable()
    {
        if (overloadCoroutine != null) StopCoroutine(overloadCoroutine);
    }

    protected override void Move()
    {
        if (isOverloading) return;

        if (!hasCalculatedDirection)
        {
            if (PlayerMovement.Instance != null)
            {
                Vector3 playerPos = PlayerMovement.Instance.transform.position;
                direction = (playerPos - transform.position).normalized;
                hasCalculatedDirection = true;
            }
        }

        transform.position += direction * speed * Time.deltaTime;
    }

    protected override void CheckBounds()
    {
        if (isOverloading) return; 
        base.CheckBounds();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeInHierarchy || isOverloading) return; 

        if (other.CompareTag("BouncyBullet"))
        {
            if (addScoreEventChannel != null)
            {
                addScoreEventChannel.RaiseEvent(scoreValue);
            }

            overloadCoroutine = StartCoroutine(OverloadSequence());
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            overloadCoroutine = StartCoroutine(OverloadSequence());
        }
        else if (other.CompareTag("Player"))
        {
            base.OnTriggerEnter(other);
        }
    }

    private IEnumerator OverloadSequence()
    {
        isOverloading = true;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayVolatileCharge();

        float timer = 0f;
        while (timer < overloadDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / overloadDuration;

            if (visuals != null) visuals.AnimateOverload(timer, progress, explosionRadius);

            yield return null;
        }

        Detonate();
    }

    private void Detonate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius * damageRadiusMultiplier, destructionMask);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            if (hit.TryGetComponent(out Enemy enemyHit))
            {
                enemyHit.Die();
            }
            else if (hit.TryGetComponent(out BouncyBullet bullet))
            {
                bullet.ReturnToPool();
            }
            else if (hit.TryGetComponent(out PlayerHealth player))
            {
                if (PlayerShield.HasActiveShield(hit)) continue;
                player.Die();
            }
        }

        Die();
    }

    public override void Die()
    {
        if (!gameObject.activeInHierarchy) return;

        if (enemyDeathEventChannel != null)
        {
            enemyDeathEventChannel.RaiseEvent(transform.position);
        }

        ReturnToPool();
    }
}
