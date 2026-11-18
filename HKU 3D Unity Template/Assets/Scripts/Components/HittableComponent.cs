using UnityEngine;

public class HittableComponent : MonoBehaviour
{
    public void GetHit(PlayerAttackData incomingData)
    {
        Debug.Log("I just got hit!" + incomingData.baseDamage);
    }
}
