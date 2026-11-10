using System;
using UnityEngine;

namespace Game.Gameplay
{
    public abstract class Pickup : MonoBehaviour
    {
        [SerializeField] private float lifetime = 12f;

        private Action onDespawn;
        private float timer;

        public void SetDespawnCallback(Action callback)
        {
            onDespawn = callback;
        }

        private void OnEnable()
        {
            timer = lifetime;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeInHierarchy || !other.CompareTag("Player")) return;

            if (TryApplyEffect(other.gameObject))
            {
                PlayPickupSound();
                ReturnToPool();
            }
        }

        protected abstract bool TryApplyEffect(GameObject player);

        protected abstract void PlayPickupSound();

        private void ReturnToPool()
        {
            onDespawn?.Invoke();
            if (onDespawn == null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
