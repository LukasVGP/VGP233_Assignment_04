using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI pickupsText;

    private void Start()
    {
        UpdateDisplay();
    }

    private void Update()
    {
        // Update every frame to ensure it's always current
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (GameManager.Instance == null) return;

        // Update lives display
        if (livesText != null)
        {
            livesText.text = "Lives: " + GameManager.Instance.GetCurrentLives();
        }

        // Update timer display
        if (timerText != null)
        {
            float timeRemaining = GameManager.Instance.GetRemainingTime();
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }

        // Update pickups display
        if (pickupsText != null)
        {
            pickupsText.text = "Items: " + GameManager.Instance.GetCollectedItems();
        }
    }
}
