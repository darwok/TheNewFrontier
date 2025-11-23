using UnityEngine;

public class scoreManager : MonoBehaviour
{
    public static scoreManager instance;

    int currentScore;
    int highScore;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void Start()
    {
        scoreUI.instance.UpdateScore(0);
        highScore = PlayerPrefs.GetInt("High Score");
        scoreUI.instance.UpdateHighScore(highScore);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreUI.instance.UpdateScore(currentScore);
    }

    public void SetHighScore()
    {
        if (currentScore > highScore) PlayerPrefs.SetInt("High Score", currentScore);
    }
}