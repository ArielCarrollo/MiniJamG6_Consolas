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
        RaycastHit hit;
        bool hasHit = Physics.SphereCast(cam.transform.position, radioEsfera, cam.transform.forward, out hit, distanciaInteraccion, capaInteraccion);

        InteractableObject interactableActual = hasHit ? hit.collider.GetComponent<InteractableObject>() : null;

        if (interactableActual != objetoDetectado)
        {
            // Notificamos al GamePresenter que hemos dejado de mirar el objeto anterior
            if (objetoDetectado != null)
            {
                GamePresenter.Instance.OnGazeExitInteractable(objetoDetectado);
            }

            objetoDetectado = interactableActual;

            // Notificamos al GamePresenter que estamos mirando un nuevo objeto
            if (objetoDetectado != null)
            {
                GamePresenter.Instance.OnGazeEnterInteractable(objetoDetectado);
            }
        }
    }
    private void Interactuar()
    {

        // 1. Preguntamos al Presenter si hay un panel abierto.
        if (GamePresenter.Instance.IsUIPanelOpen())
        {
            // Si lo hay, nuestra única acción es cerrarlo.
            GamePresenter.Instance.Evento_CerrarDiario();
        }
        // 2. Si no hay ningún panel abierto...
        else if (objetoDetectado != null)
        {
            // ...procedemos con la interacción normal del objeto que estamos mirando.
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