using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ShimmerEffect))] // Asegura que siempre tenga el efecto
public class InteractableObject : MonoBehaviour
{
    [Header("Referencias")]
    public ShimmerEffect shimmerEffect; // Referencia a su propio efecto de brillo

    [Header("Textos de Interacción")]
    [TextArea(3, 5)]
    public string textoAlMirar;
    public string textoDelPrompt;

    [Header("Lógica de Misión (Opcional)")]
    public bool esObjetoDeMision = false;
    public string idDeMision;

    [Header("Eventos")]
    public UnityEvent onInteract;
    public UnityEvent onGazeEnter;
    public UnityEvent onGazeExit;

    private void Awake()
    {
        // Asigna la referencia automáticamente al iniciar
        shimmerEffect = GetComponent<ShimmerEffect>();
    }
}