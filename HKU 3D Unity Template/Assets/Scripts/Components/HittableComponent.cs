using UnityEngine;

public class HittableComponent : MonoBehaviour
{
    [SerializeField] private AudioEventSO hitAudio1;
    [SerializeField] private GameObject hitVFX1;

    public void GetHit(PlayerAttackData incomingData)
    {
        Debug.Log("I just got hit!" + incomingData.baseDamage);

        AudioManagerScript.Instance.PlayOneShot(hitAudio1, transform.position); //Likely dont want this here

        GetComponent<WobbleTest>()?.Hit(); //Likely Dont want this here
        Instantiate(hitVFX1, transform);
    }
}
