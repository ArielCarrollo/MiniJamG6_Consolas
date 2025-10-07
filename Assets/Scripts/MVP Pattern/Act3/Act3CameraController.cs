using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Act3CameraController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInput playerInput;

    [Header("Control Manual")]
    [SerializeField] private float sensitivity = 200f;
    private Vector2 rotation;
    private Vector2 delta;
    private bool controlManualActivo = true;

    [Header("Animaciones Automáticas")]
    [SerializeField] private float velocidadBajada = 1.5f;
    [SerializeField] private float anguloMiradaAbajo = 25f;
    [SerializeField] private float duracionTemblor = 0.3f;
    [SerializeField] private float intensidadTemblor = 0.5f;

    private Vector3 rotacionOriginal;
    private bool animacionEnCurso = false;
    private Quaternion rotacionObjetivo; // Para interpolación suave

    private void Start()
    {
        rotacionOriginal = transform.eulerAngles;
        rotation = new Vector2(rotacionOriginal.y, -rotacionOriginal.x);
        rotacionObjetivo = transform.rotation;
    }

    private void OnEnable() => InputReader.OnDelta += MoveCamera;
    private void OnDisable() => InputReader.OnDelta -= MoveCamera;

    private void Update()
    {
        if (animacionEnCurso)
        {
            // Durante animación, interpolar suavemente hacia el objetivo
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 2f);
            return;
        }

        // Control manual normal
        if (controlManualActivo && playerInput != null && playerInput.actions.enabled)
        {
            rotation += delta * sensitivity * Time.deltaTime;
            rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
            transform.rotation = Quaternion.Euler(-rotation.y, rotation.x, 0f);
        }
    }

    private void MoveCamera(Vector2 value)
    {
        if (!animacionEnCurso) // Solo aceptar input si no hay animación
        {
            delta = value;
        }
    }

    public void AutomatizarBajadaMirada()
    {
        if (animacionEnCurso) return;
        StartCoroutine(SecuenciaBajadaMirada());
    }

    private System.Collections.IEnumerator SecuenciaBajadaMirada()
    {
        animacionEnCurso = true;
        controlManualActivo = false;
        delta = Vector2.zero; // Resetear input

        // 1. Temblor inicial
        float tiempoTemblor = 0f;
        Vector3 posicionOriginal = transform.position;

        while (tiempoTemblor < duracionTemblor)
        {
            Vector3 temblor = Random.insideUnitSphere * intensidadTemblor;
            transform.position = posicionOriginal + temblor;
            tiempoTemblor += Time.deltaTime;
            yield return null;
        }

        transform.position = posicionOriginal;

        // 2. Calcular rotación objetivo absoluta
        Vector3 rotacionActual = transform.eulerAngles;
        Vector3 rotacionFinal = new Vector3(anguloMiradaAbajo, rotacionActual.y, rotacionActual.z);
        rotacionObjetivo = Quaternion.Euler(rotacionFinal);

        // 3. Animar hacia el objetivo durante el tiempo especificado
        float tiempoTranscurrido = 0f;
        Quaternion rotacionInicial = transform.rotation;

        while (tiempoTranscurrido < velocidadBajada)
        {
            float t = tiempoTranscurrido / velocidadBajada;
            t = Mathf.SmoothStep(0f, 1f, t); // Suavizar la curva

            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionObjetivo, t);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Asegurar que llegue exactamente al objetivo
        transform.rotation = rotacionObjetivo;

        // 4. Actualizar variables de rotación para mantener coherencia
        Vector3 finalEuler = rotacionObjetivo.eulerAngles;
        rotation.x = finalEuler.y;
        rotation.y = -finalEuler.x;

        yield return new WaitForSeconds(1f);

        // 5. Reactivar control
        controlManualActivo = true;
        animacionEnCurso = false;
    }

    public void CongelarControl()
    {
        controlManualActivo = false;
        animacionEnCurso = true;
        delta = Vector2.zero;
    }

    public void ReactivarControl()
    {
        controlManualActivo = true;
        animacionEnCurso = false;
    }
}
