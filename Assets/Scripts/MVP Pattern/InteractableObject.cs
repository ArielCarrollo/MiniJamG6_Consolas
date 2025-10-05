using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ShimmerEffect))]
public class InteractableObject : MonoBehaviour
{
    [Header("Referencias")]
    public ShimmerEffect shimmerEffect;

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

    [Header("Efecto Titileo Manual")]
    public bool titileoManual = false;

    private void Awake()
    {
        shimmerEffect = GetComponent<ShimmerEffect>();
    }

    private void Start()
    {
        // CLAVE: Iniciar el shimmer si está marcado como manual
        if (titileoManual && shimmerEffect != null)
        {
            shimmerEffect.StartShimmer();
        }
    }

    public void ActivarTitileo()
    {
        titileoManual = true;
        shimmerEffect?.StartShimmer();
    }

    public void ApagarTitileo()
    {
        titileoManual = false;
        shimmerEffect?.StopShimmer();
    }
}

