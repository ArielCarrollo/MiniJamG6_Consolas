using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado
using System.Collections;

public class UIView : MonoBehaviour
{
    [Header("Elementos de la UI")]
    public TextMeshProUGUI textoInicialCentrado; // NUEVO: Para el texto del principio
    public TextMeshProUGUI textoPensamientos;
    public TextMeshProUGUI textoPrompt;

    [Header("Paneles de Fundido")]
    public CanvasGroup panelInicial; // NUEVO: Para el fondo negro y texto inicial
    public CanvasGroup panelFadeTransicion; // ANTES: panelFade. Renombrado para claridad.

    void Start()
    {
        // Asegurarse de que todo empieza en el estado correcto
        textoPensamientos.text = "";
        textoPrompt.text = "";
        // El panel de transición empieza transparente
        if (panelFadeTransicion) panelFadeTransicion.alpha = 0;
        // El panel inicial empieza visible
        if (panelInicial) panelInicial.alpha = 1;
    }

    public void MostrarPensamiento(string texto)
    {
        textoPensamientos.text = texto;
    }

    public void LimpiarPensamiento()
    {
        textoPensamientos.text = "";
    }

    public void MostrarPrompt(string texto)
    {
        textoPrompt.text = texto;
    }

    public void OcultarPrompt()
    {
        textoPrompt.text = "";
    }

    // --- LÓGICA DE SECUENCIA INICIAL (NUEVA) ---

    // Esta corrutina maneja toda la introducción
    public IEnumerator SecuenciaInicial(string texto, float duracionFadeIn, float tiempoVisible, float duracionFadeOut)
    {
        // 1. Preparamos el panel y el texto
        panelInicial.alpha = 1; // El fondo negro aparece de inmediato
        textoInicialCentrado.text = texto;
        textoInicialCentrado.alpha = 0; // El texto empieza totalmente transparente

        // 2. --- NUEVO: Fundido de entrada para el texto ---
        float tiempoFadeIn = 0;
        while (tiempoFadeIn < duracionFadeIn)
        {
            // Aumentamos la opacidad del texto progresivamente
            textoInicialCentrado.alpha = Mathf.Lerp(0, 1, tiempoFadeIn / duracionFadeIn);
            tiempoFadeIn += Time.deltaTime;
            yield return null;
        }
        textoInicialCentrado.alpha = 1; // Nos aseguramos de que termine 100% visible

        // 3. Esperar unos segundos con el texto ya visible
        yield return new WaitForSeconds(tiempoVisible);

        // 4. Desvanecer todo el panel inicial (fondo y texto juntos)
        float tiempoFadeOut = 0;
        while (tiempoFadeOut < duracionFadeOut)
        {
            panelInicial.alpha = Mathf.Lerp(1, 0, tiempoFadeOut / duracionFadeOut);
            tiempoFadeOut += Time.deltaTime;
            yield return null;
        }
        panelInicial.alpha = 0;
        panelInicial.gameObject.SetActive(false);
    }


    // --- LÓGICA DE FUNDIDO PARA TRANSICIONES (Renombrado) ---

    public void IniciarFundidoDeTransicion(float duracion)
    {
        StartCoroutine(FundidoTransicionCoroutine(duracion));
    }

    private IEnumerator FundidoTransicionCoroutine(float duracion)
    {
        float tiempo = 0;
        while (tiempo < duracion)
        {
            panelFadeTransicion.alpha = Mathf.Lerp(0, 1, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }
        panelFadeTransicion.alpha = 1;
    }
}