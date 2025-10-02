using UnityEngine;
using UnityEngine.InputSystem;

public static class VibrationManager
{
    private static Coroutine stopVibrationCoroutine;

    // Método principal para llamar a la vibración
    public static void Vibrate(float lowFrequency, float highFrequency, float duration)
    {
        // Comprueba si hay un gamepad conectado
        if (Gamepad.current == null) return;

        // Inicia la vibración
        Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);

        // Detiene la vibración después de la duración especificada
        // Esto se hace en un MonoBehaviour temporal para poder usar corrutinas
        if (stopVibrationCoroutine != null)
        {
            MonoBehaviourDummy.Instance.StopCoroutine(stopVibrationCoroutine);
        }
        stopVibrationCoroutine = MonoBehaviourDummy.Instance.StartCoroutine(StopVibration(duration));
    }

    private static System.Collections.IEnumerator StopVibration(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    // Pequeña clase auxiliar para poder ejecutar corrutinas desde una clase estática
    private class MonoBehaviourDummy : MonoBehaviour
    {
        public static MonoBehaviourDummy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("VibrationManagerDummy").AddComponent<MonoBehaviourDummy>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static MonoBehaviourDummy _instance;
    }
}