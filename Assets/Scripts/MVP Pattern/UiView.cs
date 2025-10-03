using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening; // ¡Importante! Añadir el namespace de DOTween

public class UIView : MonoBehaviour
{
    [Header("Elementos de la UI")]
    public TextMeshProUGUI textoInicialCentrado;
    public TextMeshProUGUI textoPensamientos;
    public TextMeshProUGUI textoPistaDeMision;
    public TextMeshProUGUI textoPrompt;

    [Header("Paneles de Fundido")]
    public CanvasGroup panelInicial;
    public CanvasGroup panelFadeTransicion;

    [Header("Barra de Coraje")]
    public Slider sliderCoraje;

    [Header("Paneles Específicos")]
    public GameObject panelDiario;
    private RectTransform diarioRectTransform; // Para la animación del diario
    private CanvasGroup diarioCanvasGroup;

    private RectTransform pistaRectTransform; // Para el efecto de barrido
    private float pistaAncho; // Para calcular las posiciones del barrido

    private void Awake()
    {
        // Guardamos la referencia al RectTransform de la pista
        if (textoPistaDeMision != null)
        {
            pistaRectTransform = textoPistaDeMision.GetComponent<RectTransform>();
            pistaAncho = pistaRectTransform.rect.width;
        }
        if (panelDiario != null)
        {
            diarioRectTransform = panelDiario.GetComponent<RectTransform>();
            diarioCanvasGroup = panelDiario.GetComponent<CanvasGroup>();
            if (diarioCanvasGroup == null) // Añadir si no existe
            {
                diarioCanvasGroup = panelDiario.AddComponent<CanvasGroup>();
            }
        }
    }
    void Start()
    {
        // Asegurarse de que todo empieza en el estado correcto
        textoPensamientos.text = "";
        textoPistaDeMision.text = "";
        textoPrompt.text = "";
        if (panelFadeTransicion) panelFadeTransicion.alpha = 0;
        if (panelInicial) panelInicial.alpha = 1;
        if (panelDiario != null) panelDiario.SetActive(false);
    }

    // --- MÉTODOS DE TEXTO CON TRANSICIÓN (DOTWEEN) ---

    public void MostrarPensamiento(string texto, float opacidad)
    {
        ActualizarTextoConFundido(textoPensamientos, texto, opacidad);
    }

    public void MostrarPista(string texto)
    {
        if (textoPistaDeMision != null)
        {
            // --- NUEVO: Llamamos al efecto de barrido ---
            ActualizarPistaConBarrido(texto);
        }
    }

    // Método genérico para animar cualquier texto con un fundido cruzado
    private void ActualizarTextoConFundido(TextMeshProUGUI elementoTexto, string nuevoTexto, float opacidadFinal)
    {
        elementoTexto.DOKill();

        if (elementoTexto.text == nuevoTexto) return;

        // --- CORRECCIÓN DE LA ANIMACIÓN DE APARICIÓN ---
        // La animación ahora es la misma tanto si el texto está vacío como si no.
        elementoTexto.DOFade(0, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            elementoTexto.text = nuevoTexto;

            // Si el nuevo texto está vacío, lo dejamos transparente.
            if (string.IsNullOrEmpty(nuevoTexto))
            {
                elementoTexto.alpha = 0;
            }
            else
            {
                elementoTexto.DOFade(opacidadFinal, 0.25f).SetEase(Ease.InQuad);
            }
        });
    }
    private void ActualizarPistaConBarrido(string nuevoTexto)
    {
        if (textoPistaDeMision.text == nuevoTexto) return;
        if (pistaRectTransform == null) return;

        pistaRectTransform.DOKill();

        // Secuencia de animación de barrido
        Sequence sequence = DOTween.Sequence();
        sequence.Append(pistaRectTransform.DOAnchorPosX(-pistaAncho, 0.3f).SetEase(Ease.InCubic)) // 1. Sale por la izquierda
                .AppendCallback(() => textoPistaDeMision.text = nuevoTexto) // 2. Cambia el texto
                .Append(pistaRectTransform.DOAnchorPosX(pistaAncho, 0)) // 3. Se reposiciona a la derecha (invisible)
                .Append(pistaRectTransform.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutCubic)); // 4. Entra desde la derecha
    }

    // --- MÉTODOS DE PANELES Y OTROS (DOTWEEN) ---

    public void MostrarPrompt(string texto)
    {
        textoPrompt.DOKill();
        textoPrompt.text = texto;
        textoPrompt.DOFade(1, 0.2f);
    }

    public void OcultarPrompt()
    {
        textoPrompt.DOKill();
        textoPrompt.DOFade(0, 0.2f).OnComplete(() => textoPrompt.text = "");
    }

    public void ActualizarBarraCoraje(float valor)
    {
        if (sliderCoraje != null)
        {
            // Usamos DOTween para que la barra se llene suavemente
            sliderCoraje.DOValue(valor, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    public void MostrarPanelDiario(bool mostrar)
    {
        if (diarioRectTransform == null) return;

        diarioRectTransform.DOKill();
        diarioCanvasGroup.DOKill();

        if (mostrar)
        {
            panelDiario.SetActive(true);

            // Simula una página abriéndose
            diarioRectTransform.localScale = new Vector3(0, 1, 1);
            diarioCanvasGroup.alpha = 0;

            DOTween.Sequence()
                .Append(diarioRectTransform.DOScaleX(1, 0.4f).SetEase(Ease.OutQuad))
                .Join(diarioCanvasGroup.DOFade(1, 0.3f));
        }
        else
        {
            DOTween.Sequence()
                .Append(diarioRectTransform.DOScaleX(0, 0.4f).SetEase(Ease.InQuad))
                .Join(diarioCanvasGroup.DOFade(0, 0.3f))
                .OnComplete(() => panelDiario.SetActive(false));
        }
    }


    // --- SECUENCIA INICIAL (DOTWEEN) ---

    // La corrutina se elimina y se reemplaza por este método
    public void SecuenciaInicial(string texto, float duracionFadeIn, float tiempoVisible, float duracionFadeOut)
    {
        if (panelInicial == null || textoInicialCentrado == null) return;

        // Preparamos los estados iniciales
        panelInicial.alpha = 1;
        textoInicialCentrado.text = texto;
        textoInicialCentrado.alpha = 0;

        // Creamos la secuencia completa con DOTween
        DOTween.Sequence()
            .Append(textoInicialCentrado.DOFade(1, duracionFadeIn)) // 1. Aparece el texto
            .AppendInterval(tiempoVisible)                          // 2. Pausa
            .Append(panelInicial.DOFade(0, duracionFadeOut))      // 3. Se desvanece todo el panel
            .OnComplete(() => panelInicial.gameObject.SetActive(false)); // 4. Se desactiva al final
    }

    // --- FUNDIDO DE TRANSICIÓN (DOTWEEN) ---

    public void IniciarFundidoDeTransicion(float duracion)
    {
        if (panelFadeTransicion == null) return;
        // Una sola línea para iniciar el fundido. ¡Mucho más limpio!
        panelFadeTransicion.DOFade(1, duracion);
    }
}