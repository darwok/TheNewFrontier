using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverUIManager : MonoBehaviour
{
    public static gameOverUIManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        gameObject.SetActive(false);
    }

    public void ShowgameOverUI()
    {
        gameObject.SetActive(true);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }
    public void ShowWinUI()
    {

    }
}
