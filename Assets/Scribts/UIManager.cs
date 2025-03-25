using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private GameObject pauseMenuUI;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI pickupsText;
    [SerializeField] private TextMeshProUGUI gameOverReasonText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set up button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        // Subscribe to game events
        GameManager.Instance.OnGameOver += ShowGameOver;
        GameManager.Instance.OnVictory += ShowVictory;
        GameManager.Instance.OnLifeLost += UpdateLivesUI;
        GameManager.Instance.OnItemCollected += UpdatePickupsUI;
        GameManager.Instance.OnTimerUpdated += UpdateTimerUI;

        // Hide all UI panels initially
        HideAllPanels();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowGameOver;
            GameManager.Instance.OnVictory -= ShowVictory;
            GameManager.Instance.OnLifeLost -= UpdateLivesUI;
            GameManager.Instance.OnItemCollected -= UpdatePickupsUI;
            GameManager.Instance.OnTimerUpdated -= UpdateTimerUI;
        }
    }

    private void HideAllPanels()
    {
        if (gameplayUI) gameplayUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        HideAllPanels();
        if (gameplayUI) gameplayUI.SetActive(true);
        UpdateLivesUI();
        UpdatePickupsUI();
    }

    public void ShowGameOver()
    {
        HideAllPanels();
        if (gameOverUI) gameOverUI.SetActive(true);
    }

    public void ShowVictory()
    {
        HideAllPanels();
        if (victoryUI) victoryUI.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void ResumeGame()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + GameManager.Instance.GetCurrentLives();
        }
    }

    private void UpdatePickupsUI()
    {
        if (pickupsText != null)
        {
            int required = GameManager.Instance.GetRequiredCollectibles();
            int collected = GameManager.Instance.GetCollectedItems();

            if (required > 0)
            {
                pickupsText.text = $"Items: {collected}/{required}";
            }
            else
            {
                pickupsText.text = "Items: " + collected;
            }
        }
    }

    private void UpdateTimerUI(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    private void RestartGame()
    {
        LevelManager.Instance.RestartCurrentLevel();
    }

    private void ReturnToMainMenu()
    {
        LevelManager.Instance.LoadMainMenu();
    }

    private void QuitGame()
    {
        LevelManager.Instance.QuitGame();
    }
}
