using System;
using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float energyAmount = 25f;
    [SerializeField] private int scoreValue = 2;
    [SerializeField] private float lifetime = 12f;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO addScoreEventChannel;

    private Action onDespawn;
    private float timer;

    public void SetDespawnCallback(Action callback) => onDespawn = callback;

    private void OnEnable() => timer = lifetime;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent(out PlayerTurbo turbo) || !turbo.TryAddEnergy(energyAmount)) return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnergyPickup();
        }

        if (addScoreEventChannel != null) addScoreEventChannel.RaiseEvent(scoreValue);
        Despawn();
    }

    private void Despawn()
    {
        if (onDespawn != null) onDespawn();
        else gameObject.SetActive(false);
    }
}
