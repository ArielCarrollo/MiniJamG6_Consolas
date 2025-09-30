using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void Move()
    {
        agent.SetDestination(target.position);
    }
}
