using UnityEngine;
using TMPro;

public class scoreUI : MonoBehaviour, IPlayerStatsObserver
{
    public static scoreUI instance;

    [SerializeField] private PlayerStatsSubject subject;
    [SerializeField] private TextMeshProUGUI scoreValue;
    [SerializeField] private TextMeshProUGUI hScoreValue;

    private int highScore;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (subject == null)
            subject = Object.FindFirstObjectByType<PlayerStatsSubject>();

        if (subject != null)
            subject.RegisterObserver(this);

        highScore = PlayerPrefs.GetInt("High Score", 0);
        if (hScoreValue != null)
            hScoreValue.text = highScore.ToString();
    }

    private void OnDisable()
    {
        if (subject != null)
            subject.UnregisterObserver(this);
    }

    public void OnHealthChanged(float current, float max) { }

    public void OnScoreChanged(int score)
    {
        if (scoreValue != null)
            scoreValue.text = score.ToString();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("High Score", highScore);
            if (hScoreValue != null)
                hScoreValue.text = highScore.ToString();
        }
    }

    public void OnAmmoChanged(int ammo) { }

    public void UpdateScore(int score)
    {
        if (scoreValue != null)
            scoreValue.text = score.ToString();
    }

    public void UpdateHighScore(int highScore)
    {
        if (hScoreValue != null)
            hScoreValue.text = highScore.ToString();
    }
}