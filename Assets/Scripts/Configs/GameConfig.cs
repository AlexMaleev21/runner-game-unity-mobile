using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private float initialSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedIncreasePerSecond;
    [SerializeField] private float scoreMultiplier;
    [SerializeField] private float noObstacleTime;

    public float InitialSpeed => initialSpeed;
    public float MaxSpeed => maxSpeed;
    public float SpeedIncreasePerSecond => speedIncreasePerSecond;
    public float ScoreMultiplier => scoreMultiplier;
    public float NoObstacleTime => noObstacleTime;
}