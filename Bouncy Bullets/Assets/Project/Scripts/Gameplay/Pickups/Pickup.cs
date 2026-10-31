using System;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] private float lifetime = 12f;

    private Action onDespawn;
    private float timer;

    public void SetDespawnCallback(Action callback) => onDespawn = callback;

    private void OnEnable()
    {
        timer = lifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (TryApplyEffect(other.gameObject))
        {
            PlayPickupSound();
            Despawn();
        }
    }

    protected abstract bool TryApplyEffect(GameObject player);

    protected abstract void PlayPickupSound();

    private void Despawn()
    {
        onDespawn?.Invoke();
        if (onDespawn == null)
        {
            gameObject.SetActive(false);
        }
    }
}
