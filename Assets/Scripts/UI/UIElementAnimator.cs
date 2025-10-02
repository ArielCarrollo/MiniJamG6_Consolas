using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Un componente gen�rico para activar y desactivar GameObjects con una animaci�n de fade y/o escala.
/// Requiere que el objeto a animar tenga un componente CanvasGroup.
/// </summary>
public class UIElementAnimator : MonoBehaviour
{
    [Header("Configuraci�n del Efecto")]
    [Tooltip("La duraci�n en segundos de la animaci�n de entrada y salida.")]
    [SerializeField] private float animationDuration = 0.3f;

    [Tooltip("Curva de animaci�n para un efecto m�s suave y profesional. EaseInOut es una buena opci�n.")]
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Elige si quieres un efecto de fade, de escala, o ambos.")]
    [SerializeField] private AnimationType animationType = AnimationType.Both;

    public enum AnimationType { Fade, Scale, Both }

    // Usamos un diccionario para rastrear las corutinas activas por cada objeto.
    // Esto evita que se ejecuten m�ltiples animaciones sobre el mismo objeto a la vez.
    private readonly Dictionary<GameObject, Coroutine> activeAnimations = new Dictionary<GameObject, Coroutine>();

    /// <summary>
    /// Activa un objeto con el efecto configurado.
    /// </summary>
    /// <param name="target">El GameObject que se va a activar y animar.</param>
    public void Show(GameObject target)
    {
        if (target == null) return;

        // Si ya hay una animaci�n en curso sobre este objeto, la detenemos primero.
        if (activeAnimations.ContainsKey(target))
        {
            StopCoroutine(activeAnimations[target]);
            activeAnimations.Remove(target);
        }

        // Iniciamos la nueva corutina y la guardamos en el diccionario.
        Coroutine animationCoroutine = StartCoroutine(AnimateIn(target));
        activeAnimations.Add(target, animationCoroutine);
    }

    /// <summary>
    /// Desactiva un objeto con el efecto configurado.
    /// </summary>
    /// <param name="target">El GameObject que se va a desactivar y animar.</param>
    public void Hide(GameObject target)
    {
        if (target == null) return;

        // Si ya hay una animaci�n en curso, la detenemos.
        if (activeAnimations.ContainsKey(target))
        {
            StopCoroutine(activeAnimations[target]);
            activeAnimations.Remove(target);
        }

        // Iniciamos la corutina de salida.
        Coroutine animationCoroutine = StartCoroutine(AnimateOut(target));
        activeAnimations.Add(target, animationCoroutine);
    }

    /// <summary>
    /// Alterna la visibilidad de un objeto con efecto.
    /// </summary>
    /// <param name="target">El GameObject que se va a mostrar u ocultar.</param>
    public void Toggle(GameObject target)
    {
        if (target == null) return;

        if (target.activeSelf)
        {
            Hide(target);
        }
        else
        {
            Show(target);
        }
    }


    private IEnumerator AnimateIn(GameObject target)
    {
        // Obtenemos o a�adimos el componente CanvasGroup, necesario para el fade.
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }

        // Activamos el objeto para que la corutina pueda manipularlo.
        target.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            // Calculamos el progreso y lo aplicamos a la curva de animaci�n.
            float progress = elapsedTime / animationDuration;
            float curveValue = easeCurve.Evaluate(progress);

            // Aplicamos los efectos seg�n la configuraci�n.
            if (animationType == AnimationType.Fade || animationType == AnimationType.Both)
            {
                canvasGroup.alpha = curveValue;
            }
            if (animationType == AnimationType.Scale || animationType == AnimationType.Both)
            {
                target.transform.localScale = Vector3.one * curveValue;
            }

            elapsedTime += Time.unscaledDeltaTime; // Usamos unscaledDeltaTime para que funcione aunque el juego est� en pausa.
            yield return null; // Esperamos al siguiente frame.
        }

        // Nos aseguramos de que al final de la animaci�n los valores sean los correctos.
        canvasGroup.alpha = 1f;
        target.transform.localScale = Vector3.one;
        activeAnimations.Remove(target); // Liberamos el objeto del diccionario.
    }

    private IEnumerator AnimateOut(GameObject target)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            float curveValue = easeCurve.Evaluate(progress);

            // Hacemos la animaci�n a la inversa (de 1 a 0).
            if (animationType == AnimationType.Fade || animationType == AnimationType.Both)
            {
                canvasGroup.alpha = 1f - curveValue;
            }
            if (animationType == AnimationType.Scale || animationType == AnimationType.Both)
            {
                target.transform.localScale = Vector3.one * (1f - curveValue);
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Al final, desactivamos el objeto por completo.
        target.SetActive(false);
        activeAnimations.Remove(target);
    }
}