using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    public class ShieldUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image fillImage;

        [Header("Event Channels")]
        [SerializeField] private BoolEventChannelSO shieldReadyEventChannel;
        [SerializeField] private FloatEventChannelSO shieldCooldownStartedEventChannel;

        private bool isCooling;
        private float cooldownEndTime;
        private float cooldownDuration;

        private void Awake()
        {
            fillImage.fillAmount = 1f;
        }

        private void OnEnable()
        {
            if (shieldReadyEventChannel != null) shieldReadyEventChannel.Subscribe(HandleShieldReady);
            if (shieldCooldownStartedEventChannel != null) shieldCooldownStartedEventChannel.Subscribe(HandleCooldownStarted);
        }

        private void OnDisable()
        {
            if (shieldReadyEventChannel != null) shieldReadyEventChannel.Unsubscribe(HandleShieldReady);
            if (shieldCooldownStartedEventChannel != null) shieldCooldownStartedEventChannel.Unsubscribe(HandleCooldownStarted);
        }

        private void HandleShieldReady(bool isReady)
        {
            isCooling = false;
            fillImage.fillAmount = isReady ? 1f : 0f;
        }

        private void HandleCooldownStarted(float duration)
        {
            cooldownDuration = duration;
            cooldownEndTime = Time.time + duration;
            isCooling = true;
        }

        private void Update()
        {
            if (!isCooling) return;

            float remaining = cooldownEndTime - Time.time;
            fillImage.fillAmount = 1f - Mathf.Clamp01(remaining / cooldownDuration);

            if (remaining <= 0f) isCooling = false;
        }
    }
}
