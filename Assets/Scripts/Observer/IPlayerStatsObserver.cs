public interface IPlayerStatsObserver
{
    void OnHealthChanged(float current, float max);
    void OnScoreChanged(int score);
    void OnAmmoChanged(int ammo);
}