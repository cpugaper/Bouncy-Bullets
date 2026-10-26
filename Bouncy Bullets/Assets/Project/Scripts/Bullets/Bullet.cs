using System;
using System.Collections;
using UnityEngine;

public enum BulletType
{
    Bouncy,
    Homing
}

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] protected float speed = 5.0f;
    [SerializeField] protected float lifetime = 4.0f;
    [SerializeField] protected float warningDuration = 1.2f;

    protected Action OnDespawn;
    protected Coroutine lifetimeCoroutine;
    protected BulletEffect bulletVisuals;
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        bulletVisuals = GetComponent<BulletEffect>();
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Action despawnCallback)
    {
        OnDespawn = despawnCallback;
    }

    public virtual void Launch(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

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
        ScreenBounds bounds = ScreenBounds.Instance;
        if (bounds == null || rb == null) return false;

        Vector3 position = rb.position;
        Vector3 velocity = rb.linearVelocity;

        if (!bounds.KeepInsideView(ref position, ref velocity)) return false;

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
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }

        if (bulletVisuals != null)
        {
            bulletVisuals.ResetVisuals();
        }

        if (OnDespawn != null)
        {
            OnDespawn();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
