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
    public PlayerController playerController;
    public CameraController cameraController;

    [Header("Configuración de Escena General")]
    [TextArea(3, 5)]
    public string textoLiricaInicial;
    public string proximaEscena;
    private bool isUIPanelOpen = false;

    [Header("Sistema de Misión de Escena")]
    public bool usarSistemaDeMision = false;
    public List<MissionStep> pasosDeMision = new List<MissionStep>();
    public string misionFinalID;

    [Header("Objetos de Misión")]
    public List<InteractableObject> objetosMisionInicial;
    public InteractableObject objetoMisionFinal;

    private HashSet<string> misionesCompletadas = new HashSet<string>();
    private bool misionFinalDesbloqueada = false;
    private string textoPistaActual;
    private int misionActualIndex = 0;

    // Variables para el temporizador del Acto 3
    private Coroutine temporizadorVagonCoroutine;
    private float tiempoAcumulado = 0f;
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

        if (usarSistemaDeMision)
        {
            misionesCompletadas.Clear();
            misionFinalDesbloqueada = false;
            misionActualIndex = 0;

            ActualizarTextoDePista();

            // --- LÓGICA SECUENCIAL: Apagamos todos los brillos... ---
            foreach (var obj in objetosMisionInicial) obj.shimmerEffect?.StopShimmer();
            objetoMisionFinal.shimmerEffect?.StopShimmer();

            // ...y encendemos solo el del primer objetivo.
            if (objetosMisionInicial.Count > 0)
            {
                objetosMisionInicial[0].shimmerEffect?.StartShimmer();
            }
        }
    }

    private IEnumerator RutinaDeInicio()
    {
        if (view != null && view.panelInicial != null && view.textoInicialCentrado != null && !string.IsNullOrEmpty(textoLiricaInicial))
        {
            view.SecuenciaInicial(textoLiricaInicial, 2f, 2f, 1.5f);
            // Añadimos una espera manual para dar tiempo a que la animación termine.
            yield return new WaitForSeconds(2f + 2f + 1.5f);
        }

        if (playerInput != null) playerInput.ActivateInput();

        if (view != null)
        {
            view.ActualizarBarraCoraje(playerData.Coraje);
            if (!usarSistemaDeMision && pasosDeMision.Count == 0) // Muestra el texto de pista genérico si no hay misiones
            {
                textoPistaActual = " "; // Un texto por defecto si no hay misiones
                view.MostrarPensamiento(textoPistaActual, 1f);
            }
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

    // --- LÓGICA DEL SISTEMA DE MISIÓN ---
    private void ActualizarTextoDePista()
    {
        // Busca la primera misión NO completada y muestra su pista
        foreach (var paso in pasosDeMision)
        {
            if (!misionesCompletadas.Contains(paso.missionID))
            {
                textoPistaActual = paso.hintText;
                view.MostrarPista(textoPistaActual);
                return;
            }
        }

        if (misionFinalDesbloqueada)
        {
            textoPistaActual = "Quizá... quizá mañana podría ser diferente.";
            view.MostrarPista(textoPistaActual);
        }
    }

    public void CompletarMision(string id)
    {
        if (!usarSistemaDeMision || misionesCompletadas.Contains(id)) return;

        // --- SOLUCIÓN AL ERROR "OUT OF INDEX" ---
        // Comprobamos si es la misión final ANTES de acceder a la lista secuencial.
        if (id == misionFinalID)
        {
            misionesCompletadas.Add(id);
            Debug.Log($"Misión final completada: {id}");
            // No necesita más lógica aquí porque Evento_AtreverseConFalda ya lo gestiona.
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

        if (misionActualIndex < objetosMisionInicial.Count)
        {
            objetosMisionInicial[misionActualIndex].shimmerEffect?.StartShimmer();
        }

        ActualizarTextoDePista();

        if (!misionFinalDesbloqueada && misionesCompletadas.Count >= pasosDeMision.Count)
        {
            misionFinalDesbloqueada = true;
            ActualizarTextoDePista();
            VibrationManager.Vibrate(0.1f, 0.1f, 0.1f);
            objetoMisionFinal.shimmerEffect?.StartShimmer();
        }
    }

    // --- MÉTODOS DE NOTIFICACIÓN (Llamados por PlayerInteraction) ---

    public void OnGazeEnterInteractable(InteractableObject objeto)
    {
        // --- LÓGICA CONTEXTUAL MEJORADA ---
        // Solo mostramos el texto si el objeto es el objetivo actual
        if (usarSistemaDeMision)
        {
            // Comprueba si es un objeto de misión inicial y si es el actual
            bool esMisionActual = misionActualIndex < objetosMisionInicial.Count && objeto == objetosMisionInicial[misionActualIndex];
            // Comprueba si es el objeto final y si ya está desbloqueado
            bool esMisionFinal = misionFinalDesbloqueada && objeto == objetoMisionFinal;

            if (esMisionActual || esMisionFinal)
            {
                view.MostrarPensamiento(objeto.textoAlMirar, 1f);
                view.MostrarPrompt(objeto.textoDelPrompt);
            }
        }
        else // Si no es una escena de misión, funciona como siempre
        {
            view.MostrarPensamiento(objeto.textoAlMirar, 1f);
            view.MostrarPrompt(objeto.textoDelPrompt);
        }
    }

    public void OnGazeExitInteractable(InteractableObject objeto)
    {
        view.MostrarPensamiento("", 1f);
        if (usarSistemaDeMision)
        {
            view.MostrarPista(textoPistaActual);
        }
        view.OcultarPrompt();
    }
    public bool IsUIPanelOpen()
    {
        return isUIPanelOpen;
    }
    // --- MÉTODOS DE EVENTOS ESPECÍFICOS DE LA ESCENA 1 ---

    public void Evento_LeerDiario()
    {
        // Si el panel ya está abierto, no hacemos nada para evitar re-animaciones.
        if (isUIPanelOpen) return;

        view.MostrarPanelDiario(true);
        if (playerInput != null) playerInput.DeactivateInput();

        isUIPanelOpen = true; // ACTUALIZADO: Ponemos el interruptor en ON

        CompletarMision("diarioLeido");
    }

    public void Evento_CerrarDiario()
    {
        // Si no hay panel abierto, no hay nada que cerrar.
        if (!isUIPanelOpen) return;

        view.MostrarPanelDiario(false);
        if (playerInput != null) playerInput.ActivateInput();

        isUIPanelOpen = false; // ACTUALIZADO: Ponemos el interruptor en OFF
    }
    public void Evento_AtreverseConFalda()
    {
        if (usarSistemaDeMision && !misionFinalDesbloqueada)
        {
            // Esto es un pensamiento de rechazo, se mantiene
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

    // --- MÉTODOS DE EVENTOS ESPECÍFICOS DE LA ESCENA 3 ---

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
        VibrationManager.Vibrate(0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(2f);
        IniciarTransicionAEscena(proximaEscena);
    }
}
