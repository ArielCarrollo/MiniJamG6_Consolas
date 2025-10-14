using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Act5Presenter : GamePresenterBase
{
    [Header("Configuración de la Escena")]
    public string escenaMenu;
    public Color colorTransicionFinal = Color.red;

    [Header("Textos de la Escena")]
    public string textoLiricaInicial = "Y ya estamos llegando, mi vida ha cambiado.";
    public string textoTomaMano = "Me tomas la mano...";
    public string textoPistaBusqueda = "Busca su rostro entre la oscuridad...";
    public string textoBeso = "Me vuelvo valiente y te beso en los labios.";
    public string dialogoChico = "Siento como si te conociera de toda la vida.";
    public string pensamientoQuerer = "Dices que me quieres...";
    public string pensamientoRegalo = "...y yo te regalo...";
    public string promptFinal = "[Mantener {INTERACT}] ...el último soplo de mi corazón.";

    [Header("Componentes de la Escena")]
    [Tooltip("El objeto guía que tiene el script 'InteractableObject'.")]
    public InteractableObject guiaInteractable;
    public PlayerController playerController;
    public CameraController mainCameraController;

    [Header("Configuración de la Búsqueda")]
    public float tiempoParaEncontrar = 1.5f;

    // --> INICIO: NUEVAS VARIABLES PARA VIBRACIÓN POR PROXIMIDAD DE MIRA
    [Header("Configuración de Vibración por Proximidad de Mira")]
    [Tooltip("El ángulo máximo para que la vibración comience. Si la mira está más lejos, no vibra.")]
    public float maxAngleForVibration = 30f;
    [Tooltip("El ángulo en el que la vibración es máxima. Recompensa la precisión.")]
    public float minAngleForVibration = 2f;
    [Tooltip("La intensidad máxima de la vibración (0 a 1).")]
    [Range(0f, 1f)]
    public float maxVibrationIntensity = 0.7f;
    // --> FIN: NUEVAS VARIABLES

    [Header("Configuración de Feedback")]
    public string sfxTomaMano = "HandHold";
    public string sfxBeso = "SoftKiss";
    public string sfxLatidoUnico = "SingleHeartbeat";
    public string nombreSfxPista = "HintUpdate";

    private bool objetivoEncontrado = false;
    private Coroutine corrutinaDeEncontrar;
    private Coroutine corrutinaDeVibracion; // --> NUEVO: Referencia a la corrutina de vibración

    protected override void Start()
    {
        StartCoroutine(RutinaEscenaFinal());
    }

    // --- LÓGICA DE BÚSQUEDA USANDO TU SISTEMA DE GAZE ---

    public override void OnGazeEnterInteractable(InteractableObject objeto)
    {
        if (objeto == guiaInteractable && !objetivoEncontrado)
        {
            objeto.shimmerEffect?.StartShimmer();
            // Esta vibración actuará como un "clic" de confirmación.
            VibrationManager.Vibrate(0.2f, 0.2f, 0.1f);
            corrutinaDeEncontrar = StartCoroutine(RutinaEncontrarObjetivo());
        }
    }

    public override void OnGazeExitInteractable(InteractableObject objeto)
    {
        if (objeto == guiaInteractable)
        {
            objeto.shimmerEffect?.StopShimmer();
            // Detenemos la vibración del "clic", la corrutina de proximidad se reactivará sola.
            VibrationManager.Vibrate(0, 0, 0);
            if (corrutinaDeEncontrar != null)
            {
                StopCoroutine(corrutinaDeEncontrar);
            }
        }
    }

    private IEnumerator RutinaEncontrarObjetivo()
    {
        yield return new WaitForSeconds(tiempoParaEncontrar);
        objetivoEncontrado = true;
    }

    // --- SECUENCIA PRINCIPAL DE LA ESCENA ---

    private IEnumerator RutinaEscenaFinal()
    {
        // FASE 0: INICIO
        playerController?.CongelarMovimiento(true);
        mainCameraController?.PermitirRotacion(false);
        view.SecuenciaInicial(textoLiricaInicial, 1.5f, 2f, 2f);
        yield return new WaitForSeconds(5.5f);

        // FASE 1: CONTACTO
        SoundManager.Instance?.StopAmbience(1.0f);
        view.MostrarPensamiento(textoTomaMano, 3f);
        SoundManager.Instance?.PlaySFX(sfxTomaMano, 0.6f);
        VibrationManager.Vibrate(0.2f, 0.2f, 0.3f);
        yield return new WaitForSeconds(3f);
        view.OcultarPensamiento();

        // FASE 2: BÚSQUEDA
        mainCameraController?.PermitirRotacion(true);
        MostrarPistaConSonido(textoPistaBusqueda);

        // --> INICIO: Arrancamos la corrutina de vibración por proximidad.
        corrutinaDeVibracion = StartCoroutine(RutinaVibracionPorProximidadDeMira());

        // Esperamos hasta que la variable 'objetivoEncontrado' se vuelva 'true'
        yield return new WaitUntil(() => objetivoEncontrado);

        // --> INICIO: Detenemos la corrutina de vibración para evitar conflictos.
        if (corrutinaDeVibracion != null)
        {
            StopCoroutine(corrutinaDeVibracion);
        }

        // Objetivo encontrado, detenemos todo
        mainCameraController?.PermitirRotacion(false);
        guiaInteractable.shimmerEffect?.StopShimmer();
        MostrarPistaConSonido(""); // Limpiamos la pista
        VibrationManager.Vibrate(0, 0, 0); // Nos aseguramos de que toda vibración pare.

        // FASE 3: BESO Y DIÁLOGO
        view.MostrarPensamiento(textoBeso, 1f);
        SoundManager.Instance?.PlaySFX(sfxBeso, 0.5f);
        VibrationManager.Vibrate(0.4f, 0.4f, 0.4f);
        yield return new WaitForSeconds(4f);
        view.OcultarPensamiento();

        view.MostrarSubtitulo(dialogoChico);
        yield return new WaitForSeconds(4f);
        view.OcultarSubtitulo();
        yield return new WaitForSeconds(1f);

        // FASE 4: REGALO FINAL
        view.MostrarPensamiento(pensamientoQuerer, 1f);
        yield return new WaitForSeconds(2f);
        view.MostrarPensamiento(pensamientoRegalo, 1f);
        yield return new WaitForSeconds(2.5f);
        view.OcultarPensamiento();

        view.MostrarPrompt(promptFinal.Replace("{INTERACT}", "E"));

        bool inputRecibido = false;
        System.Action onInteract = () => inputRecibido = true;
        InputReader.OnInteract += onInteract;
        yield return new WaitUntil(() => inputRecibido);
        InputReader.OnInteract -= onInteract;

        yield return StartCoroutine(RutinaSoploFinal());

        // FASE 5: FINAL
        SoundManager.Instance.StopLoopingSFX();
        view.OcultarPrompt();
        yield return new WaitForSeconds(3f);

        view.SetColorDeTransicion(colorTransicionFinal);
        IniciarTransicionAEscena(escenaMenu);
    }

    // --> INICIO: NUEVA CORRUTINA PARA LA VIBRACIÓN POR PROXIMIDAD DE MIRA
    /// <summary>
    /// Gestiona una vibración continua basada en qué tan cerca está la mira del objeto guía.
    /// </summary>
    private IEnumerator RutinaVibracionPorProximidadDeMira()
    {
        while (!objetivoEncontrado)
        {
            if (mainCameraController != null && guiaInteractable != null)
            {
                // Dirección en la que la cámara está mirando
                Vector3 cameraForward = mainCameraController.transform.forward;
                // Dirección desde la cámara hacia el objeto guía
                Vector3 directionToTarget = (guiaInteractable.transform.position - mainCameraController.transform.position).normalized;

                // Calculamos el ángulo entre las dos direcciones
                float angle = Vector3.Angle(cameraForward, directionToTarget);

                // Mapeamos el ángulo a una intensidad de 0 a 1.
                // Si el ángulo es maxAngleForVibration o más, la intensidad es 0.
                // Si el ángulo es minAngleForVibration o menos, la intensidad es 1.
                float intensity = Mathf.InverseLerp(maxAngleForVibration, minAngleForVibration, angle);

                // Aplicamos la intensidad máxima configurable
                float finalIntensity = intensity * maxVibrationIntensity;

                // Hacemos vibrar el mando con la intensidad calculada
                VibrationManager.Vibrate(finalIntensity, finalIntensity, Time.deltaTime);
            }

            // Esperamos al siguiente frame para volver a calcular
            yield return null;
        }
    }
    // --> FIN: NUEVA CORRUTINA

    private IEnumerator RutinaSoploFinal()
    {
        var interactAction = playerInput.actions["Interact"];
        if (!interactAction.IsPressed()) yield break;

        SoundManager.Instance.PlayLoopingSFX(sfxLatidoUnico, 1.0f);
        float duracionFundidoLatido = 4f;
        float tiempoPresionado = 0f;

        while (interactAction.IsPressed() && tiempoPresionado < duracionFundidoLatido)
        {
            float volumen = Mathf.Lerp(1.0f, 0.0f, tiempoPresionado / duracionFundidoLatido);
            SoundManager.Instance.SetLoopingSFXVolume(volumen);
            tiempoPresionado += Time.deltaTime;
            yield return null;
        }
    }

    private void MostrarPistaConSonido(string texto)
    {
        view?.MostrarPista(texto);
        if (!string.IsNullOrEmpty(texto))
        {
            SoundManager.Instance?.PlaySFX(nombreSfxPista);
        }
    }
}