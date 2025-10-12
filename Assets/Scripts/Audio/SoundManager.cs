using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopingSfxSource;

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

    // Música melódica principal (solo una a la vez)
    public void PlayMusic(string name, float volume = 1.0f, bool loop = true, float fadeDuration = 1.5f)
    {
        Sound s = sounds.FirstOrDefault(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sonido musical: '{name}' no encontrado.");
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

    // Ambientes (ej: murmullo, estación, naturaleza), independiente de la música
    public void PlayAmbience(string name, float volume = 1.0f, bool loop = true, float fadeDuration = 1.5f)
    {
        Sound s = sounds.FirstOrDefault(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sonido de ambiente: '{name}' no encontrado.");
            return;
        }

        if (ambienceSource.clip == s.clip && ambienceSource.isPlaying) return;

        ambienceSource.DOKill();

        if (ambienceSource.isPlaying)
        {
            ambienceSource.DOFade(0, fadeDuration / 2).OnComplete(() =>
            {
                StartNewAmbience(s, volume, loop, fadeDuration / 2);
            });
        }
        else
        {
            StartNewAmbience(s, volume, loop, fadeDuration);
        }
    }

    private void StartNewAmbience(Sound s, float volume, bool loop, float fadeDuration)
    {
        ambienceSource.clip = s.clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
        ambienceSource.DOFade(volume, fadeDuration);
    }

    // Efectos puntuales (timbres, acciones, feedback)
    public void PlaySFX(string name, float volume = 1.0f)
    {
        Sound s = sounds.FirstOrDefault(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"SFX: '{name}' no encontrado.");
            return;
        }
        sfxSource.PlayOneShot(s.clip, volume);
    }
    public void PlayLoopingSFX(string name, float volume = 1.0f)
    {
        if (loopingSfxSource == null) return;

        Sound s = sounds.FirstOrDefault(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Looping SFX: '{name}' no encontrado.");
            return;
        }

        loopingSfxSource.clip = s.clip;
        loopingSfxSource.volume = volume;
        loopingSfxSource.loop = true;
        loopingSfxSource.Play();
    }
    public void StopLoopingSFX()
    {
        if (loopingSfxSource == null) return;

        loopingSfxSource.Stop();
        loopingSfxSource.clip = null; // Limpiamos la referencia
    }
    public void SetLoopingSFXVolume(float volume)
    {
        if (loopingSfxSource != null)
        {
            loopingSfxSource.volume = volume;
        }
    }
    // Métodos opcionales para detener/cambiar pistas individualmente
    public void StopMusic(float fadeDuration = 1.0f)
    {
        if (musicSource.isPlaying)
            musicSource.DOFade(0, fadeDuration).OnComplete(() => musicSource.Stop());
    }

    public void StopAmbience(float fadeDuration = 1.0f)
    {
        if (ambienceSource.isPlaying)
            ambienceSource.DOFade(0, fadeDuration).OnComplete(() => ambienceSource.Stop());
    }
}
