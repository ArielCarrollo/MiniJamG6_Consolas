using UnityEngine;
using UnityEngine.InputSystem; // Puedes mantenerlo o quitarlo, ya no es estrictamente necesario aquí.

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public UIView view; // Referencia a la Vista para mostrar/ocultar prompts

    [Header("Configuración de Interacción")]
    public float distanciaInteraccion = 3f;
    [Tooltip("El radio de la esfera para detectar objetos. Aumenta esto para que sea más fácil apuntar.")]
    public float radioEsfera = 0.5f;
    public LayerMask capaInteraccion;

    private Camera cam;
    private InteractableObject objetoDetectado;

    // Almacenamos el último hit para dibujarlo con Gizmos
    private RaycastHit lastHit;
    private bool hasHit;

    // --- ELIMINADO ---
    // public InputActionAsset actions;
    // private InputAction interactAction;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        // --- ELIMINADO ---
        // interactAction = actions.FindActionMap("Player").FindAction("Interact");
    }

    // --- MODIFICADO: Ahora nos suscribimos al evento del InputReader ---
    private void OnEnable()
    {
        InputReader.OnInteract += Interactuar; // AÑADIDO
        // interactAction.Enable(); // ELIMINADO
    }

    // --- MODIFICADO: Nos desuscribimos para evitar errores ---
    private void OnDisable()
    {
        InputReader.OnInteract -= Interactuar; // AÑADIDO
        // interactAction.Disable(); // ELIMINADO
    }

    // --- ELIMINADO ---
    // private void Start() => interactAction.performed += _ => Interactuar();

    void Update()
    {
        // Esta lógica de SphereCast ya funciona bien y no necesita cambios.
        hasHit = Physics.SphereCast(
            cam.transform.position,
            radioEsfera,
            cam.transform.forward,
            out lastHit,
            distanciaInteraccion,
            capaInteraccion
        );

        if (hasHit)
        {
            var interactable = lastHit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (objetoDetectado != interactable)
                {
                    objetoDetectado = interactable;
                    view.MostrarPensamiento(objetoDetectado.textoAlMirar);
                    view.MostrarPrompt(objetoDetectado.textoDelPrompt);
                }
                return;
            }
        }

        if (objetoDetectado != null)
        {
            view.OcultarPrompt();
            view.MostrarPensamiento(GamePresenter.Instance.textoPistaEscena);
            objetoDetectado = null;
        }
    }

    // El método Interactuar ahora es llamado por el evento de InputReader.
    // No necesita cambios internos.
    private void Interactuar()
    {
        if (objetoDetectado != null)
        {
            Debug.Log("Interactuando con " + objetoDetectado.name);
            objetoDetectado.onInteract.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) return;

        Gizmos.color = hasHit ? Color.green : Color.red;

        if (hasHit)
        {
            Gizmos.DrawRay(cam.transform.position, cam.transform.forward * lastHit.distance);
            Gizmos.DrawWireSphere(cam.transform.position + cam.transform.forward * lastHit.distance, radioEsfera);
        }
        else
        {
            Gizmos.DrawRay(cam.transform.position, cam.transform.forward * distanciaInteraccion);
            Gizmos.DrawWireSphere(cam.transform.position + cam.transform.forward * distanciaInteraccion, radioEsfera);
        }
    }
}