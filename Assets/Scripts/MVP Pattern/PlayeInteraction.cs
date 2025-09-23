using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public UIView view; // Referencia a la Vista para mostrar/ocultar prompts

    [Header("Configuración de Raycast")]
    public float distanciaInteraccion = 3f;
    public LayerMask capaInteraccion; // Para que el raycast solo detecte objetos interactuables

    private Camera cam;
    private InteractableObject objetoDetectado; // El objeto que estamos mirando actualmente

    // Referencia al Input Action del nuevo Input System
    public InputActionAsset actions;
    private InputAction interactAction;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        interactAction = actions.FindActionMap("Player").FindAction("Interact");
    }

    private void OnEnable() => interactAction.Enable();
    private void OnDisable() => interactAction.Disable();
    private void Start() => interactAction.performed += _ => Interactuar();

    void Update()
    {
        // Lanzamos un raycast desde el centro de la cámara
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distanciaInteraccion, capaInteraccion))
        {
            // Si el raycast golpea un objeto interactuable
            var interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                // Si es un nuevo objeto, actualizamos la UI
                if (objetoDetectado != interactable)
                {
                    objetoDetectado = interactable;
                    view.MostrarPensamiento(objetoDetectado.textoAlMirar);
                    view.MostrarPrompt(objetoDetectado.textoDelPrompt);
                }
                return; // Salimos para no ejecutar el código de "no golpear nada"
            }
        }

        // Si el raycast no golpea nada o el objeto no es interactuable
        if (objetoDetectado != null)
        {
            view.OcultarPrompt();
            // Le pedimos al Presenter que muestre el texto por defecto de la escena
            view.MostrarPensamiento(GamePresenter.Instance.textoInicialEscena);
            objetoDetectado = null;
        }
    }

    private void Interactuar()
    {
        // Si estamos mirando un objeto y pulsamos la tecla de interactuar
        if (objetoDetectado != null)
        {
            Debug.Log("Interactuando con " + objetoDetectado.name);
            objetoDetectado.onInteract.Invoke(); // Disparamos el evento del objeto
        }
    }
}