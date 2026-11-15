using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.Log("No Input manager found :(");
            return;
        }

        InputManager.Instance.OnAttack += AttackInput;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnAttack -= AttackInput;
    }

    private void AttackInput() //Gets called whenever the player inputs the attack button
    {

    }
    
    //CameraManager.Instance.PlayImpact(timeScale: 0.1f,duration: 0.2f,shakeStrength: 1f,shakeDuration: 0.4f);
    //This up here is just what to call if i want hitstop and screenshake. Most likely want to just make this a scriptable object to pass through but it's not necessary rn.
}
