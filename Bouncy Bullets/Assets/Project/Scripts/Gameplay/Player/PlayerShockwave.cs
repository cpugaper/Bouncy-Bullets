using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class PlayerShockwave : MonoBehaviour
    {
        [Header("Shockwave Settings")]
        [SerializeField] private int maxCharges = 3;
        [SerializeField] private float explosionRadius = 8f;
        [SerializeField] private LayerMask destructionMask;
        [SerializeField] private float expansionDuration = 0.2f;
        [SerializeField, Range(0.7f, 1f)] private float damageRadiusMultiplier = 0.9f;


        [Header("Event Channels")]
        [SerializeField] private IntEventChannelSO shockwaveChangedEventChannel;
        [SerializeField] private SfxEventChannelSO sfxEventChannel;

        [Header("Visual Reference")]
        [SerializeField] private Transform shockwaveVisualQuad;

        private int currentCharges = 0;
        private bool isExploding = false;
        private InputReader input;

        private const int MaxHits = 64;
        private readonly Collider[] blastBuffer = new Collider[MaxHits];

        public bool HasCharges => currentCharges > 0;
        public bool IsFull => currentCharges >= maxCharges;

        private void Start()
        {
            input = InputReader.Instance;

            NotifyUI();
        }

        private void Update()
        {
            if (HasCharges && !isExploding && input.ShockwavePressed)
            {
                UseShockwave();
            }
        }

        public bool TryAddShockwave()
        {
            if (currentCharges >= maxCharges) return false;

            currentCharges++;
            NotifyUI();
            return true;
        }

        private void UseShockwave()
        {
            currentCharges--;
            NotifyUI();
            StartCoroutine(ShockwaveSequence());
        }

        private void NotifyUI()
        {
            if (shockwaveChangedEventChannel != null)
            {
                shockwaveChangedEventChannel.RaiseEvent(currentCharges);
            }
        }

        private IEnumerator ShockwaveSequence()
        {
            isExploding = true;

            if (sfxEventChannel != null) sfxEventChannel.RaiseEvent(SfxType.ShockwaveCharge);

            float timer = 0f;
            while (timer < expansionDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / expansionDuration;

                if (shockwaveVisualQuad != null)
                {
                    float quadSize = Mathf.Lerp(0f, explosionRadius * 2f, progress);
                    shockwaveVisualQuad.localScale = new Vector3(quadSize, quadSize, 1f);
                }

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

                if (hit.TryGetComponent(out VolatileEnemy volatileEnemy))
                {
                    volatileEnemy.TriggerOverload();
                }
                else if (hit.TryGetComponent(out IKillable target))
                {
                    target.Die();
                }
            }

            if (shockwaveVisualQuad != null) shockwaveVisualQuad.localScale = Vector3.zero;
            isExploding = false;
        }
    }
}
