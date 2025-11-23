using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsSubject : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    [Header("Score")]
    public int Score { get; private set; }

    [Header("Ammo")]
    public int Ammo { get; private set; }

    private readonly List<IPlayerStatsObserver> observers = new List<IPlayerStatsObserver>();

    private void Awake()
    {
        CurrentHealth = maxHealth;
        NotifyHealth();
        NotifyScore();
        NotifyAmmo();
    }

    public void RegisterObserver(IPlayerStatsObserver observer)
    {
        if (observer == null || observers.Contains(observer)) return;

        observers.Add(observer);
        observer.OnHealthChanged(CurrentHealth, maxHealth);
        observer.OnScoreChanged(Score);
        observer.OnAmmoChanged(Ammo);
    }

    public void UnregisterObserver(IPlayerStatsObserver observer)
    {
        if (observer == null) return;
        observers.Remove(observer);
    }

    private void NotifyHealth()
    {
        foreach (var obs in observers)
            obs.OnHealthChanged(CurrentHealth, maxHealth);
    }

    private void NotifyScore()
    {
        foreach (var obs in observers)
            obs.OnScoreChanged(Score);
    }

    private void NotifyAmmo()
    {
        foreach (var obs in observers)
            obs.OnAmmoChanged(Ammo);
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, maxHealth);
        NotifyHealth();
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
        NotifyHealth();
    }

    public void AddScore(int amount)
    {
        Score = Mathf.Max(Score + amount, 0);
        NotifyScore();
    }

    public void AddAmmo(int amount)
    {
        Ammo = Mathf.Max(Ammo + amount, 0);
        NotifyAmmo();
    }
}