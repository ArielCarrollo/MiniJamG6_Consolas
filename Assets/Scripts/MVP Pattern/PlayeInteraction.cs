using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public float distanciaInteraccion = 3f;
    public float radioEsfera = 0.5f;
    public LayerMask capaInteraccion;

    private Camera cam;
    private InteractableObject objetoDetectado;

    // Campos usados por Gizmos
    private RaycastHit lastHit;
    private bool hasHit;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = GetComponentInParent<Camera>();
    }

    private void OnEnable() => InputReader.OnInteract += Interactuar;
    private void OnDisable() => InputReader.OnInteract -= Interactuar;

    void Update()
    {
        RaycastHit hit;
        bool hitOk = Physics.SphereCast(
            cam.transform.position,
            radioEsfera,
            cam.transform.forward,
            out hit,
            distanciaInteraccion,
            capaInteraccion
        );

        // Actualiza campos (no variables locales)
        hasHit = hitOk;
        if (hitOk) lastHit = hit;

        InteractableObject interactableActual = hitOk ? hit.collider.GetComponent<InteractableObject>() : null;

        if (interactableActual != objetoDetectado)
        {
            // Notificar salida del anterior
            if (objetoDetectado != null && GamePresenterBase.Instance != null)
            {
                GamePresenterBase.Instance.OnGazeExitInteractable(objetoDetectado);
            }

            objetoDetectado = interactableActual;

            // Notificar entrada del nuevo
            if (objetoDetectado != null && GamePresenterBase.Instance != null)
            {
                GamePresenterBase.Instance.OnGazeEnterInteractable(objetoDetectado);
            }
        }
    }

    private void Interactuar()
    {
        var presenter = GamePresenterBase.Instance;

        // Si hay UI abierta, pedir al presenter que la cierre
        if (presenter != null && presenter.IsUIPanelOpen())
        {
            if (presenter.TryCloseUIPanel())
                return;
        }
        // Si no hay panel UI abierto y estamos mirando un interactuable...
        else if (objetoDetectado != null)
        {
            Debug.Log("Interactuando con " + objetoDetectado.name);
            objetoDetectado.onInteract?.Invoke();
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
