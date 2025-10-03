using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Configuraci�n de M�sica")]
    [Tooltip("El nombre del sonido a reproducir (debe coincidir con uno de la lista del SoundManager).")]
    public string musicName;

    [Tooltip("El volumen al que sonar� la m�sica en esta escena.")]
    [Range(0f, 1f)]
    public float volume = 0.8f; // Un valor por defecto un poco m�s bajo que el m�ximo

    [Tooltip("Marcar si la m�sica debe repetirse en bucle.")]
    public bool loop = true;

    void Start()
    {
        // Al iniciar la escena, le pide al SoundManager que ponga la m�sica con el volumen especificado.
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(musicName))
        {
            SoundManager.Instance.PlayMusic(musicName, volume, loop);
        }
    }
}