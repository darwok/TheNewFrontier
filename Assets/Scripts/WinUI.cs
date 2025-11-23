using UnityEngine;
using UnityEngine.SceneManagement;

public class winUI : MonoBehaviour
{
    public static winUI instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        gameObject.SetActive(false);
    }
    public void ShowplayAgainUI()
    {
        gameObject.SetActive(true);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }
}