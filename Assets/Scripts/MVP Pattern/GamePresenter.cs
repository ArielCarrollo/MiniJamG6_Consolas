using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem; // Necesario para PlayerInput

public class GamePresenter : MonoBehaviour
{
    // --- Singleton para fácil acceso ---
    public static GamePresenter Instance { get; private set; }

    [Header("Referencias")]
    public UIView view;
    public PlayerInput playerInput; // NUEVO: Referencia para activar/desactivar control

    [Header("Configuración de Escena")]
    [TextArea(3, 5)]
    public string textoLiricaInicial; // NUEVO: Para la letra del principio
    public string textoPistaEscena; // ANTES: textoInicialEscena. Renombrado para claridad.
    public string proximaEscena;

    // --- El Modelo ---
    private GameState model;

    private void Awake()
    {
        // Lógica del Singleton
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Creamos una nueva instancia del modelo
        model = new GameState();

        // --- NUEVO: Desactivar control del jugador al inicio ---
        if (playerInput != null) playerInput.DeactivateInput();
    }

    private void Start()
    {
        // --- MODIFICADO: Iniciar la rutina de introducción ---
        StartCoroutine(RutinaDeInicio());
    }

    private IEnumerator RutinaDeInicio()
    {
        // 1. Llamar a la secuencia visual de la Vista y esperar a que termine
        yield return view.SecuenciaInicial(textoLiricaInicial, 2f, 2f, 1.5f);

        // 2. Activar el control del jugador
        if (playerInput != null) playerInput.ActivateInput();

        // 3. Mostrar el texto de "pista" o pensamiento persistente
        view.MostrarPensamiento(textoPistaEscena);
    }

    // --- MÉTODOS PÚBLICOS (Llamados por eventos del juego) ---

    public void Evento_ElegirFalda()
    {
        if (model.ProgresoNarrativo == 0)
        {
            Debug.Log("Falda elegida. El coraje aumenta.");

            // 1. Actualizar el Modelo
            model.Coraje = 0.5f;
            model.ProgresoNarrativo++;

            // --- NUEVO: Desactivar control antes de cambiar de escena ---
            if (playerInput != null) playerInput.DeactivateInput();

            // 2. Pedir a la Vista que inicie el fundido de transición
            view.IniciarFundidoDeTransicion(1.5f);

            // 3. Cargar la siguiente escena después del fundido
            StartCoroutine(CargarEscenaTrasRetraso(1.6f));
        }
    }

    private IEnumerator CargarEscenaTrasRetraso(float retraso)
    {
        yield return new WaitForSeconds(retraso);
        Debug.Log("Cargando escena: " + proximaEscena);
        SceneManager.LoadScene(proximaEscena);
    }
}