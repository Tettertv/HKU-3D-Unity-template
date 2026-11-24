using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private PlayerAttackData data;
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag) //Switch case for different types of things to get hit i suppose, probabaly dont care and let their own hittablecomponent handle what to do with getting hit
        {
            case "Enemy":
                HittableComponent target = other.GetComponent<HittableComponent>();

                target.GetHit(data);

                //Really shouldn't have this here but whatever (I have HittableComponent Now, move over to there at some point with some kind of HitDataSO
                CameraManager.Instance.PlayImpact(0.1f, 0.15f, 0.8f, 0.1f);
                break;
        }
    }
}
