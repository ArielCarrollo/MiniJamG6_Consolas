using UnityEngine;

// [System.Serializable] permite que podamos ver y editar esta clase en el Inspector de Unity.
[System.Serializable]
public class Sound
{
    public string name; // El nombre con el que llamaremos al sonido (ej: "ButtonClick", "Act1Music")
    public AudioClip clip; // El archivo de audio
}