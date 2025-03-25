using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Options Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        // Set up button listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(ShowOptions);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Make sure main panel is active and options panel is hidden
        if (mainPanel != null)
            mainPanel.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // Initialize options if they exist
        InitializeOptions();
    }

    private void InitializeOptions()
    {
        // Set up options panel functionality
        if (optionsPanel != null)
        {
            // Set up back button
            Button backButton = optionsPanel.GetComponentInChildren<Button>();
            if (backButton != null)
                backButton.onClick.AddListener(HideOptions);

            // Set up volume sliders
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

            // Set up fullscreen toggle
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

            // Set up resolution dropdown
            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                resolutionDropdown.AddOptions(new List<string>() { "1920x1080", "1600x900", "1280x720" });
                resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
        }
    }

    private void StartGame()
    {
        // Start the game
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

    private void ShowOptions()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    private void HideOptions()
    {
        if (mainPanel != null)
            mainPanel.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    private void QuitGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    // Options panel functions
    private void SetMusicVolume(float volume)
    {
        // Implement music volume control
        Debug.Log($"Music Volume: {volume}");
    }

    private void SetSFXVolume(float volume)
    {
        // Implement SFX volume control
        Debug.Log($"SFX Volume: {volume}");
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void SetResolution(int resolutionIndex)
    {
        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
        }
    }
}
