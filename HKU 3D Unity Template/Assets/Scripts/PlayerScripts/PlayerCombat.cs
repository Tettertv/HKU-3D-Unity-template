using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerMovement playerMovement; //Direct reference to player movement might want to be turned into a call to the player manager (if im even gonna use that)

    private Transform lockedOnEnemy;
    private bool lockedOn;

    private CombatState currentState = CombatState.Idle;

    private Vector3 attackDirection; //Save what direction the attack started in.
    public enum CombatState
    {
        Idle,
        Attacking,
        Recovering
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack += AttackInput;
        }

        SwordHitBox.SetActive(false); //Remove this once attack logic gets hellybelly out of this scripts
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack -= AttackInput;
        }

        playerMovement.SetMovementLocked(false);
    }

    private void AttackInput()
    {
        if (currentState != CombatState.Idle)
        {
            Debug.Log("I am currently not in idle, add some input buffering ya dumbo");
            return;
        } 

        StartAttack();
    }

    private void StartAttack() //
    {
        Debug.Log("Attacking now!");
        currentState = CombatState.Attacking;

        /* attackDirection = playerCamera.forward;
        attackDirection.y = 0f;
        attackDirection.Normalize();

        transform.rotation = Quaternion.LookRotation(attackDirection);
        
        Was using this to turn the player to the direction that the cam is facing but it feels a bit yucky
        and if i'd want this i probs would also put it into something like FaceCameraDirection()
        */

        playerMovement.SetMovementLocked(true);//This might actually have to stay here

        attackDirection = transform.forward;

        StartCoroutine(ExecuteAttack()); //Replace this with the animator thingy underneath here when allat is set up

        //animator.SetTrigger("Attack"); I wanna use this to actually do the big lifting, sending event back when hitboxes need to be put on and shi.
        //uh i'd need a luh logic here to decide what a attack needs to be played based on combo and stuff, but i think this should just call a different script so that the animator doesn't even need to be on the PlayerCombatScript
    }

    private IEnumerator ExecuteAttack() //Stinky function that needs to put in a different class or whatever, that takes information from an SO, but this is game balancing, not optimizing/creating structurally sound shit but whatever who gaf (ITS ME!!!)
    {
        //Windup stuff here i suppose
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(PushPlayer());
        AudioManagerScript.Instance.PlayOneShot(swordWhipAudio, transform.position);
        SwordHitBox.SetActive(true);
        //Main attack
        
        yield return new WaitForSeconds(0.2f);
        StartRecovery(); //recovery (duh
        SwordHitBox.SetActive(false);

        yield return new WaitForSeconds(0.2f); //All this needs to be called from animator

        FinishAttack();
    }

    [Header("AttackStuff that needs to be somewhere else")]
    [SerializeField] private float attackLungeDistance = 1.0f;
    [SerializeField] private float attackLungeDuration = 0.2f; //All this data should be put into the attacks themselves, not in the main combat controller like here
    [SerializeField] private AudioEventSO swordWhipAudio;
    [SerializeField] private GameObject SwordHitBox;
    private IEnumerator PushPlayer() //Function that also eneds to be in a different attack class eventually when moving attack logic outta here
    {
        float elapsedTime = 0f;
        float previousPosition = 0f;

        while (elapsedTime < attackLungeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / attackLungeDuration); //I think i'll probably want to use DOTween here, but this is fine for now.

            float currentPosition = attackLungeDistance * (1f - Mathf.Pow(1f - t, 3f));

            float deltaMovement = currentPosition - previousPosition;

            playerMovement.ApplyExternalMovement(attackDirection * deltaMovement);

            previousPosition = currentPosition;

            yield return null;
        }
    }

    public void StartRecovery()
    {
        //also check if combo input is buffered already? this way you can also play with how fast the second and so forth' attack comes out. Not my problem rn
        //If i want a roll/dodge/other movement maybe turn the ability to do so on after this point?
        currentState = CombatState.Recovering;
    }

    public void FinishAttack()
    {
        //Check if combo input is buffered probs
        Debug.Log("I am done attacking");
        currentState = CombatState.Idle;
        playerMovement.SetMovementLocked(false);
    }

    public void ReceiveLockInfo(Transform target, bool locked)
    {
        lockedOnEnemy = target;
        lockedOn = locked;
    }

    //CameraManager.Instance.PlayImpact(timeScale: 0.1f,duration: 0.2f,shakeStrength: 1f,shakeDuration: 0.4f);
    //This up here is just what to call if i want hitstop and screenshake. Most likely want to just make this a scriptable object to pass through but it's not necessary rn.
}
