using UnityEngine;

public enum EnemyType
{
    Melee,
    Ranged
}

[System.Serializable]
public class EnemyFactory
{
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;

    public GameObject CreateEnemy(EnemyType type, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject prefab = null;
        switch (type)
        {
            case EnemyType.Melee:
                prefab = meleeEnemyPrefab;
                break;
            case EnemyType.Ranged:
                prefab = rangedEnemyPrefab;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("EnemyFactory: prefab no asignado para " + type);
            return null;
        }

        return Object.Instantiate(prefab, position, rotation, parent);
    }
}