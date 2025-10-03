using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Librería de Sonidos")]
    public List<Sound> sounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Reproduce una pista de música con un volumen específico.
    /// </summary>
    public void PlayMusic(string name, float volume = 1.0f, bool loop = true, float fadeDuration = 1.5f)
    {
        Sound s = sounds.FirstOrDefault(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sonido: '{name}' no encontrado.");
            return;
        }

        if (musicSource.clip == s.clip && musicSource.isPlaying) return;

        musicSource.DOKill();

        if (musicSource.isPlaying)
        {
            musicSource.DOFade(0, fadeDuration / 2).OnComplete(() =>
            {
                StartNewMusic(s, volume, loop, fadeDuration / 2);
            });
        }
        else
        {
            StartNewMusic(s, volume, loop, fadeDuration);
        }
    }

    private void StartNewMusic(Sound s, float volume, bool loop, float fadeDuration)
    {
        musicSource.clip = s.clip;
        musicSource.loop = loop;
        musicSource.Play();
        musicSource.DOFade(volume, fadeDuration);
    }

    /// <summary>
    /// Reproduce un efecto de sonido con un volumen específico.
    /// </summary>
    public void PlaySFX(string name, float volume = 1.0f)
    {
        Sound s = sounds.FirstOrDefault(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sonido: '{name}' no encontrado.");
            return;
        }
        // Usamos la sobrecarga de PlayOneShot que acepta un volumen.
        sfxSource.PlayOneShot(s.clip, volume);
    }
}