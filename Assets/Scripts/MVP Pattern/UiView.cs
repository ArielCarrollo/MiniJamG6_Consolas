using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado
using System.Collections;

public class UIView : MonoBehaviour
{
    [Header("Elementos de la UI")]
    public TextMeshProUGUI textoPensamientos;
    public TextMeshProUGUI textoPrompt;
    public CanvasGroup panelFade; // Un CanvasGroup para controlar el fundido a negro

    void Start()
    {
        // Asegurarse de que todo empieza oculto y transparente
        textoPensamientos.text = "";
        textoPrompt.text = "";
        if (panelFade) panelFade.alpha = 0;
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

    // Inicia el proceso de fundido a negro
    public void IniciarFundido(float duracion)
    {
        StartCoroutine(FundidoCoroutine(duracion));
    }

    private IEnumerator FundidoCoroutine(float duracion)
    {
        float tiempo = 0;
        while (tiempo < duracion)
        {
            panelFade.alpha = Mathf.Lerp(0, 1, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }
        panelFade.alpha = 1;
    }
}