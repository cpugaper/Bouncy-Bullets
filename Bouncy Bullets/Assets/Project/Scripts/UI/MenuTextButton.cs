using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Gameplay;

namespace Game.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuTextButton : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject arrow;
        [SerializeField] private Material glowMaterial;

        private Button button;
        private Material normalMaterial;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (text != null) normalMaterial = text.fontSharedMaterial;
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            if (button != null) button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnEnable() =>
                SetHighlighted(EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject);

        public void OnSelect(BaseEventData e)
        {
            SetHighlighted(true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayUIHover();
        }

        public void OnDeselect(BaseEventData e) => SetHighlighted(false);

        public void OnPointerEnter(PointerEventData e) => TakeSelection();
        public void OnPointerMove(PointerEventData e) => TakeSelection();

        private void OnButtonClicked()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
        }

        private void TakeSelection()
        {
            EventSystem es = EventSystem.current;
            if (es == null || !button.interactable || es.currentSelectedGameObject == gameObject) return;
            es.SetSelectedGameObject(gameObject);
        }

        private void SetHighlighted(bool on)
        {
            on &= button.interactable;

            if (arrow != null) arrow.SetActive(on);
            if (text != null && glowMaterial != null && normalMaterial != null)
                text.fontSharedMaterial = on ? glowMaterial : normalMaterial;
        }
    }
}