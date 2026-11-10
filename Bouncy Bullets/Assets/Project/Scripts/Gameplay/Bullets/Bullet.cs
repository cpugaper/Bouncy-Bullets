using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] protected float speed = 5.0f;
    [SerializeField] private float lifetime = 4.0f;
    [SerializeField] private float warningDuration = 1.2f;

    private Action onDespawn;
    private Coroutine lifetimeCoroutine;
    private BulletEffect bulletVisuals;
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        bulletVisuals = GetComponent<BulletEffect>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetDespawnCallback(Action callback)
    {
        onDespawn = callback;
    }

    public virtual void Launch(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (bulletVisuals != null)
        {
            bulletVisuals.ResetVisuals();
        }

        if (lifetimeCoroutine != null) StopCoroutine(lifetimeCoroutine);
        lifetimeCoroutine = StartCoroutine(LifetimeRoutine());
    }

    protected bool BounceInsideView()
    {
        if (rb == null) return false;

        Vector3 position = rb.position;
        Vector3 velocity = rb.linearVelocity;

        if (!ScreenBounds.Instance.KeepInsideView(ref position, ref velocity)) return false;

        rb.position = position;
        rb.linearVelocity = velocity;
        return true;
    }

    protected virtual IEnumerator LifetimeRoutine()
    {
        float normalFlightDuration = Mathf.Max(0f, lifetime - warningDuration);
        yield return new WaitForSeconds(normalFlightDuration);

        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            elapsed += Time.deltaTime;
            if (bulletVisuals != null)
            {
                bulletVisuals.AnimateBlink(elapsed);
            }
            yield return null;
        }

        ReturnToPool();
    }

    public virtual void ReturnToPool()
    {
        if (!gameObject.activeInHierarchy) return;

        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }

        if (bulletVisuals != null)
        {
            bulletVisuals.ResetVisuals();
        }

        onDespawn?.Invoke();
        if (onDespawn == null)
        {
            gameObject.SetActive(false);
        }
    }
}
