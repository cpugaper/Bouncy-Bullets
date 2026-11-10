using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    public class TurboUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField, Min(0.1f)] private float fillSpeed = 0.8f;

        [Header("Event Channels")]
        [SerializeField] private FloatEventChannelSO turboChangedEventChannel;

        private float targetFill;

        private void Awake()
        {
            fillImage.fillAmount = 0f;
        }

        private void OnEnable()
        {
            if (turboChangedEventChannel != null) turboChangedEventChannel.Subscribe(SetTarget);
        }

        private void OnDisable()
        {
            if (turboChangedEventChannel != null) turboChangedEventChannel.Unsubscribe(SetTarget);
        }

        private void SetTarget(float normalized) => targetFill = normalized;

        private void Update()
        {
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFill, fillSpeed * Time.deltaTime);
        }
    }
}
