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
    public string textoPistaBusqueda = "Busca su rostro entre la oscuridad..."; // PISTA AÑADIDA
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

    [Header("Configuración de Feedback")]
    public string sfxTomaMano = "HandHold";
    public string sfxBeso = "SoftKiss";
    public string sfxLatidoUnico = "SingleHeartbeat";
    public string nombreSfxPista = "HintUpdate";

    private bool objetivoEncontrado = false;
    private Coroutine corrutinaDeEncontrar;

    protected override void Start()
    {
        StartCoroutine(RutinaEscenaFinal());
    }

    // --- LÓGICA DE BÚSQUEDA USANDO TU SISTEMA DE GAZE ---

    public override void OnGazeEnterInteractable(InteractableObject objeto)
    {
        // Si el objeto que miramos es nuestra guía y aún no la hemos encontrado...
        if (objeto == guiaInteractable && !objetivoEncontrado)
        {
            // ¡EL OBJETO BRILLA!
            objeto.shimmerEffect?.StartShimmer();
            VibrationManager.Vibrate(0.2f, 0.2f, 0.1f); // Vibración suave al encontrarlo con la mirada
            // Iniciamos la cuenta atrás para "encontrar" el objetivo.
            corrutinaDeEncontrar = StartCoroutine(RutinaEncontrarObjetivo());
        }
    }

    public override void OnGazeExitInteractable(InteractableObject objeto)
    {
        // Si dejamos de mirar la guía...
        if (objeto == guiaInteractable)
        {
            // ¡EL OBJETO DEJA DE BRILLAR!
            objeto.shimmerEffect?.StopShimmer();
            VibrationManager.Vibrate(0, 0, 0); // Detenemos vibración
            // Cancelamos la cuenta atrás.
            if (corrutinaDeEncontrar != null)
            {
                StopCoroutine(corrutinaDeEncontrar);
            }
        }
    }

    private IEnumerator RutinaEncontrarObjetivo()
    {
        yield return new WaitForSeconds(tiempoParaEncontrar);
        // Si hemos mantenido la mirada el tiempo suficiente, marcamos el objetivo como encontrado.
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
        MostrarPistaConSonido(textoPistaBusqueda); // MOSTRAMOS LA PISTA

        // Esperamos hasta que la variable 'objetivoEncontrado' se vuelva 'true' gracias a los métodos OnGaze
        yield return new WaitUntil(() => objetivoEncontrado);

        // Objetivo encontrado, detenemos todo
        mainCameraController?.PermitirRotacion(false);
        guiaInteractable.shimmerEffect?.StopShimmer();
        MostrarPistaConSonido(""); // Limpiamos la pista
        VibrationManager.Vibrate(0, 0, 0);

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

    private IEnumerator RutinaSoploFinal()
    {
        // CORREGIDO: Usamos 'playerInput' de la clase base, no 'playerInputRef'.
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