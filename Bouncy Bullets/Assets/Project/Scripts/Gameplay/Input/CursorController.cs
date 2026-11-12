using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class CursorController : MonoBehaviour
    {
        private GameManager gameManager;
        private InputReader input;
        private bool isWebGL;
        private bool lockWasActive;

        private void Start()
        {
            gameManager = GameManager.Instance;
            input = InputReader.Instance;
            isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
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

            if (isWebGL) PauseIfBrowserReleasedCursor();

            bool playing = gameManager.CurrentState == GameState.Playing;
            bool showCursor = !playing && input.ActiveDevice == InputDeviceKind.Mouse;

            CursorLockMode lockWhilePlaying = isWebGL ? CursorLockMode.Locked : CursorLockMode.Confined;
            CursorLockMode lockMode = playing ? lockWhilePlaying : CursorLockMode.None;


            if (Cursor.visible != showCursor) Cursor.visible = showCursor;
            if (Cursor.lockState != lockMode) Cursor.lockState = lockMode;
        }

        private void PauseIfBrowserReleasedCursor()
        {
            if (gameManager.CurrentState != GameState.Playing)
            {
                lockWasActive = false;
                return;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                lockWasActive = true;
            }
            else if (lockWasActive)
            {
                lockWasActive = false;
                gameManager.TogglePause();
            }
        }
    }
}
