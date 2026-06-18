using UnityEngine;

[CreateAssetMenu(fileName = "CoinSpawnConfig", menuName = "Game/CoinSpawnConfig")]
public class CoinSpawnConfig : ScriptableObject
{
    [Header("Prefab and Pool")]
    [Tooltip("Coin prefab used by the coin pool.")]
    [SerializeField] private GameObject coinPrefab;
    [Tooltip("Initial amount of coin instances created in the pool.")]
    [SerializeField] private int coinPoolSize = 64;

    [Header("Chain Length")]
    [Tooltip("Minimum amount of coins in one spawned chain.")]
    [SerializeField] private int minCoinsPerRow = 1;
    [Tooltip("Maximum amount of coins in one spawned chain.")]
    [SerializeField] private int maxCoinsPerRow = 6;

    [Header("Spawn Chances")]
    [Tooltip("Chance that a coin chain will spawn when an obstacle row spawns.")]
    [SerializeField, Range(0f, 1f)] private float rowSpawnChance = 0.85f;
    [Tooltip("Chance to place a coin chain as an arc over an obstacle when possible.")]
    [SerializeField, Range(0f, 1f)] private float arcOverObstacleChance = 0.65f;

    [Header("Positions")]
    [Tooltip("Default coin height above the ground.")]
    [SerializeField] private float coinY = 0.75f;
    [Tooltip("Fallback height used when PlayerConfig cannot provide a valid jump height.")]
    [SerializeField] private float aboveObstacleY = 2.45f;
    [Tooltip("How much of the player's reachable jump height is used for coins above obstacles.")]
    [SerializeField, Range(0.1f, 1f)] private float aboveObstacleJumpHeightMultiplier = 0.85f;
    [Tooltip("Distance between coins along the Z axis for regular chains.")]
    [SerializeField] private float coinSpacingZ = 0.85f;
    [Tooltip("Base Z spacing for chains placed above obstacles. It is scaled with current speed.")]
    [SerializeField] private float aboveObstacleCoinSpacingZ = 1.1f;
    [Tooltip("How strongly coin chain spacing grows from initial speed to max speed.")]
    [SerializeField, Range(0f, 1f)] private float coinSpacingSpeedScale = 0.2f;
    [Tooltip("Maximum multiplier applied to coin spacing at high speed.")]
    [SerializeField] private float maxCoinSpacingSpeedMultiplier = 1.25f;
    [Tooltip("Z distance around an obstacle that is treated as unsafe for regular-height coins.")]
    [SerializeField] private float obstacleZClearance = 1.1f;

    public GameObject CoinPrefab => coinPrefab;
    public int CoinPoolSize => Mathf.Max(coinPoolSize, MaxCoinsPerRow);
    public int MinCoinsPerRow => Mathf.Max(1, minCoinsPerRow);
    public int MaxCoinsPerRow => Mathf.Max(MinCoinsPerRow, maxCoinsPerRow);
    public float RowSpawnChance => rowSpawnChance;
    public float ArcOverObstacleChance => arcOverObstacleChance;
    public float CoinY => coinY;
    public float AboveObstacleY => aboveObstacleY;
    public float AboveObstacleJumpHeightMultiplier => aboveObstacleJumpHeightMultiplier;
    public float CoinSpacingZ => Mathf.Max(0.1f, coinSpacingZ);
    public float AboveObstacleCoinSpacingZ => Mathf.Max(0.1f, aboveObstacleCoinSpacingZ);
    public float CoinSpacingSpeedScale => Mathf.Max(0f, coinSpacingSpeedScale);
    public float MaxCoinSpacingSpeedMultiplier => Mathf.Max(1f, maxCoinSpacingSpeedMultiplier);
    public float ObstacleZClearance => Mathf.Max(0f, obstacleZClearance);
}
