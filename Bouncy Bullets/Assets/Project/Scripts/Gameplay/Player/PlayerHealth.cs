using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private PositionEventChannelSO playerDeathEventChannel;

        private PlayerShield shield;
        private bool isDead;

        private void Start()
        {
            shield = GetComponent<PlayerShield>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (shield != null && shield.IsShieldActive) return;

            if (other.CompareTag("Enemy") || other.CompareTag("BouncyBullet") || other.CompareTag("EnemyBullet"))
            {
                Die();
            }
        }

        public void Die()
        {
            if (isDead) return;
            isDead = true;

            if (playerDeathEventChannel != null)
            {
                playerDeathEventChannel.RaiseEvent(transform.position);
            }

            Destroy(gameObject);
        }
    }
}
