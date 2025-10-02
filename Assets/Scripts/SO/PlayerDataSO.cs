using UnityEngine;

// Esto crea una opci�n en el men� Assets para crear este objeto
[CreateAssetMenu(fileName = "PlayerData", menuName = "Walking Simulator/Player Data", order = 0)]
public class PlayerDataSO : ScriptableObject
{
    [Header("Estad�sticas del Jugador")]
    [Range(0f, 1f)]
    public float Coraje;

    // Podemos a�adir m�s variables aqu� en el futuro (ej. ProgresoNarrativo)

    // Este m�todo se puede llamar al empezar un nuevo juego para resetear los valores
    public void ResetearValores()
    {
        Coraje = 0.1f; // Empezamos con un poquito de coraje base
    }
}