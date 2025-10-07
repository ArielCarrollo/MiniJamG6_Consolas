using UnityEngine;
using System.Collections;
using DG.Tweening; // Asegúrate de tener DOTween en tu proyecto.

public class Act4CameraController : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Act4CameraController necesita estar en el mismo objeto que el componente Camera.");
        }
    }

    /// <summary>
    /// Inicia un zoom suave hacia un punto objetivo.
    /// </summary>
    /// <param name="objetivo">El transform hacia el que se hará el zoom.</param>
    /// <param name="fovFinal">El campo de visión final (más bajo es más zoom).</param>
    /// <param name="duracion">Cuánto tiempo durará el efecto de zoom.</param>
    public void IniciarZoomHaciaObjetivo(Transform objetivo, float fovFinal, float duracion)
    {
        if (cam == null || objetivo == null) return;

        // Rotar la cámara para que mire directamente al objetivo.
        Quaternion rotacionObjetivo = Quaternion.LookRotation(objetivo.position - transform.position);
        transform.DORotateQuaternion(rotacionObjetivo, duracion / 2); // Rotamos en la mitad del tiempo del zoom.

        // Animar el Field of View para crear el efecto de zoom.
        cam.DOFieldOfView(fovFinal, duracion).SetEase(Ease.OutCubic);
    }
}