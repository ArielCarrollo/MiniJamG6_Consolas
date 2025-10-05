using UnityEngine;
using System.Collections;

public class Act3Presenter : GamePresenterBase
{
    [Header("Configuración Acto 3")]
    public string proximaEscena;
    [TextArea(3, 8)]
    public string textoLiricoContexto = "De estación a estación, en frente tú y yo, va y viene el silencio.";
    public InteractableObject chicoInteractuable;
    public ChicoLowPolyController chicoController;
    public Act3CameraController camController;

    [Header("Tiempos narrativos")]
    public float duracionEscena = 30f;
    public float tiempoMinimoAntesReaccion = 5f; // Tiempo de observación libre
    public float tiempoMiradaParaReaccion = 2.0f;
    public float tiempoParaMostrarTristeza = 20f; // Si no mira al chico antes de este tiempo
    public float opacidadMinimaCoraje = 0.15f;

    [Header("Textos Narrativos")]
    [TextArea(2, 4)]
    [SerializeField] private string pistaInicial = "El silencio llena el vagón... Quiero verte.";
    [TextArea(2, 4)]
    [SerializeField] private string pistaContemplaChico = "Ahí está él, como siempre.";
    [TextArea(2, 4)]
    [SerializeField]private string pistaTrasReaccion = "Mi miró...";
    [TextArea(2, 4)]
    [SerializeField] private string pensamientoTristeza = "Hoy tampoco me atreví...";
    [TextArea(2, 4)]
    [SerializeField] private string pistaFinal = "Mañana será otro día.";

    // --- Variables de Estado ---
    private bool jugadorEstaMirandoAlChico = false;
    private bool miradaMutuaOcurrida = false;
    private bool puedeInteractuar = false;
    private bool mostroTristeza = false;
    private float tiempoMirandoChicoAcumulado = 0f;
    private string pistaActual;

    protected override void Awake()
    {
        base.Awake();
        if (playerInput != null) playerInput.DeactivateInput();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SecuenciaNarrativaCompleta());
    }

    private void Update()
    {
        if (!puedeInteractuar || miradaMutuaOcurrida)
            return;

        if (jugadorEstaMirandoAlChico)
        {
            // Actualizar pista si es la primera vez que mira
            if (pistaActual != pistaContemplaChico)
            {
                ActualizarPista(pistaContemplaChico);
            }

            // MEJORAR: Opacidad más baja al inicio, crece con el tiempo mirando
            float tiempoNormalizado = Mathf.Clamp01(tiempoMirandoChicoAcumulado / tiempoMiradaParaReaccion);
            float opacidadBase = Mathf.Lerp(0.05f, opacidadMinimaCoraje, tiempoNormalizado); // Empieza muy tenue
            float opacidadCoraje = Mathf.Lerp(opacidadBase, 1f, Mathf.Clamp01(playerData.Coraje));

            view.MostrarPensamiento(chicoInteractuable.textoAlMirar, opacidadCoraje);

            tiempoMirandoChicoAcumulado += Time.deltaTime;

            if (tiempoMirandoChicoAcumulado >= tiempoMiradaParaReaccion)
            {
                miradaMutuaOcurrida = true;
                StartCoroutine(ReaccionMiradaMutua());
            }
        }
        else
        {
            tiempoMirandoChicoAcumulado = 0f;
            view.MostrarPensamiento("", 0f);

            // Volver a la pista inicial si deja de mirar
            if (pistaActual == pistaContemplaChico)
            {
                ActualizarPista(pistaInicial);
            }
        }
    }


    private IEnumerator SecuenciaNarrativaCompleta()
    {
        // 1. Fundido de entrada
        view?.IniciarFundidoDeEntrada(0.5f);
        yield return new WaitForSeconds(0.5f);

        // 2. Mostrar texto lírico inicial
        if (view != null && view.panelInicial != null && !string.IsNullOrEmpty(textoLiricoContexto))
        {
            view.SecuenciaInicial(textoLiricoContexto, 1.0f, 2.2f, 0.8f);
            yield return new WaitForSeconds(1.0f + 2.2f + 0.8f);
        }

        // 3. Activar controles y UI inicial
        if (playerInput != null) playerInput.ActivateInput();
        if (view != null) view.ActualizarBarraCoraje(playerData.Coraje);

        // 4. Mostrar pista inicial
        ActualizarPista(pistaInicial);
        yield return new WaitForSeconds(2f);

        // 5. Permitir interacción después del tiempo mínimo
        yield return new WaitForSeconds(tiempoMinimoAntesReaccion - 2f);
        puedeInteractuar = true;

        // 6. Monitorear progreso durante la escena
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionEscena)
        {
            // Si no ha mirado al chico y ya pasó mucho tiempo, mostrar tristeza
            if (!mostroTristeza && !miradaMutuaOcurrida && tiempoTranscurrido >= tiempoParaMostrarTristeza)
            {
                mostroTristeza = true;
                StartCoroutine(MostrarSecuenciaTristeza());
            }

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // 7. Finalización
        StartCoroutine(SecuenciaFinal());
    }

    private void ActualizarPista(string nuevaPista)
    {
        if (pistaActual != nuevaPista)
        {
            pistaActual = nuevaPista;
            view.MostrarPista(pistaActual);

            // Sonido opcional de actualización de pista
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("HintUpdate", 0.4f);
            }
        }
    }

    private IEnumerator ReaccionMiradaMutua()
    {
        Debug.Log("Reacción Acto 3: iniciando secuencia emocional...");

        // PARAR la lógica de Update para evitar duplicación
        puedeInteractuar = false;

        // Actualizar pista tras la reacción
        ActualizarPista(pistaTrasReaccion);

        // Animaciones del chico y efectos
        if (chicoController != null) chicoController.MirarJugador();
        SoundManager.Instance?.PlaySFX("Latido", 0.7f);

        yield return new WaitForSeconds(1.0f);

        // Cámara baja (efecto "me hago pequeñita")
        if (camController != null) camController.AutomatizarBajadaMirada();

        // Reducir coraje y actualizar barra
        playerData.Coraje = Mathf.Max(0, playerData.Coraje - 0.15f);
        view.ActualizarBarraCoraje(playerData.Coraje);

        // Mostrar pensamiento final una sola vez con nueva opacidad
        yield return new WaitForSeconds(0.5f);
        float nuevaOpacidad = Mathf.Lerp(opacidadMinimaCoraje, 0.8f, Mathf.Clamp01(playerData.Coraje));
        view.MostrarPensamiento(chicoInteractuable.textoAlMirar, nuevaOpacidad);
        chicoInteractuable.shimmerEffect?.StopShimmer();
        Debug.Log("Fin de reacción emocional.");
    }

    private IEnumerator MostrarSecuenciaTristeza()
    {
        // Si nunca miró al chico, mostrar pensamiento de tristeza
        ActualizarPista(pistaFinal);
        view.MostrarPensamiento(pensamientoTristeza, 0.08f);
        yield return new WaitForSeconds(3f);
        view.MostrarPensamiento("", 0f);
    }

    private IEnumerator SecuenciaFinal()
    {
        puedeInteractuar = false;

        // Si no mostró la tristeza antes, mostrarla ahora
        if (!mostroTristeza && !miradaMutuaOcurrida)
        {
            yield return StartCoroutine(MostrarSecuenciaTristeza());
        }
        else if (!miradaMutuaOcurrida)
        {
            // Solo actualizar pista final si no hubo reacción
            ActualizarPista(pistaFinal);
            yield return new WaitForSeconds(2f);
        }

        // Limpiar UI antes de transición
        view?.MostrarPensamiento("", 0);
        view?.OcultarPrompt();
        view?.MostrarPista("");

        // Sonido final opcional
        SoundManager.Instance?.PlaySFX("Suspiro", 0.6f);

        IniciarTransicionAEscena(proximaEscena);
    }

    public override void OnGazeEnterInteractable(InteractableObject objeto)
    {
        if (objeto == chicoInteractuable)
        {
            jugadorEstaMirandoAlChico = true;
        }
    }

    public override void OnGazeExitInteractable(InteractableObject objeto)
    {
        if (objeto == chicoInteractuable)
        {
            jugadorEstaMirandoAlChico = false;
        }

        // Mantener la pista actual visible al dejar de mirar objetos
        if (!string.IsNullOrEmpty(pistaActual))
        {
            view.MostrarPista(pistaActual);
        }
    }

    public override bool TryCloseUIPanel()
    {
        return false; // No hay paneles UI en este acto
    }
}
