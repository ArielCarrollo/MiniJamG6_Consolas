using UnityEngine;
using System.Collections;

public class ShimmerEffect : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material materialInstance;
    private Color baseEmissionColor = Color.black;

    [Header("Configuración del Brillo")]
    [Tooltip("El color del resplandor.")]
    [ColorUsage(true, true)] // Permite usar HDR para colores más intensos
    public Color shimmerColor = Color.white;

    [Tooltip("Qué tan intenso es el brillo máximo.")]
    [Range(0f, 2f)]
    public float intensity = 0.5f;

    private Coroutine shimmerCoroutine;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            materialInstance = objectRenderer.material;
            materialInstance.EnableKeyword("_EMISSION");
        }
    }

    public void StartShimmer()
    {
        if (shimmerCoroutine == null && materialInstance != null)
        {
            shimmerCoroutine = StartCoroutine(ShimmerRoutine());
        }
    }

    public void StopShimmer()
    {
        if (shimmerCoroutine != null)
        {
            StopCoroutine(shimmerCoroutine);
            shimmerCoroutine = null;
        }
        if (materialInstance != null)
        {
            materialInstance.SetColor("_EmissionColor", baseEmissionColor);
        }
    }

    private IEnumerator ShimmerRoutine()
    {
        float frequency = 1.5f;
        while (true)
        {
            // Usamos Sin para la pulsación y lo multiplicamos por el color y la intensidad
            float emissionValue = Mathf.Abs(Mathf.Sin(Time.time * frequency));
            Color finalColor = shimmerColor * emissionValue * intensity;
            materialInstance.SetColor("_EmissionColor", finalColor);
            yield return null;
        }
    }
}