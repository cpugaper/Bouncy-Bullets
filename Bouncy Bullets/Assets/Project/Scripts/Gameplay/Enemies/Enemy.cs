using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] protected PositionEventChannelSO enemyDeathEventChannel;
    [SerializeField] protected IntEventChannelSO addScoreEventChannel;

    [Header("Score Settings")]
    [SerializeField] protected int scoreValue = 1;

    private Action onDespawn;

    public void SetDespawnCallback(Action callback)
    {
        onDespawn = callback;
    }

    protected virtual void Update()
    {
        Move();
        CheckBounds();
    }

    protected abstract void Move();

    protected virtual void CheckBounds()
    {
        Rect view = ScreenBounds.Instance.GetVisibleArea();
        Vector3 pos = transform.position;
        bool isOutOfBounds = pos.x < view.xMin || pos.x > view.xMax || pos.z < view.yMin || pos.z > view.yMax;

        if (isOutOfBounds)
        {
            OnExitScreen();
        }
    }

    protected virtual void OnExitScreen()
    {
        ReturnToPool();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BouncyBullet") || (other.CompareTag("Player") && PlayerShield.HasActiveShield(other)))
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (!gameObject.activeInHierarchy) return;

        if (enemyDeathEventChannel != null)
        {
            enemyDeathEventChannel.RaiseEvent(transform.position);
        }

        if (addScoreEventChannel != null)
        {
            addScoreEventChannel.RaiseEvent(scoreValue);
        }

        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        onDespawn?.Invoke();
        if (onDespawn == null)
        {
            gameObject.SetActive(false);
        }     
    }
}
