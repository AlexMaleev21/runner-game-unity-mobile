using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private float initialSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedIncreasePerSecond;
    [SerializeField] private float scoreMultiplier;
    [SerializeField] private float noObstacleTime;

    [Header("Nickname Testing")]
    [Tooltip("Use the test nickname automatically on launch and skip the nickname input window.")]
    [SerializeField] private bool useTestNickname;
    [Tooltip("Nickname used when Use Test Nickname is enabled.")]
    [SerializeField] private string testNickname = "TestPlayer";

    public float InitialSpeed => initialSpeed;
    public float MaxSpeed => maxSpeed;
    public float SpeedIncreasePerSecond => speedIncreasePerSecond;
    public float ScoreMultiplier => scoreMultiplier;
    public float NoObstacleTime => noObstacleTime;
    public bool UseTestNickname => useTestNickname;
    public string TestNickname => testNickname;
}
