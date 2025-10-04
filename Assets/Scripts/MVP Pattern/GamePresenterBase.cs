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
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    protected virtual void Start()
    {
        // Lógica de inicio común a casi todas las escenas.
        if (playerInput != null) playerInput.ActivateInput();
        if (view != null)
        {
            view.ActualizarBarraCoraje(playerData.Coraje);
        }
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

    // Estos métodos son 'virtual' para que los presenters hijos puedan darles un comportamiento especial.
    public virtual void OnGazeEnterInteractable(InteractableObject objeto)
    {
        view.MostrarPensamiento(objeto.textoAlMirar, 1f);
        view.MostrarPrompt(objeto.textoDelPrompt);
    }

    public virtual void OnGazeExitInteractable(InteractableObject objeto)
    {
        view.MostrarPensamiento("", 1f);
        view.OcultarPrompt();
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
}