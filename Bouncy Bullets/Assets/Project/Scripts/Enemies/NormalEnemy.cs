using UnityEngine;

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
        ScreenBounds bounds = ScreenBounds.Instance;
        if (bounds == null) return;

        if (transform.position.z < bounds.GetVisibleArea().yMin) OnExitScreen();
    }

    protected override void OnExitScreen()
    {
        if (spawnBulletEventChannel != null)
        {
            float randomAngle = Random.Range(-40, 40);
            Quaternion bounceRotation = Quaternion.Euler(0f, randomAngle, 0f);
            spawnBulletEventChannel.RaiseEvent(BulletType.Bouncy, transform.position, bounceRotation);
        }

        base.OnExitScreen();
    }
}
