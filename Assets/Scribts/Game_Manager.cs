using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int currentLives;
    [SerializeField] private float gameDuration = 300f; // 5 minutes
    [SerializeField] private float currentTime;
    [SerializeField] private int requiredCollectibles = 0;
    [SerializeField] private bool requireAllCollectibles = false;

    private bool isGameOver = false;
    private bool isGamePaused = false;
    private int collectedItems = 0;
    private int totalCollectibles = 0;
    private bool isTimerRunning = false;

    // Events
    public event Action OnGameOver;
    public event Action OnVictory;
    public event Action OnLifeLost;
    public event Action OnItemCollected;
    public event Action<float> OnTimerUpdated;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isTimerRunning && !isGameOver && !isGamePaused)
        {
            UpdateTimer();
        }
    }

    private void InitializeGame()
    {
        currentLives = maxLives;
        currentTime = gameDuration;
        isGameOver = false;
        isGamePaused = false;
        collectedItems = 0;
        totalCollectibles = 0;
        isTimerRunning = false;
    }

    public void StartGame()
    {
        InitializeGame();
        UIManager.Instance.ShowGameplayUI();
        LevelManager.Instance.LoadGameLevel();
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        currentTime = gameDuration;
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        OnTimerUpdated?.Invoke(currentTime);

        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver("Time's up!");
        }
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        OnLifeLost?.Invoke();

        if (currentLives <= 0)
        {
            GameOver("You ran out of lives!");
        }
    }

    public void AddPickup()
    {
        collectedItems++;
        OnItemCollected?.Invoke();
    }

    public void SetTotalCollectibles(int count)
    {
        totalCollectibles = count;
        if (requireAllCollectibles)
        {
            requiredCollectibles = totalCollectibles;
        }
    }

    public void GameOver(string reason = "Game Over!")
    {
        if (isGameOver) return;

        isGameOver = true;
        isTimerRunning = false;
        Debug.Log(reason);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnGameOver?.Invoke();
    }

    public void Victory()
    {
        if (isGameOver) return;

        // Check if required collectibles have been gathered
        if (requiredCollectibles > 0 && !HasCollectedRequiredItems())
        {
            Debug.Log($"You need to collect {requiredCollectibles} items to win!");
            return;
        }

        // Only win if player has at least one life
        if (currentLives > 0)
        {
            isGameOver = true;
            isTimerRunning = false;
            Debug.Log("Victory!");

            // Unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            OnVictory?.Invoke();
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    // Add this method to fix the error
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Getters
    public int GetCurrentLives() => currentLives;
    public int GetMaxLives() => maxLives;
    public int GetCollectedItems() => collectedItems;
    public float GetRemainingTime() => currentTime;
    public int GetTotalCollectibles() => totalCollectibles;
    public int GetRequiredCollectibles() => requiredCollectibles;
    public bool HasCollectedRequiredItems() => collectedItems >= requiredCollectibles;
    public bool IsGameOver() => isGameOver;
    public bool IsGamePaused() => isGamePaused;

    // Setters
    public void SetGameDuration(float duration)
    {
        gameDuration = duration;
        currentTime = duration;
    }
}
