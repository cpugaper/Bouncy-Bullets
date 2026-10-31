using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyType enemyType;
    public EnemyType Type => enemyType;

    [Header("Event Channels")]
    [SerializeField] protected PositionEventChannelSO enemyDeathEventChannel;

    [Header("Score Settings")]
    [SerializeField] protected int scoreValue = 1;
    [SerializeField] protected IntEventChannelSO addScoreEventChannel;

    private Action OnDespawn;

    public void SetDespawnCallback(Action callback)
    {
        OnDespawn = callback;
    }

    protected virtual void Update()
    {
        Move();
        CheckBounds();
    }

    protected abstract void Move();

    protected virtual void CheckBounds()
    {
        ScreenBounds bounds = ScreenBounds.Instance;
        if (bounds == null) return;

        Rect view = bounds.GetVisibleArea();
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
        if (OnDespawn != null)
        {
            OnDespawn?.Invoke();
        }
        else
        {
            gameObject.SetActive(false);
        }     
    }
}
