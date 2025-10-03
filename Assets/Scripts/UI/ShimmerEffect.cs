using UnityEngine;
using DG.Tweening; // ¡Importante! Añadir el namespace de DOTween

public class ShimmerEffect : MonoBehaviour
{
    [Header("Configuración del Brillo")]
    [SerializeField] private Color highlightEmissionColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private float shimmerDuration = 1.5f;

    private Material materialInstance;
    private Sequence shimmerSequence;

    private void Awake()
    {
        Renderer objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            materialInstance = objectRenderer.material;
            materialInstance.EnableKeyword("_EMISSION");
        }
    }

    public void StartShimmer()
    {
        if (materialInstance == null) return;

        // Detenemos cualquier secuencia anterior para evitar solapamientos
        shimmerSequence?.Kill();

        // Creamos una secuencia de DOTween
        shimmerSequence = DOTween.Sequence();
        shimmerSequence.Append(materialInstance.DOColor(highlightEmissionColor, "_EmissionColor", shimmerDuration))
                       .Append(materialInstance.DOColor(Color.black, "_EmissionColor", shimmerDuration))
                       .SetLoops(-1, LoopType.Restart); // Bucle infinito
    }

    public void StopShimmer()
    {
        // Detenemos la secuencia y reseteamos el color
        shimmerSequence?.Kill();
        if (materialInstance != null)
        {
            // Usamos un tween para que el apagado sea suave
            materialInstance.DOColor(Color.black, "_EmissionColor", 0.5f);
        }
    }

    private void OnDestroy()
    {
        // Buena práctica: asegurarse de detener los tweens cuando el objeto se destruye
        shimmerSequence?.Kill();
    }
}