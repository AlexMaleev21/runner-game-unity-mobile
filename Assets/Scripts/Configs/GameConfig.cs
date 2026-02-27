using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float speedIncreasePerSecond = 0.2f;
    public float scoreMultiplier = 1f;
    public float noObstacleTime = 3f;
}