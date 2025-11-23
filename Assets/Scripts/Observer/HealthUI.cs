using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour, IPlayerStatsObserver
{
    [SerializeField] private PlayerStatsSubject subject;
    [SerializeField] private Slider hpSlider;

    private void OnEnable()
    {
        if (subject == null)
            subject = Object.FindFirstObjectByType<PlayerStatsSubject>();

        if (subject != null)
            subject.RegisterObserver(this);
    }

    private void OnDisable()
    {
        if (subject != null)
            subject.UnregisterObserver(this);
    }

    public void OnHealthChanged(float current, float max)
    {
        if (hpSlider == null) return;
        hpSlider.maxValue = max;
        hpSlider.value = current;
    }

    public void OnScoreChanged(int score) { }
    public void OnAmmoChanged(int ammo) { }
}