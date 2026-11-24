using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //part of this script was made using this tutorial: (PlayerMoveAndRotate())
    //https://www.youtube.com/watch?v=2jqNoWmdKJ8

    //Part of this script was also made with Tarodev's 2d character controller (the jump and gravity)

    [Header("References")]
    [SerializeField] private PlayerMovementStats stats;
    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private Transform playerCamera;

    [SerializeField] private float turnSmoothingVelocity;

    private Vector2 playerMoveAmount;
    private float time;


    //JumpingVariables
    private float verticalVelocity;
    private bool wasGrounded;
    private bool canCoyote;
    private float timeJumpWasPressed;
    private float frameLeftGrounded = float.MinValue;
    private bool coyoteUsable;

    //FocusMode
    private Transform lockedOnEnemy;
    private bool lockedOn;

    //StateVariables... STATE? this? State? hit the rooooad dude, This is priiiivate variables
    private bool movementLocked; //Hmmmmm

    private MovementState movementState = MovementState.Grounded;
    public enum MovementState
    {
        Grounded,
        InAir,
        NoInput
    }

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
        canCoyote = true;
        timeJumpWasPressed = time;
    }

    private void Update()
    {
        time += Time.deltaTime;

        CheckGrounded();

        HandleJump();
        HandleGravity();

        switch (movementState)
        {
            case MovementState.Grounded:
                HandlePlayerMovement();
                break;

            case MovementState.InAir:
                HandlePlayerMovement();
                break;

            case MovementState.NoInput:
                break;
        }

        //HandleGravity();
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

        if (canCoyote && (grounded || CanUseCoyote()) && !movementLocked)
        {
            ExecuteJump();
        }

        if (canCoyote && time > timeJumpWasPressed + stats.JumpBuffer)
        {
            canCoyote = false;
        }
    }
    private bool CanUseCoyote()
    {
        if (!coyoteUsable || playerCharacterController.isGrounded || time >= frameLeftGrounded + stats.CoyoteTime)
        {
            return false;
        }

        return true;
    }

    private void ExecuteJump()
    {
        canCoyote = false;
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

    private void HandlePlayerMovement()
    {
        if (!lockedOn)
        {
            RotationalPlayerMovement();
            return;
        }
        else
        {
            FocusPlayerMovement();
        }
    }

    private void RotationalPlayerMovement()//Previously called PlayerMoveAndRotate
    {
        if (movementLocked) //Only fall when movement is locked, mainly during combat animations
        {
            Vector3 onlyUseGravity = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

            playerCharacterController.Move(onlyUseGravity);
            return;
        }

        Vector3 playerDirection = new Vector3(playerMoveAmount.x, 0f, playerMoveAmount.y).normalized;
        Vector3 verticalMove = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

        if (playerDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothingVelocity, stats.rotationalRotateDamp);

            transform.rotation = Quaternion.Euler(0f, smoothTargetAngle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            playerCharacterController.Move(moveDirection.normalized * stats.rotationalWalkSpeed * Time.deltaTime + verticalMove);
        }
        else
        {
            playerCharacterController.Move(verticalMove);
        }
    }

    private void FocusPlayerMovement()
    {
        RotateTowardsTarget();

        if (movementLocked)
        {
            Vector3 onlyUseGravity = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

            playerCharacterController.Move(onlyUseGravity);
            return;
        }

        float horizontalInput = playerMoveAmount.x;
        float verticalInput = playerMoveAmount.y;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 forwardMovement = forward * verticalInput;
        Vector3 strafeMovement = right * horizontalInput;

        float forwardSpeed = verticalInput >= 0f ? stats.forwardSpeedMultiplier : stats.backwardSpeedMultiplier; //Decide to use forward or backwardsmultiplier

        Vector3 moveDirection = forwardMovement * forwardSpeed + strafeMovement * stats.strafeSpeedMultiplier;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 verticalMove = Vector3.up * verticalVelocity * Time.deltaTime;

        playerCharacterController.Move(moveDirection * stats.focusWalkSpeed * Time.deltaTime + verticalMove);

    }

    private void RotateTowardsTarget()
    {
        if (!lockedOnEnemy)
        {
            Debug.Log("No Enemy Locked On");
            return;
        }

        Debug.Log("Rotating towards target!");

        Vector3 direction = lockedOnEnemy.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation, stats.focusRotateDamp * Time.deltaTime);
    }

    public void ChangeMovementState(MovementState newState) //Not sure if i'd want this public, makes sense that the playermanager could acces it but dont want state to just be toyed with by other scripts i think
    {
        if (movementState == newState)
        {
            Debug.Log("Tried to transition into state but was already in there" + newState.ToString());
            return;
        }


        ExitState(movementState); // Might want a switch case in for if different exits require different things, or just a single if to check if its the exeptional one

        EnterState(newState);
    }

    private void ExitState(MovementState state)//Things like cleanup of random shi or whatever
    {
        
    }

    private void EnterState(MovementState state)
    {
        movementState = state;
    }
    public void ApplyExternalMovement(Vector3 movement) //Other scripts (Mainly the attack scripts) use this to push the player.
    {
        playerCharacterController.Move(movement);
    }

    public void SetMovementLocked(bool locked) //Shi i dont think i like having this restriction here but it works.
    {
        movementLocked = locked;
    }

    public void ReceiveLockInfo(Transform target, bool locked)
    {
        lockedOnEnemy = target;
        lockedOn = locked;
    }
}
