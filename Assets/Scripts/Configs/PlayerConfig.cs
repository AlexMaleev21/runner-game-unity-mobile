using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Visuals")]
    [SerializeField] private GameObject playerModelPrefab;

    [Header("Movement")]
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float laneWidth = 2f;
    [SerializeField] private float laneSwitchSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float normalHeight = 1f;
    [SerializeField] private LayerMask groundLayer;

    public GameObject PlayerModelPrefab => playerModelPrefab;
    public float InitialSpeed => initialSpeed;
    public float LaneWidth => laneWidth;
    public float LaneSwitchSpeed => laneSwitchSpeed;
    public float JumpForce => jumpForce;
    public float JumpHeight => jumpHeight;
    public float JumpDuration => jumpDuration;
    public float NormalHeight => normalHeight;
    public LayerMask GroundLayer => groundLayer;

}
