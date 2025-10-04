using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Act1Presenter : GamePresenterBase
{
    [Header("Configuración Acto 1")]
    public string proximaEscena;
    [TextArea(3, 5)]
    public string textoLiricaInicial;

    [Header("Sistema de Misión Acto 1")]
    public List<MissionStep> pasosDeMision = new List<MissionStep>();
    public string misionFinalID;

    [Header("Objetos de Misión (EN ORDEN)")]
    public List<InteractableObject> objetosMisionInicial;
    public InteractableObject objetoMisionFinal;

    private HashSet<string> misionesCompletadas = new HashSet<string>();
    private bool misionFinalDesbloqueada = false;
    private int misionActualIndex = 0;
    private string textoPistaActual;

    protected override void Awake()
    {
        base.Awake(); // Llama al Awake() de GamePresenterBase (importante para el Singleton).
        if (playerInput != null) playerInput.DeactivateInput(); // Lógica específica de esta escena.
    }

    protected override void Start()
    {
        // No llamamos a base.Start() porque esta escena tiene una rutina de inicio especial.
        StartCoroutine(RutinaDeInicio());

        misionesCompletadas.Clear();
        misionFinalDesbloqueada = false;
        misionActualIndex = 0;
        ActualizarTextoDePista();

        // Apagamos todos los brillos...
        foreach (var obj in objetosMisionInicial) obj.shimmerEffect?.StopShimmer();
        objetoMisionFinal.shimmerEffect?.StopShimmer();

        // ...y encendemos solo el del primer objetivo.
        if (objetosMisionInicial.Count > 0)
        {
            objetosMisionInicial[0].shimmerEffect?.StartShimmer();
        }
    }

    private IEnumerator RutinaDeInicio()
    {
        if (view != null && view.panelInicial != null && !string.IsNullOrEmpty(textoLiricaInicial))
        {
            view.SecuenciaInicial(textoLiricaInicial, 2f, 2f, 1.5f);
            yield return new WaitForSeconds(2f + 2f + 1.5f);
        }
        if (playerInput != null) playerInput.ActivateInput();
        if (view != null) view.ActualizarBarraCoraje(playerData.Coraje);
    }

    // --- Lógica de Misión (Específica del Acto 1) ---

    private void ActualizarTextoDePista()
    {
        string textoPistaAnterior = textoPistaActual;

        // Busca la primera misión NO completada y muestra su pista.
        foreach (var paso in pasosDeMision)
        {
            if (!misionesCompletadas.Contains(paso.missionID))
            {
                textoPistaActual = paso.hintText;
                break; // Salimos del bucle en cuanto encontramos la pista.
            }
        }

        // Si todas las misiones se completaron, se actualiza el texto final.
        if (misionesCompletadas.Count >= pasosDeMision.Count && misionFinalDesbloqueada)
        {
            textoPistaActual = "Quizá... quizá mañana podría ser diferente.";
        }

        // Solo reproducimos el sonido si el texto de la pista ha cambiado.
        if (textoPistaActual != textoPistaAnterior && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("HintUpdate", 0.7f);
        }

        view.MostrarPista(textoPistaActual);
    }

    public void CompletarMision(string id)
    {
        if (misionesCompletadas.Contains(id)) return;

        // Comprobamos si es la misión final ANTES de acceder a la lista secuencial.
        if (id == misionFinalID)
        {
            misionesCompletadas.Add(id);
            Debug.Log($"Misión final completada: {id}");
            return;
        }

        // Si no es la misión final, continuamos con la lógica secuencial.
        if (misionActualIndex >= pasosDeMision.Count || id != pasosDeMision[misionActualIndex].missionID)
        {
            Debug.LogWarning($"Intento de completar la misión '{id}' fuera de orden.");
            return;
        }

        misionesCompletadas.Add(id);
        Debug.Log($"Misión completada: {id}");

        objetosMisionInicial[misionActualIndex].shimmerEffect?.StopShimmer();
        misionActualIndex++;

        // Activa el brillo del siguiente objeto en la secuencia.
        if (misionActualIndex < objetosMisionInicial.Count)
        {
            objetosMisionInicial[misionActualIndex].shimmerEffect?.StartShimmer();
        }

        ActualizarTextoDePista();

        // Desbloquea la misión final si todas las misiones iniciales están completas.
        if (!misionFinalDesbloqueada && misionesCompletadas.Count >= pasosDeMision.Count)
        {
            misionFinalDesbloqueada = true;
            ActualizarTextoDePista();
            VibrationManager.Vibrate(0.1f, 0.1f, 0.1f);
            objetoMisionFinal.shimmerEffect?.StartShimmer();
        }
    }

    // --- Métodos de Notificación (Comportamiento específico del Acto 1) ---

    public override void OnGazeEnterInteractable(InteractableObject objeto)
    {
        // Solo mostramos el texto si el objeto es el objetivo actual.
        bool esMisionActual = misionActualIndex < objetosMisionInicial.Count && objeto == objetosMisionInicial[misionActualIndex];
        bool esMisionFinal = misionFinalDesbloqueada && objeto == objetoMisionFinal;

        if (esMisionActual || esMisionFinal)
        {
            view.MostrarPensamiento(objeto.textoAlMirar, 1f);
            view.MostrarPrompt(objeto.textoDelPrompt);
        }
    }

    public override void OnGazeExitInteractable(InteractableObject objeto)
    {
        base.OnGazeExitInteractable(objeto); // Llama al método base para ocultar prompt y pensamiento.
        view.MostrarPista(textoPistaActual); // Muestra de nuevo la pista actual.
    }

    // --- Eventos (Específicos del Acto 1) ---

    public void Evento_LeerDiario()
    {
        if (IsUIPanelOpen()) return;

        SetUIPanelOpen(true); // Usa el método de la clase base para gestionar el estado y el input.

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("OpenBook", 1f);
        }

        view.MostrarPanelDiario(true);
        CompletarMision("diarioLeido");
    }

    public void Evento_CerrarDiario()
    {
        if (!IsUIPanelOpen()) return;

        SetUIPanelOpen(false); // Usa el método de la clase base.

        // Añadimos un sonido para cerrar el diario para mayor feedback.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("CloseBook", 1f);
        }

        view.MostrarPanelDiario(false);
    }

    public void Evento_AtreverseConFalda()
    {
        if (!misionFinalDesbloqueada)
        {
            view.MostrarPensamiento("Aún no... necesito pensar.", 0.8f);
            return;
        }

        objetoMisionFinal.shimmerEffect?.StopShimmer();

        Debug.Log("Falda elegida. El coraje aumenta.");
        playerData.Coraje = 0.5f;
        view.ActualizarBarraCoraje(playerData.Coraje);
        VibrationManager.Vibrate(0.5f, 0.5f, 0.2f);
        CompletarMision(misionFinalID);
        StartCoroutine(RutinaFinalActo1());
    }

    private IEnumerator RutinaFinalActo1()
    {
        view.MostrarPensamiento("Ya está. Hoy no hay vuelta atrás.", 1f);
        yield return new WaitForSeconds(3f);
        IniciarTransicionAEscena(proximaEscena);
    }
}