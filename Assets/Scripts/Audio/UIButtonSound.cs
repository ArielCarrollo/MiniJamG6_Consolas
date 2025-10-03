using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Header("Configuración de Sonido")]
    [Tooltip("El nombre del SFX a reproducir (del SoundManager).")]
    public string clickSoundName = "ButtonClick";

    [Tooltip("El volumen del sonido del clic.")]
    [Range(0f, 1f)]
    public float volume = 1.0f; // NUEVO: Campo para el volumen

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(PlayClickSound);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (SoundManager.Instance != null)
        {
            // Pasamos el volumen al SoundManager
            SoundManager.Instance.PlaySFX(clickSoundName, volume);
        }
    }
}