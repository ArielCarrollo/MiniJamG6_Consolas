using UnityEngine;
using UnityEngine.Events; // Necesario para UnityEvent

public class InteractableObject : MonoBehaviour
{
    [Header("Textos de Interacci�n")]
    [TextArea(3, 5)]
    public string textoAlMirar;
    public string textoDelPrompt;

    [Header("Evento")]
    public UnityEvent onInteract; // �La magia de la reusabilidad est� aqu�!
}