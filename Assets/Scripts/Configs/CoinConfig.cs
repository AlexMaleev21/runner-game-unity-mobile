using UnityEngine;

[CreateAssetMenu(fileName = "CoinSpawnConfig", menuName = "Game/CoinSpawnConfig")]
public class CoinSpawnConfig : ScriptableObject
{
    public float spawnInterval = 2f;
    public float minSpawnInterval = 0.8f;
    public int maxCoinsPerSpawn = 3;
    public float laneWidth = 2f;
    public GameObject coinPrefab;
    public int poolSize = 10;
}