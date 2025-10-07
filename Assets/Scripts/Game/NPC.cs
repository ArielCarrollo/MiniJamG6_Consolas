using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [Tooltip("La distancia a la que el NPC se detendrá del jugador.")]
    public float followDistance = 2.0f;

    private NavMeshAgent agent;
    private Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Si tiene un objetivo, actualiza su destino constantemente.
        // Esto es necesario porque el jugador se está moviendo.
        if (target != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(target.position);
        }
    }

    /// <summary>
    /// Le dice al NPC que comience a seguir a un nuevo objetivo.
    /// </summary>
    public void IniciarSeguimiento(Transform nuevoTarget)
    {
        target = nuevoTarget;
        if (agent != null)
        {
            // ¡Clave! Asignamos la distancia de seguimiento.
            agent.stoppingDistance = followDistance;
            agent.isStopped = false;
        }
    }

    /// <summary>
    /// Detiene el movimiento del NPC.
    /// </summary>
    public void DetenerSeguimiento()
    {
        target = null;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }
}