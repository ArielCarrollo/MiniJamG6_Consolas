using UnityEngine;
using System.Collections;

public class Act4Presenter : GamePresenterBase
{
    [Header("Textos de la Escena")]
    [TextArea(3, 5)] public string textoLiricaInicial;
    [TextArea(2, 4)] public string textoPistaRutina = "Otro día más.";
    [TextArea(3, 5)] public string textoPensamientoRutina = "La misma estación de siempre. Hora de coger el tren.";
    [TextArea(3, 5)] public string textoPensamientoPanico;
    [TextArea(3, 5)] public string textoPensamientoResolucion = "Sí que puedo.";
    [TextArea(2, 4)] public string textoPistaAcercarse = "Acércate a él (Solo adelante).";

    [Header("Componentes de la Escena")]
    public Act4CameraController camController;
    public CameraController mainCameraController;
    public Transform objetivoZoom;
    public PlayerController playerController;
    public Transform playerTransform;
    public EventTrigger triggerVerAlChico;
    public EventTrigger triggerDialogo;

    [Header("Gestores de Escena")]
    public DialogueManager dialogueManager;

    [Header("Configuración de Gameplay")]
    public float fovZoom = 40f;
    public float duracionZoom = 3.0f;
    public float corajeGanado = 0.2f;

    [Header("Configuración de Feedback")]
    public string nombreSfxLatido = "Heartbeat";
    [Range(0f, 1f)] public float volumenLatido = 0.7f;
    public string nombreSfxPista = "HintUpdate";
    [Range(0f, 1f)] public float volumenPista = 0.8f;

    private bool escenaActiva = false;
    private bool haRespirado = false;
    private bool enDialogo = false;

    protected override void Awake()
    {
        base.Awake();
        triggerDialogo?.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        StartCoroutine(RutinaDeInicioActo4());
        triggerVerAlChico?.onPlayerEnter.AddListener(IniciarSecuenciaEncuentro);
        triggerDialogo?.onPlayerEnter.AddListener(IniciarSecuenciaDialogo);
    }

    private void OnEnable() => InputReader.OnInteract += Evento_Interactuar;
    private void OnDisable() => InputReader.OnInteract -= Evento_Interactuar;

    private void Evento_Interactuar()
    {
        if (escenaActiva && !haRespirado)
        {
            haRespirado = true;
            StartCoroutine(RutinaRespirarHondo());
        }
    }

    public void IniciarSecuenciaDialogo()
    {
        if (enDialogo) return;
        enDialogo = true;
        playerController?.CongelarMovimiento(true);
        MostrarPistaConSonido("");
        dialogueManager?.IniciarDialogo();
    }

    private void MostrarPistaConSonido(string texto)
    {
        view?.MostrarPista(texto);
        if (!string.IsNullOrEmpty(texto))
        {
            SoundManager.Instance?.PlaySFX(nombreSfxPista, volumenPista);
        }
    }

    private IEnumerator RutinaDeInicioActo4()
    {
        playerController?.CongelarMovimiento(true);
        mainCameraController?.PermitirRotacion(false);

        if (view != null && !string.IsNullOrEmpty(textoLiricaInicial))
        {
            view.SecuenciaInicial(textoLiricaInicial, 2f, 2f, 1.5f);
            yield return new WaitForSeconds(5.5f);
        }

        MostrarPistaConSonido(textoPistaRutina);
        view?.MostrarPensamiento(textoPensamientoRutina, 0.8f);

        playerController?.CongelarMovimiento(false);
        mainCameraController?.PermitirRotacion(true);
    }

    public void IniciarSecuenciaEncuentro()
    {
        playerController?.CongelarMovimiento(true);
        mainCameraController?.PermitirRotacion(false);
        StartCoroutine(RutinaEncuentroConChico());
    }

    private IEnumerator RutinaEncuentroConChico()
    {
        view?.OcultarPensamiento();
        MostrarPistaConSonido("");
        yield return new WaitForSeconds(0.5f);

        view?.MostrarPensamiento(textoPensamientoPanico, 0.9f);
        view?.ActualizarBarraCoraje(playerData.Coraje);
        SoundManager.Instance?.PlayLoopingSFX(nombreSfxLatido, volumenLatido);
        VibrationManager.Vibrate(0.7f, 0.7f, 1.5f);
        camController?.IniciarZoomHaciaObjetivo(objetivoZoom, fovZoom, duracionZoom);
        yield return new WaitForSeconds(1.0f);

        view?.MostrarPrompt("[E] Respirar Hondo");
        escenaActiva = true;
    }

    private IEnumerator RutinaRespirarHondo()
    {
        escenaActiva = false;
        view?.OcultarPrompt();
        SoundManager.Instance?.StopLoopingSFX();
        VibrationManager.Vibrate(0.3f, 0.3f, 0.2f);

        if (playerData != null)
        {
            playerData.Coraje += corajeGanado;
            view?.ActualizarBarraCoraje(playerData.Coraje);
        }

        view?.OcultarPensamiento();
        yield return new WaitForSeconds(0.4f);
        view?.MostrarPensamiento(textoPensamientoResolucion, 1f);
        yield return new WaitForSeconds(1.5f);
        view?.OcultarPensamiento();

        MostrarPistaConSonido(textoPistaAcercarse);

        playerController?.CongelarMovimiento(false);
        playerController?.PermitirMovimientoLateral(false);

        triggerDialogo?.gameObject.SetActive(true);
    }

    public void FinalizarDialogoYActivarSeguimiento(NPC npc, string pista, string pensamiento)
    {
        MostrarPistaConSonido(pista);
        view?.MostrarPensamiento(pensamiento, 0.9f);
        VibrationManager.Vibrate(0.4f, 0.4f, 0.3f);

        playerController?.CongelarMovimiento(false);
        playerController?.PermitirMovimientoLateral(true);
        mainCameraController?.PermitirRotacion(true);

        npc?.IniciarSeguimiento(playerTransform);
    }
}