using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tornado : MonoBehaviour
{
    public int damage = 3;
    public float forceMultiplier;
    public NavMeshAgent agent;
    public float wanderRadius = 10f;
    public int maxAttempts = 10;

    void Start()
    {
        InvokeRepeating(nameof(SetRandomDestination), 0f, 2.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerLife player))
        {
            Vector3 direction = (other.transform.position - transform.position) * forceMultiplier;
            player.GetDamage(damage, direction);
        }
    }

    void SetRandomDestination()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
            randomPoint.y = transform.position.y;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();

                if (agent.CalculatePath(hit.position, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }

    void Update()
    {
        
    }
}
