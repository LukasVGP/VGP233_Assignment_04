using UnityEngine;
using UnityEngine.UI;

public class SimpleUIManager : MonoBehaviour
{
    [Header("Screen References")]
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;

    [Header("Main Menu")]
    [SerializeField] private Button startGameButton;

    [Header("Game Over")]
    [SerializeField] private Button gameOverMenuButton;

    [Header("Victory")]
    [SerializeField] private Button victoryMenuButton;

    [Header("Background Images")]
    [SerializeField] private Image mainMenuBackground;
    [SerializeField] private Image gameOverBackground;
    [SerializeField] private Image victoryBackground;

    [Header("Background Sprites")]
    [SerializeField] private Sprite mainMenuSprite;
    [SerializeField] private Sprite gameOverSprite;
    [SerializeField] private Sprite victorySprite;

    private void Start()
    {
        // Set up button listeners
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);

        if (gameOverMenuButton != null)
            gameOverMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (victoryMenuButton != null)
            victoryMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Set background images
        if (mainMenuBackground != null && mainMenuSprite != null)
            mainMenuBackground.sprite = mainMenuSprite;

        if (gameOverBackground != null && gameOverSprite != null)
            gameOverBackground.sprite = gameOverSprite;

        if (victoryBackground != null && victorySprite != null)
            victoryBackground.sprite = victorySprite;

        // Show appropriate screen based on game state
        UpdateScreenVisibility();
    }

    public void ShowMainMenu()
    {
        if (mainMenuScreen) mainMenuScreen.SetActive(true);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (victoryScreen) victoryScreen.SetActive(false);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowGameOver()
    {
        if (mainMenuScreen) mainMenuScreen.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(true);
        if (victoryScreen) victoryScreen.SetActive(false);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowVictory()
    {
        if (mainMenuScreen) mainMenuScreen.SetActive(false);
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (victoryScreen) victoryScreen.SetActive(true);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateScreenVisibility()
    {
        // Check current scene or game state to determine which screen to show
        if (GameManager.Instance != null)
        {
            // This would need to be adapted based on how your GameManager tracks state
            // For now, we'll just default to showing the main menu
            ShowMainMenu();
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            // Fallback if GameManager doesn't exist
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }

    private void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            // Fallback if GameManager doesn't exist
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
