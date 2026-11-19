using Game.Core;
using System.Collections;
using UnityEngine;

namespace Game.Gameplay
{
    public class VolatileEnemy : Enemy
    {
        [Header("Volatile Movement")]
        [SerializeField] private float speed = 4f;

        [Header("Explosion Mechanics")]
        [SerializeField] private float overloadDuration = 1.2f;
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private LayerMask destructionMask;
        [SerializeField, Range(0.7f, 1f)] private float damageRadiusMultiplier = 0.9f;

        [Header("Event Channels")]
        [SerializeField] private SfxEventChannelSO sfxEventChannel;

        [Header("References")]
        [SerializeField] private TransformReferenceSO playerReference;

        private Vector3 direction;
        private bool hasCalculatedDirection;
        private bool isOverloading = false;

        private VolatileEffect visuals;
        private Coroutine overloadCoroutine;

        private const int MaxHits = 64;
        private readonly Collider[] blastBuffer = new Collider[MaxHits];

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
                Transform player = playerReference.Value;
                if (player != null)
                {
                    Vector3 playerPos = player.position;
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
                TriggerOverload();
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

            if (sfxEventChannel != null) sfxEventChannel.RaiseEvent(SfxType.VolatileCharge);

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
            int count = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius * damageRadiusMultiplier, blastBuffer, destructionMask);

            for (int i = 0; i < count; i++)
            {
                Collider hit = blastBuffer[i];
                if (hit.gameObject == gameObject) continue;
                if (hit.TryGetComponent(out IKillable target)) target.Die();
            }

            Die();
        }

        public void TriggerOverload()
        {
            if (isOverloading) return;

            if (addScoreEventChannel != null)
            {
                addScoreEventChannel.RaiseEvent(scoreValue);
            }

            overloadCoroutine = StartCoroutine(OverloadSequence());
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
}
