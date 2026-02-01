using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Thunder : MonoBehaviour
{
    public int damage = 6;
    public GameObject warning;
    public GameObject ray;
    public float warningDuration = 1f;
    public float rayDuration = 0.5f;
    public float thunderInterval = 5f;
    public Collider col;
    public NavMeshAgent agent;
    public float wanderRadius = 10f;
    public int maxAttempts = 10;

    void Start()
    {
        StartCoroutine(WarningAndStrike());
        InvokeRepeating(nameof(SetRandomDestination), 0f, 2.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerLife player))
        {
            player.GetDamage(damage, Vector3.zero);
        }
    }

    IEnumerator WarningAndStrike()
    {
        warning.SetActive(true);
        agent.isStopped = true;
        yield return new WaitForSeconds(warningDuration);
        col.enabled = true;
        warning.SetActive(false);
        ray.SetActive(true);
        yield return new WaitForSeconds(rayDuration);
        ray.SetActive(false);
        agent.isStopped = false;
        col.enabled = false;
        yield return new WaitForSeconds(thunderInterval);
        StartCoroutine(WarningAndStrike());
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
