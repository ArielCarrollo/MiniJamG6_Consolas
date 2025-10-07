using UnityEngine;
using DG.Tweening;

public class ChicoLowPolyController : MonoBehaviour
{
    [Header("Referencias del Esqueleto")]
    [SerializeField] private Transform cabeza;
    [SerializeField] private Transform cuello;
    [SerializeField] private Transform torso;

    [Header("Rotaciones Manuales")]
    [SerializeField] private Vector3 rotacionCabezaMirar = new Vector3(0, 45f, 0); // Ajusta estos valores manualmente
    [SerializeField] private Vector3 rotacionCuelloMirar = new Vector3(0, 15f, 0); // Ajusta estos valores manualmente
    [SerializeField] private float velocidadGiro = 1f;
    [SerializeField] private float tiempoMirando = 2.5f;

    private Vector3 rotacionCabezaOriginal;
    private Vector3 rotacionCuelloOriginal;
    private bool mirandoJugador = false;

    private void Start()
    {
        // Guardar rotaciones originales
        if (cabeza != null) rotacionCabezaOriginal = cabeza.localEulerAngles;
        if (cuello != null) rotacionCuelloOriginal = cuello.localEulerAngles;
    }

    public void MirarJugador()
    {
        if (mirandoJugador) return;
        StartCoroutine(SecuenciaMirarJugador());
    }

    private System.Collections.IEnumerator SecuenciaMirarJugador()
    {
        mirandoJugador = true;

        // Usar rotaciones manuales específicas
        if (cabeza != null)
        {
            cabeza.DOLocalRotate(rotacionCabezaOriginal + rotacionCabezaMirar, velocidadGiro)
                  .SetEase(Ease.OutQuad);
        }

        if (cuello != null)
        {
            cuello.DOLocalRotate(rotacionCuelloOriginal + rotacionCuelloMirar, velocidadGiro)
                  .SetEase(Ease.OutQuad);
        }

        yield return new WaitForSeconds(tiempoMirando);
        VolverAPosturaOriginal();
    }

    private void VolverAPosturaOriginal()
    {
        if (cabeza != null)
        {
            cabeza.DOLocalRotate(rotacionCabezaOriginal, velocidadGiro)
                  .SetEase(Ease.InQuad);
        }

        if (cuello != null)
        {
            cuello.DOLocalRotate(rotacionCuelloOriginal, velocidadGiro)
                  .SetEase(Ease.InQuad)
                  .OnComplete(() => mirandoJugador = false);
        }
        else
        {
            mirandoJugador = false;
        }
    }

    public void Bostezo()
    {
        if (mirandoJugador) return;

        cabeza?.DOLocalRotate(rotacionCabezaOriginal + new Vector3(-10f, 0, 0), 0.5f)
              .SetEase(Ease.OutQuad)
              .OnComplete(() => {
                  cabeza.DOLocalRotate(rotacionCabezaOriginal, 0.5f)
                        .SetEase(Ease.InQuad);
              });
    }
}
