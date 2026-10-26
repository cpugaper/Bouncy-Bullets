using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerShield shield;

    [Header("Event Channels")]
    [SerializeField] private PositionEventChannelSO playerDeathEventChannel;

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
        if (playerDeathEventChannel != null)
        {
            playerDeathEventChannel.RaiseEvent(transform.position);
        }

        Destroy(gameObject);
    }
}
