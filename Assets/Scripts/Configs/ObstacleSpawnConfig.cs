using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleSpawnConfig", menuName = "Game/ObstacleSpawnConfig")]
public class ObstacleSpawnConfig : ScriptableObject
{
    [SerializeField] private float initialSafeZone;
    [SerializeField] private float baseSpawnInterval;
    [SerializeField] private float minSpawnInterval;
    [Tooltip("How much the obstacle spawn interval is reduced when current speed reaches MaxSpeed.")]
    [SerializeField, Range(0f, 0.9f)] private float maxSpeedIntervalReduction = 0.35f;
    [SerializeField] private float postJumpSafetyTime = 0.2f;
    [SerializeField] private float laneWidth;
    [SerializeField] private int obstaclePoolSize;
    [SerializeField] private GameObject obstaclePrefab;
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
    public ObstacleType ObstacleType => ObstacleType.Planet;
    public int MinObstaclesPerRow => Mathf.Clamp(minObstaclesPerRow, 1, 3);
    public int MaxObstaclesPerRow => Mathf.Clamp(maxObstaclesPerRow, MinObstaclesPerRow, 3);
    public float SpawnZ => spawnZ;
    public float ObstacleY => obstacleY;

    public GameObject GetPrefab(ObstacleType type)
    {
        return obstaclePrefab;
    }

    public ObstacleType GetRandomType()
    {
        return ObstacleType;
    }
}

public enum ObstacleType
{
    Planet = 1,
    SwerveObstacle = 1,
    SlideObstacle = 2,
    JumpObstacle = 3
}
