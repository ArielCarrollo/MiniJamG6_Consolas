using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GamePresenter : MonoBehaviour
{
    // --- Singleton para fácil acceso ---
    public static GamePresenter Instance { get; private set; }

    [Header("Referencias")]
    public UIView view; // Referencia a la Vista (el script de la UI)

    [Header("Configuración de Escena")]
    public string textoInicialEscena;
    public string proximaEscena;

    // --- El Modelo ---
    private GameState model;

    private void Awake()
    {
        // Lógica del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Descomentar si quieres que persista entre escenas
        }

        // Creamos una nueva instancia del modelo
        model = new GameState();
    }

    private void Start()
    {
        // Al empezar la escena, le decimos a la vista que muestre el texto inicial
        view.MostrarPensamiento(textoInicialEscena);
    }

    // --- MÉTODOS PÚBLICOS (Llamados por eventos del juego) ---

    // Este es el evento que se activará al interactuar con la falda
    public void Evento_ElegirFalda()
    {
        if (model.ProgresoNarrativo == 0)
        {
            Debug.Log("Falda elegida. El coraje aumenta.");

            // 1. Actualizar el Modelo
            model.Coraje = 0.5f;
            model.ProgresoNarrativo++;

            // 2. Pedir a la Vista que inicie el fundido
            view.IniciarFundido(1.5f);

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