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

    [Header("RotationalMovement")]
    public float rotationalWalkSpeed = 5f;
    public float rotationalRotateDamp = 0.1f;

    [Header("FocusMovement")]
    public float focusWalkSpeed = 5f;
    public float forwardSpeedMultiplier = 1f;
    public float backwardSpeedMultiplier = 0.85f;
    public float strafeSpeedMultiplier = 0.75f;
    public float focusRotateDamp = 12f;
}
