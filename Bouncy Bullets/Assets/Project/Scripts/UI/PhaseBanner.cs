using System.Collections;
using UnityEngine;
using TMPro;
using Game.Core;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PhaseBanner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text label;
        [SerializeField] private IntEventChannelSO phaseChangedEventChannel;

        [Header("Timing")]
        [SerializeField] private float fadeTime = 0.4f;
        [SerializeField] private float holdTime = 1.7f;

        private CanvasGroup canvasGroup;
        private Coroutine showRoutine;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        private void OnEnable()
        {
            if (phaseChangedEventChannel != null) phaseChangedEventChannel.Subscribe(OnPhaseChanged);
        }

        private void OnDisable()
        {
            if (phaseChangedEventChannel != null) phaseChangedEventChannel.Unsubscribe(OnPhaseChanged);
        }

        private void OnPhaseChanged(int phaseNumber)
        {
            label.text = $"PHASE {phaseNumber}";

            if (showRoutine != null) StopCoroutine(showRoutine);
            showRoutine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            yield return Fade(0f, 1f);
            yield return new WaitForSeconds(holdTime);
            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = to;
        }
    }
}
