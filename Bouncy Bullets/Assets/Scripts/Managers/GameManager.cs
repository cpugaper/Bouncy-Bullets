using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum GameState
{
    MainMenu, 
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsUsingGamepad { get; private set; } = false;

    [Header("Events Channels")]
    [SerializeField] private PositionEventChannelSO playerDeathEventChannel;
    [SerializeField] private GameEventChannelSO runStartedEventChannel;

    [Header("Settings")]
    [SerializeField] private float gameOverDelay = 0.8f;

    private bool mouseActive = true;

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

    private void Update()
    {
        DetectActiveInputDevice();
        ApplyCursorRules();
    }

    private void DetectActiveInputDevice()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                gamepad.rightStick.ReadValue().sqrMagnitude > 0.1f ||
                gamepad.dpad.ReadValue().sqrMagnitude > 0.0f)
            {
                IsUsingGamepad = true;
                mouseActive = false;
                return;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            if (mouse.delta.ReadValue().sqrMagnitude > 1.0f ||
                mouse.leftButton.wasPressedThisFrame ||
                mouse.rightButton.wasPressedThisFrame)
            {
                IsUsingGamepad = false;
                mouseActive = true;
                return;
            }
        }

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.anyKey.isPressed)
        {
            IsUsingGamepad = false;
            mouseActive = false;
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) ApplyCursorRules();
    }

    private void ApplyCursorRules()
    {
        bool playing = CurrentState == GameState.Playing;
        bool showCursor = !playing && mouseActive;
        CursorLockMode lockMode = playing ? CursorLockMode.Confined : CursorLockMode.None;

        if (Cursor.visible != showCursor) Cursor.visible = showCursor;
        if (Cursor.lockState != lockMode) Cursor.lockState = lockMode;
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch(newState)
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

        ApplyCursorRules();
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
        if (SaveManager.Instance != null)
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
