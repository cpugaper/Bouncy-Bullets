using Game.Core;
using System;
using UnityEngine;

namespace Game.Gameplay
{
    public abstract class Pickup : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] protected SfxEventChannelSO sfxEventChannel;

        [SerializeField] private float lifetime = 12f;

        private Action onDespawn;
        private float timer;

        protected abstract SfxType PickupSfx { get; }

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
                if (sfxEventChannel != null) sfxEventChannel.RaiseEvent(PickupSfx);
                ReturnToPool();
            }
        }

        protected abstract bool TryApplyEffect(GameObject player);

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
