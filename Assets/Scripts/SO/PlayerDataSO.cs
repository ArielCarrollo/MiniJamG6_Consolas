using UnityEngine;

// Esto crea una opción en el menú Assets para crear este objeto
[CreateAssetMenu(fileName = "PlayerData", menuName = "Walking Simulator/Player Data", order = 0)]
public class PlayerDataSO : ScriptableObject
{
    [Header("Estadísticas del Jugador")]
    [Range(0f, 1f)]
    public float Coraje;

    // Podemos añadir más variables aquí en el futuro (ej. ProgresoNarrativo)

    // Este método se puede llamar al empezar un nuevo juego para resetear los valores
    public void ResetearValores()
    {
        Coraje = 0.1f; // Empezamos con un poquito de coraje base
    }
}