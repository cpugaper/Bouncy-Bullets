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

        [Header("Event Channels")]
        [SerializeField] private IntEventChannelSO shockwaveChangedEventChannel;

        [Header("Visual Reference")]
        [SerializeField] private Transform shockwaveVisualQuad;

        private int currentCharges = 0;
        private bool isExploding = false;
        private InputReader input;

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

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayShockwaveCharge();
            }

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
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, destructionMask);

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
                else if (hit.TryGetComponent(out HomingBullet enemyBullet))
                {
                    enemyBullet.ReturnToPool();
                }
            }

            if (shockwaveVisualQuad != null) shockwaveVisualQuad.localScale = Vector3.zero;
            isExploding = false;
        }
    }
}
