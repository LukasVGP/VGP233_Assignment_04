using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int currentLives;
    [SerializeField] private float gameDuration = 300f; // 5 minutes in seconds
    [SerializeField] private float currentTime;
    [SerializeField] private bool isTimerRunning = false;

    [Header("UI References")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI pickupsText;

    [Header("Simple UI")]
    [SerializeField] private SimpleUIManager uiManager;

    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName = "Game";

    [Header("Player References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Level Settings")]
    [SerializeField] private int requiredCollectibles = 0;
    [SerializeField] private bool requireAllCollectibles = false;

    [Header("Enemy Settings")]
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Collectible Settings")]
    [SerializeField] private Transform[] collectibleSpawnPoints;
    [SerializeField] private GameObject[] collectiblePrefabs;

    private bool isGameOver = false;
    private int collectedItems = 0;
    private int totalCollectibles = 0;

    private void Awake()
    {
        // Singleton pattern setup
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

    private void Start()
    {
        // Hide UI elements at start
        SetupUI();

        // Register for scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unregister from scene loaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            // Initialize level when game scene is loaded
            InitializeLevel();
        }
    }

    private void Update()
    {
        if (isTimerRunning && !isGameOver)
        {
            UpdateTimer();
        }
    }

    private void SetupUI()
    {
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        if (gameplayUI) gameplayUI.SetActive(SceneManager.GetActiveScene().name == gameSceneName);
        UpdateLivesUI();
        UpdatePickupsUI();
    }

    private void InitializeGame()
    {
        currentLives = maxLives;
        currentTime = gameDuration;
        isGameOver = false;
        collectedItems = 0;
        totalCollectibles = 0;
        isTimerRunning = false;
    }

    private void InitializeLevel()
    {
        // Spawn player
        SpawnPlayer();

        // Spawn enemies
        SpawnEnemies();

        // Spawn collectibles
        SpawnCollectibles();

        // Update UI
        SetupUI();

        // Start the timer
        StartTimer();
    }

    private void SpawnPlayer()
    {
        // Find player spawn point if not set
        if (playerSpawnPoint == null)
        {
            GameObject spawnPointObj = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawnPointObj != null)
            {
                playerSpawnPoint = spawnPointObj.transform;
            }
        }

        // Only spawn player if we have a spawn point and prefab
        if (playerSpawnPoint != null && playerPrefab != null)
        {
            // Check if player already exists
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (existingPlayer == null)
            {
                Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            }
            else
            {
                // Move existing player to spawn point
                existingPlayer.transform.position = playerSpawnPoint.position;
                existingPlayer.transform.rotation = playerSpawnPoint.rotation;
            }
        }
    }

    private void SpawnEnemies()
    {
        // Only spawn enemies if we have spawn points and prefabs
        if (enemySpawnPoints != null && enemySpawnPoints.Length > 0 &&
            enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            // Clear existing enemies if needed
            GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in existingEnemies)
            {
                Destroy(enemy);
            }

            // Spawn new enemies
            for (int i = 0; i < enemySpawnPoints.Length; i++)
            {
                if (enemySpawnPoints[i] != null)
                {
                    // Select a random enemy prefab
                    GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                    // Instantiate the enemy
                    GameObject enemy = Instantiate(enemyPrefab, enemySpawnPoints[i].position, enemySpawnPoints[i].rotation);

                    // Set enemy tag if it doesn't have one
                    if (!enemy.CompareTag("Enemy"))
                    {
                        enemy.tag = "Enemy";
                    }
                }
            }
        }
    }

    private void SpawnCollectibles()
    {
        // Only spawn collectibles if we have spawn points and prefabs
        if (collectibleSpawnPoints != null && collectibleSpawnPoints.Length > 0 &&
            collectiblePrefabs != null && collectiblePrefabs.Length > 0)
        {
            // Clear existing collectibles if needed
            GameObject[] existingCollectibles = GameObject.FindGameObjectsWithTag("Collectible");
            foreach (GameObject collectible in existingCollectibles)
            {
                Destroy(collectible);
            }

            // Reset total collectibles count
            totalCollectibles = 0;

            // Spawn new collectibles
            for (int i = 0; i < collectibleSpawnPoints.Length; i++)
            {
                if (collectibleSpawnPoints[i] != null)
                {
                    // Select a random collectible prefab
                    GameObject collectiblePrefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

                    // Instantiate the collectible
                    GameObject collectible = Instantiate(collectiblePrefab, collectibleSpawnPoints[i].position, collectibleSpawnPoints[i].rotation);

                    // Set collectible tag if it doesn't have one
                    if (!collectible.CompareTag("Collectible"))
                    {
                        collectible.tag = "Collectible";
                    }

                    totalCollectibles++;
                }
            }

            // Update required collectibles if we're requiring all of them
            if (requireAllCollectibles)
            {
                requiredCollectibles = totalCollectibles;
            }
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        currentTime = gameDuration;
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            TimerExpired();
        }
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    private void TimerExpired()
    {
        if (!isGameOver)
        {
            GameOver("Time's up!");
        }
    }

    public void LoseLife()
    {
        if (isGameOver) return;
        currentLives--;
        UpdateLivesUI();
        // Check if player is out of lives
        if (currentLives <= 0)
        {
            GameOver("You ran out of lives!");
        }
    }

    public void AddPickup()
    {
        collectedItems++;
        UpdatePickupsUI();

        // Check if player has collected enough items to enable victory
        if (HasCollectedRequiredItems() && requiredCollectibles > 0)
        {
            Debug.Log("Collected all required items!");
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }
    }

    private void UpdatePickupsUI()
    {
        if (pickupsText != null)
        {
            if (requiredCollectibles > 0)
            {
                pickupsText.text = $"Items: {collectedItems}/{requiredCollectibles}";
            }
            else
            {
                pickupsText.text = "Items: " + collectedItems;
            }
        }
    }

    public void GameOver(string reason = "Game Over!")
    {
        if (isGameOver) return;
        isGameOver = true;
        isTimerRunning = false;
        Debug.Log(reason);

        // Show game over UI (legacy method)
        if (gameOverUI)
        {
            gameOverUI.SetActive(true);
            // Set reason text if available
            TextMeshProUGUI reasonText = gameOverUI.GetComponentInChildren<TextMeshProUGUI>();
            if (reasonText != null)
            {
                reasonText.text = reason;
            }
        }

        // Show game over screen using SimpleUIManager
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

            // Show victory UI (legacy method)
            if (victoryUI) victoryUI.SetActive(true);

            // Show victory screen using SimpleUIManager
            if (uiManager != null)
            {
                uiManager.ShowVictory();
            }

            // Unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        InitializeGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void StartGame()
    {
        InitializeGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Getter for current lives (for UI or other scripts)
    public int GetCurrentLives()
    {
        return currentLives;
    }

    // Getter for max lives
    public int GetMaxLives()
    {
        return maxLives;
    }

    // Getter for collected items
    public int GetCollectedItems()
    {
        return collectedItems;
    }

    // Getter for remaining time
    public float GetRemainingTime()
    {
        return currentTime;
    }

    // Getter for total collectibles in level
    public int GetTotalCollectibles()
    {
        return totalCollectibles;
    }

    // Getter for required collectibles
    public int GetRequiredCollectibles()
    {
        return requiredCollectibles;
    }

    // Check if player has collected required items
    public bool HasCollectedRequiredItems()
    {
        return collectedItems >= requiredCollectibles;
    }

    // Set game duration (can be called from other scripts)
    public void SetGameDuration(float duration)
    {
        gameDuration = duration;
        currentTime = duration;
    }

    // Called when level is loaded
    public void OnLevelLoaded()
    {
        SetupUI();
        StartTimer();
    }
}
