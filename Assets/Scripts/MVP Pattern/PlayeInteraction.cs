using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public UIView view;

    [Header("Configuración de Interacción")]
    public float distanciaInteraccion = 3f;
    public float radioEsfera = 0.5f;
    public LayerMask capaInteraccion;

    private Camera cam;
    private InteractableObject objetoDetectado; // Cambiado de 'objetoDetectadoAnterior' a 'objetoDetectado' para mayor claridad

    private RaycastHit lastHit;
    private bool hasHit;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable() => InputReader.OnInteract += Interactuar;
    private void OnDisable() => InputReader.OnInteract -= Interactuar;

    void Update()
    {
        hasHit = Physics.SphereCast(cam.transform.position, radioEsfera, cam.transform.forward, out lastHit, distanciaInteraccion, capaInteraccion);

        InteractableObject interactableActual = null;
        if (hasHit)
        {
            interactableActual = lastHit.collider.GetComponent<InteractableObject>();
        }

        // --- LÓGICA DE DETECCIÓN MEJORADA ---
        if (interactableActual != objetoDetectado)
        {
            // Dejamos de mirar el objeto anterior
            if (objetoDetectado != null)
            {
                view.OcultarPrompt(); // Ocultamos el prompt del objeto anterior
                objetoDetectado.onGazeExit.Invoke();
            }

            // Actualizamos el objeto que estamos mirando
            objetoDetectado = interactableActual;

            // Empezamos a mirar el nuevo objeto
            if (objetoDetectado != null)
            {
                // Mostramos el texto y el prompt del nuevo objeto
                view.MostrarPensamiento(objetoDetectado.textoAlMirar, 1f);
                view.MostrarPrompt(objetoDetectado.textoDelPrompt);
                objetoDetectado.onGazeEnter.Invoke();
            }
            else
            {
                // Si no miramos nada interactuable, mostramos el texto por defecto de la escena
                view.MostrarPensamiento(GamePresenter.Instance.textoPistaEscena, 1f);
            }
        }
    }

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