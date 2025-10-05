using UnityEngine;
using System.Collections;

public class Act2Presenter : GamePresenterBase
{
    [Header("Configuración Acto 2")]
    [TextArea(2, 4)]
    public string textoLiricaInicialAct2 = "…y así pasan los días, de lunes a viernes.";
    [TextArea(2, 4)]
    public string textoContextoInicial = "...y así pasan los días, de lunes a viernes...";
    [TextArea(2, 4)]
    public string textoPistaPrincipal = "El tren de siempre. Hora de subir.";
    [SerializeField] private float retrasoParaPista = 4f;

    [Header("Configuración de Audio")]
    [SerializeField] private string sonidoAmbienteEstacion = "StationAmbience";

    private string textoPistaActual;

    protected override void Awake()
    {
        base.Awake();
        if (playerInput != null) playerInput.DeactivateInput();
    }

    protected override void Start()
    {
        // Música/ambiente: empezar cuanto antes
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(sonidoAmbienteEstacion))
        {
            SoundManager.Instance.PlayAmbience(sonidoAmbienteEstacion, 0.6f, true);
        }

        StartCoroutine(RutinaDeInicioActo2());
    }

    private IEnumerator RutinaDeInicioActo2()
    {
        // 1) Si vienes de una transición con pantalla negra, haz un fade-in corto desde negro
        //    (esto usa el panelFadeTransicion, no el panelInicial de la lírica)
        view.IniciarFundidoDeEntrada(0.5f);
        yield return new WaitForSeconds(0.5f);

        // 2) Mostrar la lírica centrada como en Acto 1 y esperar a que termine
        //    Duraciones de ejemplo: fade-in 1.5s, visible 2.0s, fade-out 1.0s
        if (view != null && view.panelInicial != null && !string.IsNullOrEmpty(textoLiricaInicialAct2))
        {
            view.SecuenciaInicial(textoLiricaInicialAct2, 1.5f, 2.0f, 1.0f);
            yield return new WaitForSeconds(1.5f + 2.0f + 1.0f);
            // En OnComplete, SecuenciaInicial desactiva panelInicial automáticamente
        }

        // 3) Ahora sí, activar control y actualizar barra
        if (playerInput != null) playerInput.ActivateInput();
        view.ActualizarBarraCoraje(playerData.Coraje);

        // 4) Pista con barrido: primero contexto, luego objetivo
        textoPistaActual = textoContextoInicial;
        view.MostrarPista(textoPistaActual);

        yield return new WaitForSeconds(retrasoParaPista);

        textoPistaActual = textoPistaPrincipal;
        view.MostrarPista(textoPistaActual);
    }

    public override void OnGazeExitInteractable(InteractableObject objeto)
    {
        base.OnGazeExitInteractable(objeto);
        view.MostrarPista(textoPistaActual);
    }
    public override bool TryCloseUIPanel()
    {
        return false;
    }
}
