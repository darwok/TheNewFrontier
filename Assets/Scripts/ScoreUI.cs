using UnityEngine;
using TMPro;

public class scoreUI : MonoBehaviour
{
    public static scoreUI instance;

    [SerializeField] TextMeshProUGUI scoreValue;
    [SerializeField] TextMeshProUGUI hScoreValue;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void UpdateScore(int score)
    {
        scoreValue.text = score.ToString();
    }

    public void UpdateHighScore(int highScore)
    {
        hScoreValue.text = highScore.ToString();
    }
}