using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    [Header("Options Settings")]
    public Slider volumeSlider;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Show the main menu and hide the options panel when the game starts.
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Back button logic
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Well... it quits the game.
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
