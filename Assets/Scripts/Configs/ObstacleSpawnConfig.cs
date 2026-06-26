using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleSpawnConfig", menuName = "Game/ObstacleSpawnConfig")]
public class ObstacleSpawnConfig : ScriptableObject
{
    private static readonly string[] DefaultPlanetResourcePaths =
    {
        "Prefabs/Planet1",
        "Prefabs/Planet2",
        "Prefabs/planet3",
        "Prefabs/planet4",
        "Prefabs/planet5"
    };

    [SerializeField] private float initialSafeZone;
    [SerializeField] private float baseSpawnInterval;
    [SerializeField] private float minSpawnInterval;
    [Tooltip("How much the obstacle spawn interval is reduced when current speed reaches MaxSpeed.")]
    [SerializeField, Range(0f, 0.9f)] private float maxSpeedIntervalReduction = 0.35f;
    [SerializeField] private float postJumpSafetyTime = 0.2f;
    [SerializeField] private float laneWidth;
    [SerializeField] private int obstaclePoolSize;
    [Header("Obstacle Prefabs")]
    [Tooltip("Fallback for old configs. Add asteroid prefabs to Asteroid Prefabs for new setups.")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject[] asteroidPrefabs;
    [SerializeField] private GameObject[] planetPrefabs;
    [SerializeField, Range(0f, 1f)] private float planetSpawnChance = 0.2f;

    [Header("Row Settings")]
    [SerializeField] private int minObstaclesPerRow = 1;
    [SerializeField] private int maxObstaclesPerRow = 3;
    [SerializeField] private float spawnZ = 20f;
    [SerializeField] private float obstacleY = 0f;

    public float InitialSafeZone => initialSafeZone;
    public float BaseSpawnInterval => baseSpawnInterval;
    public float MinSpawnInterval => minSpawnInterval;
    public float MaxSpeedIntervalReduction => maxSpeedIntervalReduction;
    public float PostJumpSafetyTime => postJumpSafetyTime;
    public float LaneWidth => laneWidth;
    public int ObstaclePoolSize => obstaclePoolSize;
    public GameObject ObstaclePrefab => obstaclePrefab;
    public ObstacleType ObstacleType => ObstacleType.Asteroid;
    public GameObject[] AsteroidPrefabs => asteroidPrefabs;
    public GameObject[] PlanetPrefabs => planetPrefabs;
    public float PlanetSpawnChance => planetSpawnChance;
    public int MinObstaclesPerRow => Mathf.Clamp(minObstaclesPerRow, 1, 3);
    public int MaxObstaclesPerRow => Mathf.Clamp(maxObstaclesPerRow, MinObstaclesPerRow, 3);
    public float SpawnZ => spawnZ;
    public float ObstacleY => obstacleY;

    public GameObject GetPrefab(ObstacleType type)
    {
        return type == ObstacleType.Planet
            ? GetRandomPlanetPrefab()
            : GetRandomAsteroidPrefab();
    }

    public ObstacleType GetRandomType()
    {
        return ObstacleType;
    }

    public GameObject GetRandomAsteroidPrefab()
    {
        GameObject prefab = GetRandomPrefab(asteroidPrefabs);
        return prefab != null ? prefab : obstaclePrefab;
    }

    public GameObject GetRandomPlanetPrefab()
    {
        GameObject prefab = GetRandomPrefab(planetPrefabs);
        if (prefab != null)
            return prefab;

        int startIndex = Random.Range(0, DefaultPlanetResourcePaths.Length);
        for (int offset = 0; offset < DefaultPlanetResourcePaths.Length; offset++)
        {
            prefab = Resources.Load<GameObject>(DefaultPlanetResourcePaths[(startIndex + offset) % DefaultPlanetResourcePaths.Length]);
            if (prefab != null)
                return prefab;
        }

        return null;
    }

    public bool ShouldSpawnPlanet()
    {
        return Random.value < planetSpawnChance && GetRandomPlanetPrefab() != null;
    }

    private static GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0)
            return null;

        int startIndex = Random.Range(0, prefabs.Length);
        for (int offset = 0; offset < prefabs.Length; offset++)
        {
            GameObject prefab = prefabs[(startIndex + offset) % prefabs.Length];
            if (prefab != null)
                return prefab;
        }

        return null;
    }
}

public enum ObstacleType
{
    Asteroid = 1,
    Planet = 2,
    SwerveObstacle = 1,
    SlideObstacle = 3,
    JumpObstacle = 4
}
