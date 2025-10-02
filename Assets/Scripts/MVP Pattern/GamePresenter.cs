using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.InputSystem;

public class GamePresenter : MonoBehaviour
{
    public static GamePresenter Instance { get; private set; }

    [Header("Referencias")]
    public UIView view;
    public PlayerInput playerInput;
    public PlayerDataSO playerData;

    [Header("Configuración de Escena")]
    [TextArea(3, 5)]
    public string textoLiricaInicial;
    public string textoPistaEscena;
    public string proximaEscena;

    [Header("Sistema de Misión de Escena (Opcional)")]
    [Tooltip("Activa el sistema de misión para esta escena.")]
    public bool usarSistemaDeMision = false;
    [Tooltip("Lista de IDs de misión que deben completarse para progresar.")]
    public List<string> misionesRequeridas = new List<string>();
    [Tooltip("El ID de la misión final que desbloquea la transición (ej: 'faldaElegida').")]
    public string misionFinalID;

    [Header("Objetos de Misión")]
    public List<InteractableObject> objetosMisionInicial;
    public InteractableObject objetoMisionFinal;

    private HashSet<string> misionesCompletadas = new HashSet<string>();
    private bool misionFinalDesbloqueada = false;

    private Coroutine temporizadorVagonCoroutine;
    private float tiempoAcumulado = 0f; // NUEVO: Guarda el tiempo que hemos mirado
    private bool temporizadorCompletado = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        if (playerInput != null) playerInput.DeactivateInput();
    }

    private void Start()
    {
        StartCoroutine(RutinaDeInicio());
        // Reseteamos el progreso de la misión al iniciar la escena
        misionesCompletadas.Clear();
        misionFinalDesbloqueada = false;
        if (usarSistemaDeMision)
        {
            foreach (var obj in objetosMisionInicial)
            {
                obj.shimmerEffect?.StartShimmer();
            }
            objetoMisionFinal.shimmerEffect?.StopShimmer(); // Asegurarse de que el final no brilla
        }
    }

    private IEnumerator RutinaDeInicio()
    {
        // --- CORRECCIÓN DEL ERROR NullReferenceException ---
        // Solo ejecutamos la secuencia de intro si el panel y el texto existen en la escena actual
        if (view != null && view.panelInicial != null && view.textoInicialCentrado != null && !string.IsNullOrEmpty(textoLiricaInicial))
        {
            yield return view.SecuenciaInicial(textoLiricaInicial, 2f, 2f, 1.5f);
        }

        if (playerInput != null) playerInput.ActivateInput();

        // Actualizamos la UI, comprobando que no sea nula
        if (view != null)
        {
            view.ActualizarBarraCoraje(playerData.Coraje);
            view.MostrarPensamiento(textoPistaEscena, 1f);
        }
    }

    public void IniciarTransicionAEscena(string nombreEscena)
    {
        StartCoroutine(RutinaDeTransicion(nombreEscena, 1.5f));
    }

    private IEnumerator RutinaDeTransicion(string nombreEscena, float duracionFundido)
    {
        if (playerInput != null) playerInput.DeactivateInput();
        view.IniciarFundidoDeTransicion(duracionFundido);
        yield return new WaitForSeconds(duracionFundido);
        SceneManager.LoadScene(nombreEscena);
    }
    // --- MÉTODO CENTRAL DE LA MISIÓN ---

    public void CompletarMision(string id)
    {
        if (!usarSistemaDeMision || misionesCompletadas.Contains(id)) return;

        misionesCompletadas.Add(id);
        Debug.Log($"Misión completada: {id}. Progreso: {misionesCompletadas.Count}/{misionesRequeridas.Count}");

        // --- NUEVO: Apagar el brillo del objeto completado ---
        foreach (var obj in objetosMisionInicial)
        {
            if (obj.idDeMision == id)
            {
                obj.shimmerEffect?.StopShimmer();
                break;
            }
        }

        if (!misionFinalDesbloqueada && misionesCompletadas.Count >= misionesRequeridas.Count)
        {
            misionFinalDesbloqueada = true;
            view.MostrarPensamiento("Quizá... quizá mañana podría ser diferente.", 1f);
            VibrationManager.Vibrate(0.1f, 0.1f, 0.1f);

            // --- NUEVO: Activar el brillo del objeto final ---
            objetoMisionFinal.shimmerEffect?.StartShimmer();
        }
    }

    // --- EVENTOS DE INTERACCIÓN ---

    public void Evento_LeerDiario()
    {
        view.MostrarPanelDiario(true);
        // --- CORRECCIÓN: Desactivar control del jugador ---
        if (playerInput != null) playerInput.DeactivateInput();
        CompletarMision("diarioLeido");
    }

    public void Evento_CerrarDiario()
    {
        view.MostrarPanelDiario(false);
        // --- CORRECCIÓN: Devolver el control ---
        if (playerInput != null) playerInput.ActivateInput();
    }
    public void Evento_AtreverseConFalda()
    {
        if (usarSistemaDeMision && !misionFinalDesbloqueada)
        {
            view.MostrarPensamiento("Aún no... necesito pensar.", 0.8f);
            return;
        }

        // --- NUEVO: Apagar el brillo al interactuar ---
        objetoMisionFinal.shimmerEffect?.StopShimmer();

        Debug.Log("Falda elegida. El coraje aumenta.");
        playerData.Coraje = 0.5f;
        view.ActualizarBarraCoraje(playerData.Coraje);
        VibrationManager.Vibrate(0.5f, 0.5f, 0.2f);

        // Marcamos la misión final como completada
        CompletarMision(misionFinalID);

        // Inicia la cinemática del espejo y la transición
        StartCoroutine(RutinaFinalActo1());
    }

    private IEnumerator RutinaFinalActo1()
    {
        // Aquí iría la lógica de mover la cámara al espejo si la tuvieras.
        // Por ahora, solo mostramos el texto.
        view.MostrarPensamiento("Ya está. Hoy no hay vuelta atrás.", 1f);
        yield return new WaitForSeconds(3f);
        IniciarTransicionAEscena(proximaEscena);
    }
    public void Evento_ElegirFalda()
    {
        if (playerData.Coraje >= 0.5f) return; // Evitar que se active varias veces

        Debug.Log("Falda elegida. El coraje aumenta.");
        playerData.Coraje = 0.5f;
        view.ActualizarBarraCoraje(playerData.Coraje);

        // --- LLAMADA A LA VIBRACIÓN ---
        VibrationManager.Vibrate(0.5f, 0.5f, 0.2f); // Vibración corta y fuerte

        IniciarTransicionAEscena(proximaEscena);
    }

    public void EmpezarMiradaAlChico()
    {
        if (temporizadorCompletado) return;
        if (temporizadorVagonCoroutine == null)
        {
            temporizadorVagonCoroutine = StartCoroutine(RutinaTemporizadorVagon());
        }
    }

    public void TerminarMiradaAlChico()
    {
        if (temporizadorVagonCoroutine != null)
        {
            StopCoroutine(temporizadorVagonCoroutine);
            temporizadorVagonCoroutine = null;
        }
    }

    private IEnumerator RutinaTemporizadorVagon()
    {
        Debug.Log($"Reanudando temporizador. Tiempo actual: {tiempoAcumulado:F2}s");
        while (tiempoAcumulado < 15f)
        {
            tiempoAcumulado += Time.deltaTime;
            yield return null;
        }
        Debug.Log("¡Tiempo cumplido!");
        temporizadorCompletado = true;
        view.MostrarPensamiento("Se terminó el viaje.", 1f);
        VibrationManager.Vibrate(0.2f, 0.2f, 1f); // Vibración suave y larga
        yield return new WaitForSeconds(2f);
        IniciarTransicionAEscena(proximaEscena);
    }
}
