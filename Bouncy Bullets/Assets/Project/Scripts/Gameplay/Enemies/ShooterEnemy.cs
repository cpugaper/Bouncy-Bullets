using System;
using System.Collections;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    [Header("Shooter Settings")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float enterDistance = 3f;
    [SerializeField] private float aimDuration = 0.8f;
    [SerializeField] private float waitAfterShot = 2f;
    [SerializeField] private float fireOffset = 1f;

    [Header("References")]
    [SerializeField] private SpawnBulletEventChannelSO spawnBulletEventChannel;

    private Vector3 retreatDirection;
    private bool isRetreating = false;
    private Coroutine sequenceCoroutine;
    private bool hasCalculatedDirection;

    private void OnEnable()
    {
        hasCalculatedDirection = false;
        isRetreating = false;
    }

    private void OnDisable()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }
    }

    private IEnumerator ShooterSequenceRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 dirToCenter = new Vector3(-startPos.x, 0f, -startPos.z).normalized;
        dirToCenter.y = 0f;

        Vector3 targetPos = startPos + dirToCenter * enterDistance;
        retreatDirection = -dirToCenter;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(dirToCenter);
            yield return null;
        }

        float aimTimer = 0f;
        while (aimTimer < aimDuration)
        {
            aimTimer += Time.deltaTime;

            if (PlayerMovement.Instance != null)
            {
                Vector3 playerDir = (PlayerMovement.Instance.transform.position - transform.position).normalized;
                playerDir.y = 0f;
                if (playerDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDir), 10f * Time.deltaTime);
                }
            }
            yield return null; 
        }

        if (spawnBulletEventChannel != null)
        {
            Vector3 spawnPos = transform.position + transform.forward * fireOffset;
            spawnBulletEventChannel.RaiseEvent(BulletType.Homing, spawnPos, transform.rotation);

            if (AudioManager.Instance != null) AudioManager.Instance.PlayShooterShoot();
        }

        yield return new WaitForSeconds(waitAfterShot);
        isRetreating = true;
    }

    protected override void Move()
    {
        if (!hasCalculatedDirection)
        {
            hasCalculatedDirection = true;
            sequenceCoroutine = StartCoroutine(ShooterSequenceRoutine());
            return;
        }

        if (isRetreating)
        {
            transform.position += retreatDirection * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(retreatDirection);
        }
    }

    protected override void CheckBounds()
    {
        if (isRetreating)
        {
            base.CheckBounds();
        }
    }

}
