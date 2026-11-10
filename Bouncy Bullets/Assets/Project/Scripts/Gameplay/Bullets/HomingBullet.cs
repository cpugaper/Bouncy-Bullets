using UnityEngine;

namespace Game.Gameplay
{
    public class HomingBullet : Bullet
    {
        [Header("Homing Settings")]
        [SerializeField] private float turnSpeed = 130f;

        private void FixedUpdate()
        {
            HomingMovement();
            if (BounceInsideView()) rb.MoveRotation(Quaternion.LookRotation(rb.linearVelocity));
        }

        private void HomingMovement()
        {
            if (PlayerMovement.Instance != null)
            {
                Vector3 targetDir = (PlayerMovement.Instance.transform.position - transform.position).normalized;
                targetDir.y = 0f;

                if (targetDir != Vector3.zero)
                {
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
                    if (newDir != Vector3.zero) rb.MoveRotation(Quaternion.LookRotation(newDir));
                }
            }

            rb.linearVelocity = transform.forward * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ReturnToPool();
            }
            else if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent(out Enemy enemy))
                {
                    enemy.Die();
                    ReturnToPool();
                }
            }
        }
    }
}
