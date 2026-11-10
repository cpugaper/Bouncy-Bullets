using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayHUDPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject rulesModalPanel;

    [Header("Main Menu")]
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject mainMenuContent;

    [Header("Gamepad First Selected Buttons")]
    [SerializeField] private GameObject mainMenuFirstSelected;
    [SerializeField] private GameObject pauseFirstSelected;
    [SerializeField] private GameObject gameOverFirstSelected;
    [SerializeField] private GameObject rulesFirstSelected;

    [Header("Rules Pagination")]
    [SerializeField] private GameObject[] rulesPages;

    private int currentRulesPage = 0;
    private int lastNavDir;

    private InputSystem_Actions controls;

    private void Awake()
    {
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.UI.Pause.performed += OnPausePerformed;
        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Navigate.canceled += OnNavigateReleased;
        controls.UI.Pause.Enable();
        controls.UI.Navigate.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Pause.performed -= OnPausePerformed;
        controls.UI.Navigate.performed -= OnNavigate;
        controls.UI.Navigate.canceled -= OnNavigateReleased;
        controls.UI.Pause.Disable();
        controls.UI.Navigate.Disable();
    }

    private void Start()
    {
        if (continueButton != null) continueButton.interactable = SaveManager.Instance.HasSavedGame;

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        HandleGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (rulesModalPanel == null || !rulesModalPanel.activeSelf)
        {
            lastNavDir = 0;
            return;
        }

        float x = ctx.ReadValue<Vector2>().x;
        int dir = x > 0.5f ? 1 : x < -0.5f ? -1 : 0;

        if (dir == 0) { lastNavDir = 0; return; }   
        if (dir == lastNavDir) return;        

        lastNavDir = dir;
        if (dir > 0) NextRulesPage(); else PreviousRulesPage();
    }

    private void OnNavigateReleased(InputAction.CallbackContext ctx) => lastNavDir = 0;

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        if (rulesModalPanel != null && rulesModalPanel.activeSelf)
        {
            CloseRules();
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        controls.Dispose();
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(state == GameState.MainMenu);
        if (gameplayHUDPanel != null) gameplayHUDPanel.SetActive(state == GameState.Playing);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(state == GameState.Paused);
        if (gameOverPanel != null) gameOverPanel.SetActive(state == GameState.GameOver);

        if (state != GameState.MainMenu) CloseRules();

        if (state == GameState.MainMenu && mainMenuFirstSelected != null)
            EventSystem.current.SetSelectedGameObject(mainMenuFirstSelected);
        else if (state == GameState.Paused && pauseFirstSelected != null)
            EventSystem.current.SetSelectedGameObject(pauseFirstSelected);
        else if (state == GameState.GameOver && gameOverFirstSelected != null)
            EventSystem.current.SetSelectedGameObject(gameOverFirstSelected);
    }

    public void OnResumeButtonPressed() => GameManager.Instance.ResumeGame();
    public void OnRestartButtonPressed() => GameManager.Instance.RestartGame();
    public void OnQuitButtonPressed() => GameManager.Instance.QuitGame();
    public void OnNewGameButtonPressed() => GameManager.Instance.NewGame();
    public void OnContinueButtonPressed() => GameManager.Instance.ContinueGame();

    public void OpenRules()
    {
        if (rulesModalPanel == null) return;

        if (mainMenuContent != null) mainMenuContent.SetActive(false);  
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);  
        rulesModalPanel.SetActive(true);
        ShowRulesPage(0, true);
    }

    public void CloseRules()
    {
        if (rulesModalPanel != null) rulesModalPanel.SetActive(false);
        if (mainMenuContent != null) mainMenuContent.SetActive(true);

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(GameManager.Instance.CurrentState == GameState.Paused);

        if (GameManager.Instance.CurrentState == GameState.Paused)
        {
            if (pauseFirstSelected != null) EventSystem.current.SetSelectedGameObject(pauseFirstSelected);
        }
        else
        {
            if (mainMenuFirstSelected != null) EventSystem.current.SetSelectedGameObject(mainMenuFirstSelected);
        }
    }

    public void NextRulesPage()
    {
        if (rulesPages == null || rulesPages.Length == 0) return;
        currentRulesPage = (currentRulesPage + 1) % rulesPages.Length;
        ShowRulesPage(currentRulesPage, false);
    }

    public void PreviousRulesPage()
    {
        if (rulesPages == null || rulesPages.Length == 0) return;
        currentRulesPage--;
        if (currentRulesPage < 0) currentRulesPage = rulesPages.Length - 1;
        ShowRulesPage(currentRulesPage, false);
    }

    private void ShowRulesPage(int pageIndex, bool forceFirstSelected)
    {
        if (rulesPages == null || rulesPages.Length == 0) return;

        currentRulesPage = pageIndex;
        for (int i = 0; i < rulesPages.Length; i++)
        {
            if (rulesPages[i] != null)
            {
                rulesPages[i].SetActive(i == currentRulesPage);
            }
        }

        if (forceFirstSelected && rulesFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(rulesFirstSelected);
        }
    }
}
