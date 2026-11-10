using UnityEngine;

namespace Game.Gameplay
{
    public class BouncyBullet : Bullet
    {
        public override void Launch(Vector3 position, Quaternion rotation)
        {
            base.Launch(position, rotation);

            if (rb != null)
                rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }

        private void FixedUpdate()
        {
            BounceInsideView();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") || (other.CompareTag("Player") && PlayerShield.HasActiveShield(other)))
            {
                ReturnToPool();
            }
        }
    }
}
