using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap defaultActionMap;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction combatLockAction;

    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action OnJumpReleased;
    public event Action OnAttack;
    public event Action OnCombatLock;

    private int gameplayBlockers = 0;

    public bool GameplayBlocked => gameplayBlockers > 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        defaultActionMap = inputActions.FindActionMap("DefaultMovement");

        moveAction = defaultActionMap.FindAction("Move");
        jumpAction = defaultActionMap.FindAction("Jump");
        attackAction = defaultActionMap.FindAction("Attack");
        combatLockAction = defaultActionMap.FindAction("CombatLock");

        SubscribeInputs();
    }

    private void SubscribeInputs()
    {
        moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);

        jumpAction.performed += ctx => OnJump?.Invoke();
        jumpAction.canceled += ctx => OnJumpReleased?.Invoke();

        attackAction.performed += ctx => OnAttack?.Invoke();

        combatLockAction.performed += ctx => OnCombatLock?.Invoke();
    }

    private void OnEnable()
    {
        if (defaultActionMap != null && !GameplayBlocked)
            defaultActionMap.Enable();
    }

    private void OnDisable()
    {
        if (defaultActionMap != null)
            defaultActionMap.Disable();
    }

    public void BlockGameplay()
    {
        gameplayBlockers++;

        if (gameplayBlockers == 1)
        {

        }
            defaultActionMap.Disable();
    }

    public void UnblockGameplay()
    {
        gameplayBlockers = Mathf.Max(0, gameplayBlockers - 1);

        if (gameplayBlockers == 0 && isActiveAndEnabled)
        {
            defaultActionMap.Enable();
        }
    }

    public void PauseGame()
    {
        BlockGameplay();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        UnblockGameplay();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}