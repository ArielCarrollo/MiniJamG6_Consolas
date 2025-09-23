using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.Collections.Generic; 

public class MenuManager : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject panelMenuPrincipal;
    public GameObject panelSeleccionActo;

    [Header("UI Selección de Acto")]
    public Text textoActoSeleccionado; 

    private List<string> actos = new List<string>
    {
        "Acto1_Habitacion",
        "Acto2_EstacionRutina",
        "Acto3_VagonRutina",
        "Acto4_EstacionEncuentro",
        "Acto5_VagonFinal"
    };

    private int indiceActoActual = 0; 

    void Start()
    {
        panelMenuPrincipal.SetActive(true);
        panelSeleccionActo.SetActive(false);
        ActualizarUITextoActo(); 
    }


    public void BotonJugar()
    {
        SceneManager.LoadScene(actos[0]);
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
    }

    public void BotonCerrarSeleccionActo()
    {
        panelSeleccionActo.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    public void BotonSiguienteActo()
    {
        indiceActoActual++;

        if (indiceActoActual >= actos.Count)
        {
            indiceActoActual = 0;
        }

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
        SceneManager.LoadScene(actos[indiceActoActual]);
    }

    private void ActualizarUITextoActo()
    {
        if (textoActoSeleccionado != null)
        {
            textoActoSeleccionado.text = actos[indiceActoActual].Replace("Acto" + (indiceActoActual + 1) + "_", "");
        }
    }
}