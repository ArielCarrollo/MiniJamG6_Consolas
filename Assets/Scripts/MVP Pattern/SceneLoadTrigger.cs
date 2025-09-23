using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("El nombre exacto del archivo de la escena que se va a cargar.")]
    public string nombreDeLaEscenaACargar;

    [Tooltip("La etiqueta del objeto que debe activar el trigger (normalmente 'Player').")]
    public string etiquetaDelActivador = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto que entró en el trigger tiene la etiqueta correcta
        if (other.CompareTag(etiquetaDelActivador))
        {
            Debug.Log("'" + etiquetaDelActivador + "' ha entrado en el trigger. Cargando escena: " + nombreDeLaEscenaACargar);

            // Asegurarse de que el nombre de la escena no esté vacío
            if (!string.IsNullOrEmpty(nombreDeLaEscenaACargar))
            {
                SceneManager.LoadScene(nombreDeLaEscenaACargar);
            }
            else
            {
                Debug.LogWarning("El nombre de la escena a cargar no está definido en el SceneLoadTrigger.", this.gameObject);
            }
        }
    }
}