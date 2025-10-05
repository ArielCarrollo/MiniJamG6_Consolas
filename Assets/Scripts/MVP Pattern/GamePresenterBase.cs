using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class GamePresenterBase : MonoBehaviour
{
    // El Singleton ahora es de tipo GamePresenterBase para que los hijos puedan acceder a él.
    public static GamePresenterBase Instance { get; private set; }

    [Header("Referencias Principales")]
    public UIView view;
    public PlayerInput playerInput;
    public PlayerDataSO playerData;

    // 'protected' significa que esta clase y las que hereden de ella pueden ver esta variable.
    protected bool isUIPanelOpen = false;

    // 'virtual' permite que las clases que hereden puedan sobreescribir este método si lo necesitan.
    protected virtual void Awake()
    {
        // Si ya existe una instancia de un presenter, destruimos este nuevo
        // y detenemos la ejecución para evitar errores.
        if (Instance != null)
        {
            Debug.LogError("Se ha detectado más de un GamePresenter en la escena. Destruyendo el duplicado.");
            Destroy(gameObject);
            return;
        }
        // Si no hay ninguna, esta se convierte en la instancia activa.
        Instance = this;
    }

    // --- ¡ESTA ES LA PARTE CLAVE QUE FALTABA! ---
    // Cuando la escena se descarga y el presenter se destruye,
    // limpiamos la variable estática para que el presenter de la siguiente escena
    // pueda tomar su lugar.
    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    protected virtual void Start()
    {
        if (playerInput != null) playerInput.ActivateInput();
        if (view != null) view.ActualizarBarraCoraje(playerData.Coraje);
    }

    // --- Métodos de Estado y Notificación ---

    public bool IsUIPanelOpen() => isUIPanelOpen;

    // Un método 'protected' para que solo los presenters hijos puedan cambiar el estado.
    protected void SetUIPanelOpen(bool isOpen)
    {
        isUIPanelOpen = isOpen;
        if (isOpen)
        {
            if (playerInput != null) playerInput.DeactivateInput();
        }
        else
        {
            if (playerInput != null) playerInput.ActivateInput();
        }
    }
    public virtual bool TryCloseUIPanel()
    {
        // Por defecto no hay nada que cerrar
        return false;
    }

    // Estos métodos son 'virtual' para que los presenters hijos puedan darles un comportamiento especial.
    public virtual void OnGazeEnterInteractable(InteractableObject objeto)
    {
        objeto.onGazeEnter?.Invoke();

        // Si tiene titileo manual, lo apaga al mirar
        if (objeto.titileoManual)
        {
            objeto.ApagarTitileo();
        }

        // Mostrar pensamiento/prompt como siempre
        view?.MostrarPensamiento(objeto.textoAlMirar, 1f);
        view?.MostrarPrompt(objeto.textoDelPrompt);
    }

    public virtual void OnGazeExitInteractable(InteractableObject objeto)
    {
        objeto.onGazeExit?.Invoke();
        view?.MostrarPensamiento("", 1f);
        view?.OcultarPrompt();
    }

    // --- Lógica de Transición (Común a todos) ---

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
    protected IEnumerator RutinaDeFundidoDeEntrada()
    {
        // Esperamos un frame para que todo se cargue correctamente
        yield return null;

        // Le pedimos a la vista que haga el fundido de transparente a visible
        if (view != null)
        {
            view.IniciarFundidoDeEntrada(0.5f); // Una duración corta para el fade-in
            yield return new WaitForSeconds(0.5f);
        }

        // Una vez que la pantalla es visible, activamos el control del jugador
        if (playerInput != null) playerInput.ActivateInput();
        if (view != null) view.ActualizarBarraCoraje(playerData.Coraje);
    }
}