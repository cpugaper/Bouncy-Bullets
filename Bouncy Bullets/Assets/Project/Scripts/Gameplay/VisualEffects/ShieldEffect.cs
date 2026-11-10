using System.Collections;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light playerLight;
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color shieldColor = Color.red;
    [SerializeField] private float transitionDuration = 0.2f;

    [Header("Warning Blink Settings")]
    [SerializeField] private float blinkSpeed = 12f;

    private Coroutine changeColorRoutine;

    public void ActivateVisualShield()
    {
        if (playerLight == null) return;
        if (changeColorRoutine != null) StopCoroutine(changeColorRoutine);
        changeColorRoutine = StartCoroutine(TransitionLightColor(playerLight.color, shieldColor));
    }

    public void StartWarningVisual(float duration)
    {
        if (playerLight == null) return;
        if (changeColorRoutine != null) StopCoroutine(changeColorRoutine);
        changeColorRoutine = StartCoroutine(BlinkWarningRoutine(duration));
    }

    public void DeactivateVisualShield()
    {
        if (playerLight == null) return;
        if (changeColorRoutine != null) StopCoroutine(changeColorRoutine);
        changeColorRoutine = StartCoroutine(TransitionLightColor(playerLight.color, normalColor));
    }

    private IEnumerator BlinkWarningRoutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpFactor = Mathf.PingPong(elapsed * blinkSpeed, 1f);
            playerLight.color = Color.Lerp(shieldColor, normalColor, lerpFactor);
            yield return null;
        }
    }

    private IEnumerator TransitionLightColor(Color startColor, Color endColor)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / transitionDuration;
            playerLight.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        playerLight.color = endColor;
        changeColorRoutine = null;
    }
}