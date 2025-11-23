using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Factory")]
    public EnemyFactory factory;

    [Header("Spawn Config")]
    public EnemyType enemyType;
    public Transform[] spawnPoints;
    public int enemiesPerPoint = 1;

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (factory == null)
        {
            Debug.LogError("EnemySpawner: factory no asignado");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: sin spawnPoints");
            return;
        }

        foreach (var point in spawnPoints)
        {
            for (int i = 0; i < enemiesPerPoint; i++)
            {
                factory.CreateEnemy(enemyType, point.position, point.rotation);
            }
        }
    }
}