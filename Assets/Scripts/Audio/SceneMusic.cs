using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Configuración de Música")]
    [Tooltip("El nombre del sonido a reproducir (debe coincidir con uno de la lista del SoundManager).")]
    public string musicName;

    [Tooltip("El volumen al que sonará la música en esta escena.")]
    [Range(0f, 1f)]
    public float volume = 0.8f; // Un valor por defecto un poco más bajo que el máximo

    [Tooltip("Marcar si la música debe repetirse en bucle.")]
    public bool loop = true;

    void Start()
    {
        // Al iniciar la escena, le pide al SoundManager que ponga la música con el volumen especificado.
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(musicName))
        {
            SoundManager.Instance.PlayMusic(musicName, volume, loop);
        }
    }
}