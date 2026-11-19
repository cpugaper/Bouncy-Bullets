using Game.Core;
using Game.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class UISlider : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
    {
        [Header("Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.cyan;

        [Header("Event Channels")]
        [SerializeField] protected SfxEventChannelSO sfxEventChannel;

        private Slider slider;
        private Image fillImage;

        private void Awake()
        {
            slider = GetComponent<Slider>();

            Transform fillArea = transform.Find("Fill Area");
            if (fillArea != null)
            {
                Transform fill = fillArea.Find("Fill");
                if (fill != null)
                {
                    fillImage = fill.GetComponent<Image>();
                }
            }

            UpdateFillColor(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current != null && slider.interactable)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            UpdateFillColor(true);
            if (sfxEventChannel != null) sfxEventChannel.RaiseEvent(SfxType.UIHover);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            UpdateFillColor(false);
        }

        private void UpdateFillColor(bool isSelected)
        {
            if (fillImage != null)
            {
                fillImage.color = isSelected ? selectedColor : normalColor;
            }
        }
    }
}
