using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("El nombre exacto del archivo de la escena que se va a cargar.")]
    public string nombreDeLaEscenaACargar;

    [Tooltip("La etiqueta del objeto que debe activar el trigger (normalmente 'Player').")]
    public string etiquetaDelActivador = "Player";

    // Un seguro para evitar que el trigger se active múltiples veces
    private bool transicionIniciada = false;

    private void OnTriggerEnter(Collider other)
    {
        // Si la transición ya comenzó, no hacemos nada.
        if (transicionIniciada) return;

        if (other.CompareTag(etiquetaDelActivador))
        {
            if (!string.IsNullOrEmpty(nombreDeLaEscenaACargar))
            {
                transicionIniciada = true; // Activamos el seguro

                // --- MODIFICADO: Le pedimos al GamePresenter que maneje la transición ---
                GamePresenter.Instance.IniciarTransicionAEscena(nombreDeLaEscenaACargar);
            }
            else
            {
                Debug.LogWarning("El nombre de la escena a cargar no está definido.", this.gameObject);
            }
        }
    }
}