using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class CursorController : MonoBehaviour
    {
        private GameManager gameManager;
        private InputReader input;

        private void Start()
        {
            gameManager = GameManager.Instance;
            input = InputReader.Instance;
        }

        private void LateUpdate()
        {
            Apply();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) Apply();
        }

        private void Apply()
        {
            if (gameManager == null || input == null) return;

            bool playing = gameManager.CurrentState == GameState.Playing;
            bool showCursor = !playing && input.ActiveDevice == InputDeviceKind.Mouse;
            CursorLockMode lockMode = playing ? CursorLockMode.Confined : CursorLockMode.None;

            if (Cursor.visible != showCursor) Cursor.visible = showCursor;
            if (Cursor.lockState != lockMode) Cursor.lockState = lockMode;
        }
    }
}
