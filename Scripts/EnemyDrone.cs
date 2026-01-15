using UnityEngine;
using UnityEngine.AI;

public class EnemyDrone : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform player;
    public float viewDistance = 15f;

    private NavMeshAgent agent;
    private int currentPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextPoint();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < viewDistance)
        {
            agent.SetDestination(player.position);
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        if (waypoints.Length == 0) return;
        agent.destination = waypoints[currentPoint].position;
        currentPoint = (currentPoint + 1) % waypoints.Length;
    }
}