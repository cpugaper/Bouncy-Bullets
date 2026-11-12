using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Core;

namespace Game.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Events Channels")]
        [SerializeField] private PositionEventChannelSO playerDeathEventChannel;
        [SerializeField] private GameEventChannelSO runStartedEventChannel;

        [Header("Settings")]
        [SerializeField] private float gameOverDelay = 0.8f;

        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetState(GameState.MainMenu);
        }

        private void OnEnable()
        {
            if (playerDeathEventChannel != null) playerDeathEventChannel.Subscribe(OnPlayerDeath);
        }

        private void OnDisable()
        {
            if (playerDeathEventChannel != null) playerDeathEventChannel.Unsubscribe(OnPlayerDeath);
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 0f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
            OnGameStateChanged?.Invoke(newState);
        }

        public void ResumeGame() => SetState(GameState.Playing);

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
            else if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        private void OnPlayerDeath(Vector3 position)
        {
            StartCoroutine(GameOverRoutine());
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSecondsRealtime(gameOverDelay);
            SetState(GameState.GameOver);
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void NewGame() => BeginRun(false);
        public void ContinueGame() => BeginRun(true);

        private void BeginRun(bool continueSavedGame)
        {
            SaveManager.Instance.BeginRun(continueSavedGame);

            SetState(GameState.Playing);

            if (runStartedEventChannel != null)
                runStartedEventChannel.RaiseEvent();
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
