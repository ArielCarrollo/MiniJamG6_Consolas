using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [Header("Referencias")]
    public UIView view;
    public Act4Presenter act4Presenter;
    public Camera mainCamera;
    public NPC npcChico;

    [Header("Configuración de Diálogo")]
    public string nombreDelChico = "...Daniel...";
    [TextArea(3, 5)]
    public string pensamientoPanicoDialogo = "(¿Qué he hecho? Debes pensar que soy una tonta... Me quiero morir...)";
    [TextArea(3, 5)]
    public string[] lineasDialogoChico;
    public string sfxSonidoCalido = "WarmSound";
    [Range(0f, 1f)] public float volumenSonidoCalido = 0.8f;

    [Header("Tiempos del Diálogo (segundos)")]
    public float tiempoNombreEnPantalla = 1.5f;
    public float tiempoPensamientoPanico = 3.0f;
    public float pausaAntesDeRespuesta = 1.0f;

    [Header("Configuración de Vibración")]
    public Vector3 vibraAlHablar = new Vector3(0.5f, 0.5f, 0.1f);
    public Vector3 vibraAlRecibirRespuesta = new Vector3(0.3f, 0.3f, 0.3f);

    [Header("Textos Post-Diálogo")]
    public string textoPistaFinal = "Sube al tren junto a Daniel.";
    [TextArea(3, 5)] public string textoPensamientoFinal = "No puedo creer que esto esté pasando...";

    private bool esperandoInput = false;

    private void OnEnable() => InputReader.OnInteract += Evento_Interactuar;
    private void OnDisable() => InputReader.OnInteract -= Evento_Interactuar;

    private void Evento_Interactuar()
    {
        if (esperandoInput)
        {
            esperandoInput = false;
        }
    }

    public void IniciarDialogo()
    {
        StartCoroutine(RutinaDeDialogo());
    }

    private IEnumerator RutinaDeDialogo()
    {
        // 1. Prompt inicial
        view?.MostrarPrompt("[A] Pronunciar su nombre");
        yield return EsperarInput();

        // 2. El Tartamudeo y el Pánico
        view?.OcultarPrompt();
        view?.MostrarSubtitulo(nombreDelChico);
        VibrationManager.Vibrate(vibraAlHablar.x, vibraAlHablar.y, vibraAlHablar.z);
        mainCamera?.transform.DOShakePosition(0.3f, 0.05f);
        yield return new WaitForSeconds(tiempoNombreEnPantalla);

        view?.OcultarSubtitulo();
        view?.MostrarPensamiento(pensamientoPanicoDialogo, 1f);
        yield return new WaitForSeconds(tiempoPensamientoPanico);
        view?.OcultarPensamiento();

        // 3. La Respuesta que lo Cambia Todo
        SoundManager.Instance?.PlaySFX(sfxSonidoCalido, volumenSonidoCalido);
        VibrationManager.Vibrate(vibraAlRecibirRespuesta.x, vibraAlRecibirRespuesta.y, vibraAlRecibirRespuesta.z);
        yield return new WaitForSeconds(pausaAntesDeRespuesta);

        // 4. Diálogo Secuencial de Él
        for (int i = 0; i < lineasDialogoChico.Length; i++)
        {
            view?.MostrarSubtitulo(lineasDialogoChico[i]);
            view?.MostrarPrompt("[A] Escuchar");
            yield return EsperarInput();
            view?.OcultarPrompt();
            view?.OcultarSubtitulo();
            yield return new WaitForSeconds(0.2f);
        }

        // 5. Iniciar la fase final de seguimiento
        act4Presenter?.FinalizarDialogoYActivarSeguimiento(npcChico, textoPistaFinal, textoPensamientoFinal);
    }

    private IEnumerator EsperarInput()
    {
        esperandoInput = true;
        yield return new WaitUntil(() => !esperandoInput);
    }
}