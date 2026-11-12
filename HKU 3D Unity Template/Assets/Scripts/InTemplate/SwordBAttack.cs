using UnityEngine;
using UnityEngine.InputSystem;

public class Sword_attack : MonoBehaviour
{
    //This code was made with the following Unity Discussions: https://discussions.unity.com/t/how-do-make-an-object-move-in-a-circle-around-the-player/221685/2
    public Transform Target;
    public float RotationSpeed = 1.0f;
    public float CircleRadius = 1.0f;
    public float ElevationOffset = 0.0f;

    public InputActionAsset InputActions;
    private InputAction playerAttackAction;

    private Vector3 positionOffset;
    private float angle;

    private float startAngle;
    private float distanceSwung;
    public float swingLength = 3.5f;

    public MeshRenderer swordMesh;

    private bool attacking = false;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        playerAttackAction = InputSystem.actions.FindAction("Attack");
        swordMesh.enabled = false;
    }

    void LateUpdate()
    {
        if (playerAttackAction.WasPressedThisFrame() && attacking == false)
        {
            //Calculate which angle the sword swing should start at and enable the attack
            attacking = true;
            swordMesh.enabled = true;
            distanceSwung = 0.0f;
            startAngle = (90f - Target.eulerAngles.y) * Mathf.Deg2Rad;
            Debug.Log("I can swing my sword sword");
        }

        if (attacking == true && distanceSwung < swingLength)
        {
            
            angle = startAngle + distanceSwung;

            //This makes it a nice circle
            positionOffset.Set(Mathf.Cos(angle) * CircleRadius,
                 ElevationOffset,
                 Mathf.Sin(angle) * CircleRadius);
            transform.position = Target.position + positionOffset;

            //change the angle of the blade so it doesn't cut you
            Vector3 outwardDirection = transform.position - Target.position;

            transform.rotation = Quaternion.LookRotation(outwardDirection);

            distanceSwung += Time.deltaTime * RotationSpeed;
        }
        else
        {
            attacking = false;
            swordMesh.enabled = false;
        }

    }

    
}
