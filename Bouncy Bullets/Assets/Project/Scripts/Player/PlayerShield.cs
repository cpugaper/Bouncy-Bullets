using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public bool IsShieldActive { get; private set; } = false;

    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 10f;
    [SerializeField] private float shieldCooldown = 15f;
    [SerializeField] private float warningDuration = 2f;

    [Header("Event Channels")]
    [SerializeField] private GameEventChannelSO shieldActivatedEventChannel;
    [SerializeField] private GameEventChannelSO shieldDeactivatedEventChannel;
    [SerializeField] private BoolEventChannelSO shieldReadyEventChannel;
    [SerializeField] private FloatEventChannelSO shieldCooldownStartedEventChannel;

    private ShieldEffect shieldVisual;
    private float nextShieldTime = 0f;

    private void Awake()
    {
        shieldVisual = GetComponent<ShieldEffect>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.ShieldPressed && !IsShieldActive && Time.time >= nextShieldTime)
        {
            StartCoroutine(ActivateShield());
        }
    }

    IEnumerator ActivateShield()
    {
        IsShieldActive = true;

        if(shieldReadyEventChannel != null) shieldReadyEventChannel.RaiseEvent(false);
        if (shieldVisual != null) shieldVisual.ActivateVisualShield();
        if (shieldActivatedEventChannel != null) shieldActivatedEventChannel.RaiseEvent();
        
        float mainDuration = Mathf.Max(0f, shieldDuration - warningDuration);
        yield return new WaitForSeconds(mainDuration);

        if (shieldVisual != null) shieldVisual.StartWarningVisual(warningDuration);
        yield return new WaitForSeconds(warningDuration);

        if (shieldVisual != null) shieldVisual.DeactivateVisualShield();
        IsShieldActive = false;
        nextShieldTime = Time.time + shieldCooldown;

        if (shieldDeactivatedEventChannel != null) shieldDeactivatedEventChannel.RaiseEvent();
        if (shieldCooldownStartedEventChannel != null) shieldCooldownStartedEventChannel.RaiseEvent(shieldCooldown);

        yield return new WaitForSeconds(shieldCooldown);

        if (shieldReadyEventChannel != null) shieldReadyEventChannel.RaiseEvent(true);
    }

    public static bool HasActiveShield(Collider player)
    {
        return player.TryGetComponent(out PlayerShield shield) && shield.IsShieldActive;
    }
}
