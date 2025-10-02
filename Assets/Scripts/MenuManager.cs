// Importamos el namespace de TextMeshPro para un texto de mejor calidad.
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Usar TextMeshPro en lugar de UnityEngine.UI
using System.Collections.Generic;

public class MenuManagerMejorado : MonoBehaviour
{
    // [System.Serializable] permite que esta estructura aparezca en el Inspector de Unity.
    // Así podemos organizar mejor la información de cada acto.
    [System.Serializable]
    public struct ActoInfo
    {
        public string nombreEscena;      // El nombre del archivo de la escena (ej: "Acto1_Habitacion")
        public string nombreParaMostrar; // El nombre que verá el jugador (ej: "La Habitación")
    }

    [Header("Paneles UI")]
    [Tooltip("Panel que contiene los botones de Jugar, Seleccionar Acto, Salir, etc.")]
    [SerializeField] private GameObject panelMenuPrincipal;
    [Tooltip("Panel para elegir un acto específico.")]
    [SerializeField] private GameObject panelSeleccionActo;

    [Header("UI Selección de Acto")]
    [Tooltip("Componente de texto para mostrar el nombre del acto seleccionado.")]
    [SerializeField] private TextMeshProUGUI textoActoSeleccionado; // Usamos TextMeshProUGUI para mejor calidad visual

    [Header("Configuración de Actos")]
    [Tooltip("Lista de todos los actos o niveles del juego.")]
    [SerializeField] private List<ActoInfo> actos;

    private int indiceActoActual = 0;

    private void Start()
    {
        // Aseguramos que los paneles y el texto inicial estén en el estado correcto.
        MostrarPanelPrincipal();
        ActualizarUITextoActo();
    }

    #region Navegación Principal
    public void BotonJugar()
    {
        // El botón "Jugar" siempre carga el primer acto de la lista.
        if (actos.Count > 0)
        {
            SceneManager.LoadScene(actos[0].nombreEscena);
        }
        else
        {
            Debug.LogWarning("No hay actos definidos en la lista para jugar.");
        }
    }

    public void BotonAbrirSeleccionActo()
    {
        panelMenuPrincipal.SetActive(false);
        panelSeleccionActo.SetActive(true);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        // La siguiente línea es útil para probar en el editor de Unity, ya que Application.Quit() no funciona ahí.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    #region Selección de Acto
    public void BotonCerrarSeleccionActo()
    {
        MostrarPanelPrincipal();
    }

    public void BotonSiguienteActo()
    {
        // Usamos el operador de módulo (%) para que el índice vuelva a 0 automáticamente
        // después de llegar al final de la lista. Es más limpio y eficiente.
        indiceActoActual = (indiceActoActual + 1) % actos.Count;
        ActualizarUITextoActo();
    }

    public void BotonAnteriorActo()
    {
        // Esta lógica maneja el caso de ir hacia atrás y llegar al principio de la lista.
        indiceActoActual--;
        if (indiceActoActual < 0)
        {
            indiceActoActual = actos.Count - 1;
        }
        ActualizarUITextoActo();
    }

    public void BotonJugarActoSeleccionado()
    {
        SceneManager.LoadScene(actos[indiceActoActual].nombreEscena);
    }
    #endregion

    #region Métodos Privados
    private void ActualizarUITextoActo()
    {
        if (textoActoSeleccionado != null && actos.Count > 0)
        {
            // Ahora simplemente leemos el nombre para mostrar, sin manipular strings.
            textoActoSeleccionado.text = actos[indiceActoActual].nombreParaMostrar;
        }
    }


    private void MostrarPanelPrincipal()
    {
        panelMenuPrincipal.SetActive(true);
        panelSeleccionActo.SetActive(false);
    }
    #endregion
}