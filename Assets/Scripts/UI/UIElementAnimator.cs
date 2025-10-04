using UnityEngine;
using DG.Tweening;

public class UIElementAnimator : MonoBehaviour
{
    [Header("Configuración del Efecto")]
    [SerializeField] private float animationDuration = 0.4f;
    [SerializeField] private Ease easeType = Ease.OutBack; // Un ease con un poco más de personalidad
    [SerializeField] private AnimationType animationType = AnimationType.Both;

    public enum AnimationType { Fade, Scale, Both }

    public void Show(GameObject target)
    {
        // Detiene animaciones previas en el objeto para evitar conflictos
        target.transform.DOKill();
        CanvasGroup cg = GetCanvasGroup(target);
        cg.DOKill();

        target.SetActive(true);

        if (animationType == AnimationType.Fade || animationType == AnimationType.Both)
        {
            cg.alpha = 0f;
            cg.DOFade(1f, animationDuration).SetEase(easeType);
        }

        if (animationType == AnimationType.Scale || animationType == AnimationType.Both)
        {
            target.transform.localScale = Vector3.zero;
            target.transform.DOScale(1f, animationDuration).SetEase(easeType);
        }
    }

    public void Hide(GameObject target)
    {
        target.transform.DOKill();
        CanvasGroup cg = GetCanvasGroup(target);
        cg.DOKill();

        if (animationType == AnimationType.Fade || animationType == AnimationType.Both)
        {
            cg.DOFade(0f, animationDuration).SetEase(easeType);
        }

        if (animationType == AnimationType.Scale || animationType == AnimationType.Both)
        {
            target.transform.DOScale(Vector3.zero, animationDuration).SetEase(easeType);
        }

        // Desactivamos el objeto después de que la animación termine para optimizar
        DOVirtual.DelayedCall(animationDuration, () => target.SetActive(false));
    }

    // Método auxiliar para obtener o añadir un CanvasGroup al objeto
    private CanvasGroup GetCanvasGroup(GameObject target)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = target.AddComponent<CanvasGroup>();
        }
        return cg;
    }
}