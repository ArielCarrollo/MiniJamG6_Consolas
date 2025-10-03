using UnityEngine;
using DG.Tweening; // ¡Importante!

/// <summary>
/// Un componente genérico para activar y desactivar GameObjects con animaciones de DOTween.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIElementAnimator : MonoBehaviour
{
    [Header("Configuración del Efecto")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease easeType = Ease.OutQuad;
    [SerializeField] private AnimationType animationType = AnimationType.Both;

    public enum AnimationType { Fade, Scale, Both }

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Activa y anima el GameObject.
    /// </summary>
    public void Show()
    {
        // Detiene animaciones previas para evitar conflictos
        transform.DOKill();
        canvasGroup.DOKill();

        gameObject.SetActive(true);

        if (animationType == AnimationType.Fade || animationType == AnimationType.Both)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, animationDuration).SetEase(easeType);
        }

        if (animationType == AnimationType.Scale || animationType == AnimationType.Both)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(originalScale, animationDuration).SetEase(easeType);
        }
    }

    /// <summary>
    /// Desactiva y anima el GameObject.
    /// </summary>
    public void Hide()
    {
        transform.DOKill();
        canvasGroup.DOKill();

        if (animationType == AnimationType.Fade || animationType == AnimationType.Both)
        {
            canvasGroup.DOFade(0f, animationDuration).SetEase(easeType);
        }

        if (animationType == AnimationType.Scale || animationType == AnimationType.Both)
        {
            transform.DOScale(Vector3.zero, animationDuration).SetEase(easeType);
        }

        // Desactivamos el objeto después de que la animación termine
        DOVirtual.DelayedCall(animationDuration, () => gameObject.SetActive(false));
    }
}