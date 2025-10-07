using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem; // ¡IMPORTANTE! Añade este namespace.

public class MenuManagerMejorado : MonoBehaviour
{
    [System.Serializable]
    public struct ActoInfo
    {
        public string nombreEscena;
        public string nombreParaMostrar;
    }

    [Header("Referencias Clave")]
    [Tooltip("El componente PlayerInput del jugador. Esencial para cambiar entre control de juego y UI.")]
    [SerializeField] private PlayerInput playerInput; // ¡LA REFERENCIA MÁS IMPORTANTE!

    [Header("Paneles UI")]
    [SerializeField] private GameObject panelMenuPrincipal;
    [SerializeField] private GameObject panelSeleccionActo;

    [Header("UI Selección de Acto")]
    [SerializeField] private TextMeshProUGUI textoActoSeleccionado;

    [Header("Configuración de Actos")]
    [SerializeField] private List<ActoInfo> actos;

    [Header("Botones para Navegación con Mando")]
    [SerializeField] private GameObject primerBotonPrincipal;
    [SerializeField] private GameObject primerBotonSeleccionActo;

    private int indiceActoActual = 0;

    private void Start()
    {
        // --- ¡LA LÍNEA CLAVE QUE SOLUCIONA TODO! ---
        // Le decimos al PlayerInput que, para esta escena, use el mapa de acciones llamado "UI".
        // Asegúrate de que tu mapa de acciones de UI se llame exactamente "UI".
        playerInput?.SwitchCurrentActionMap("UI");

        // Ahora que el input está en el modo correcto, mostramos el panel y seleccionamos el botón.
        MostrarPanelPrincipal();
    }

    private void MostrarPanelPrincipal()
    {
        panelMenuPrincipal.SetActive(true);
        panelSeleccionActo.SetActive(false);
        StartCoroutine(EstablecerSeleccionado(primerBotonPrincipal));
    }

    private void MostrarPanelSeleccionActo()
    {
        panelMenuPrincipal.SetActive(false);
        panelSeleccionActo.SetActive(true);
        ActualizarUITextoActo();
        StartCoroutine(EstablecerSeleccionado(primerBotonSeleccionActo));
    }

    private IEnumerator EstablecerSeleccionado(GameObject boton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(boton);
    }

    #region Botones
    public void BotonJugar()
    {
        // Antes de cargar la escena de juego, volvemos al mapa "Gameplay".
        playerInput?.SwitchCurrentActionMap("Gameplay");
        if (actos.Count > 0)
        {
            SceneManager.LoadScene(actos[0].nombreEscena);
        }
    }

    public void BotonAbrirSeleccionActo()
    {
        MostrarPanelSeleccionActo();
    }

    public void BotonSalir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void BotonCerrarSeleccionActo()
    {
        MostrarPanelPrincipal();
    }

    public void BotonSiguienteActo()
    {
        indiceActoActual = (indiceActoActual + 1) % actos.Count;
        ActualizarUITextoActo();
    }

    public void BotonAnteriorActo()
    {
        indiceActoActual--;
        if (indiceActoActual < 0)
        {
            indiceActoActual = actos.Count - 1;
        }
        ActualizarUITextoActo();
    }

    public void BotonJugarActoSeleccionado()
    {
        // Antes de cargar la escena de juego, volvemos al mapa "Gameplay".
        playerInput?.SwitchCurrentActionMap("Gameplay");
        SceneManager.LoadScene(actos[indiceActoActual].nombreEscena);
    }
    #endregion

    private void ActualizarUITextoActo()
    {
        if (textoActoSeleccionado != null && actos.Count > 0)
        {
            textoActoSeleccionado.text = actos[indiceActoActual].nombreParaMostrar;
        }
    }
}