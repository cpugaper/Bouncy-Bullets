using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class NormalEnemy : Enemy
    {
        [SerializeField] private float speed = 3f;

        [Header("References")]
        [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

        protected override void Move()
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

        protected override void CheckBounds()
        {
            if (transform.position.z < ScreenBounds.Instance.GetVisibleArea().yMin) OnExitScreen();
        }

        protected override void OnExitScreen()
        {
            if (spawnBulletEventChannel != null)
            {
                Vector3 shootDirection = Vector3.back;

                if (PlayerMovement.Instance != null)
                {
                    Vector3 playerPos = PlayerMovement.Instance.transform.position;
                    shootDirection = (playerPos - transform.position).normalized;
                    shootDirection.y = 0f;
                }

                Quaternion bounceRotation = Quaternion.LookRotation(shootDirection);
                spawnBulletEventChannel.RaiseEvent(BulletType.Bouncy, transform.position, bounceRotation);
            }

            base.OnExitScreen();
        }
    }
}
