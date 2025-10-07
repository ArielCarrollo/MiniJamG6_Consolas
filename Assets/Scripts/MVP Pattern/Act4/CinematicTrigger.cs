using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EventTrigger : MonoBehaviour
{
    [Tooltip("El evento que se disparará cuando el jugador entre en el trigger.")]
    public UnityEvent onPlayerEnter;

    [Tooltip("Marcar si el trigger debe desactivarse después de su primer uso.")]
    public bool desactivarAlUsar = true;

    private void Awake()
    {
        // Aseguramos que el collider sea un trigger.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que ha entrado es el jugador.
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player ha entrado en el EventTrigger: " + gameObject.name);
            onPlayerEnter?.Invoke();

            if (desactivarAlUsar)
            {
                gameObject.SetActive(false);
            }
        }
    }
}