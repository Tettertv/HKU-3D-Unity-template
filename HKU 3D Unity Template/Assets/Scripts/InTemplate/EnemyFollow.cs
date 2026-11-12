using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    // the following tutorial was used for this: 
    // https://www.youtube.com/watch?v=UvDqnbjEEak

    public NavMeshAgent enemy;
    public Transform player;

    void Update()
    {
        enemy.SetDestination(player.position);
    }

}
