using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public float initialSpeed = 5f;
    public float laneWidth = 2f;
    public float laneSwitchSpeed = 10f;
    public float jumpForce = 8f;
    public float jumpDuration = 1f;
    public float slideDuration = 1f;
    public float slideHeight = 0.5f;
    public float normalHeight = 1f;
    public LayerMask groundLayer;
}