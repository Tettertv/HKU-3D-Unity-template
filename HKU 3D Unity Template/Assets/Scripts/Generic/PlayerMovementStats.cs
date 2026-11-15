using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Scriptable Objects/PlayerMovementStats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Jump")]
    public float JumpHeight = 2.5f;
    public float Gravity = -20f;
    public float MaxFallSpeed = 25f;
    public float CoyoteTime = 0.15f;
    public float JumpBuffer = 0.2f;
}
