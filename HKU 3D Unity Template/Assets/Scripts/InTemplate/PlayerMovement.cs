using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //This script was made using this tutorial:
    //https://www.youtube.com/watch?v=2jqNoWmdKJ8

    public CharacterController playerCharacterController;

    private Vector2 playerMoveAmount;

    [SerializeField]
    private float playerWalkSpeed = 5.0f;
    [SerializeField]
    private float playerRotateDampening = 0.1f;
    [SerializeField]
    private float turnSmoothingVelocity;

    [SerializeField] private PlayerMovementStats stats;

    private float verticalVelocity;
    private bool wasGrounded;

    private bool jumpToConsume;
    private float timeJumpWasPressed;
    private float frameLeftGrounded = float.MinValue;
    private float time;

    private bool coyoteUsable;

    [SerializeField]
    private Transform playerCamera;

    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.Log("No Input manager found :(");
            return;
        }
        InputManager.Instance.OnMove += MoveInput;
        InputManager.Instance.OnJump += JumpInput;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnMove -= MoveInput;
        InputManager.Instance.OnJump -= JumpInput;
    }

    private void MoveInput(Vector2 movement)
    {
        playerMoveAmount = movement;
    }

    private void JumpInput()
    {
        jumpToConsume = true;
        timeJumpWasPressed = time;
    }

    private void Update()
    {
        time += Time.deltaTime;

        CheckGrounded();
        HandleJump();
        HandleGravity();

        PlayerMoveAndRotate();
    }

    private bool CanUseCoyote()
    {
        if (!coyoteUsable || playerCharacterController.isGrounded || time >= frameLeftGrounded + stats.CoyoteTime)
        {
            return false;
        }

        return true;
    }
    private void CheckGrounded()
    {
        bool grounded = playerCharacterController.isGrounded;

        if (wasGrounded && !grounded)
        {
            frameLeftGrounded = time;
        }

        wasGrounded = grounded;
    }

    private void HandleJump()
    {
        bool grounded = playerCharacterController.isGrounded;

        if (grounded)
        {
            coyoteUsable = true;
        }

        if (jumpToConsume && (grounded || CanUseCoyote()))
        {
            ExecuteJump();
        }

        if (jumpToConsume && time > timeJumpWasPressed + stats.JumpBuffer)
        {
            jumpToConsume = false;
        }
    }

    private void ExecuteJump()
    {
        jumpToConsume = false;
        coyoteUsable = false;

        verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(stats.Gravity) * stats.JumpHeight);
    }

    private void HandleGravity()
    {
        if (playerCharacterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += stats.Gravity * Time.deltaTime;

            verticalVelocity = Mathf.Max(verticalVelocity,-stats.MaxFallSpeed);
        }
    }

    private void PlayerMoveAndRotate()
    { 
        Vector3 playerDirection = new Vector3(playerMoveAmount.x,0f, playerMoveAmount.y).normalized;
        Vector3 verticalMove = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

        if (playerDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothingVelocity, playerRotateDampening);

            transform.rotation = Quaternion.Euler(0f, smoothTargetAngle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            playerCharacterController.Move(moveDirection.normalized * playerWalkSpeed * Time.deltaTime + verticalMove);
        }
        else
        {
            playerCharacterController.Move(verticalMove);
        }
    }
}
