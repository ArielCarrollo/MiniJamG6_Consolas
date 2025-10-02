using UnityEngine;
using UnityEngine.InputSystem;

public static class VibrationManager
{
    private static Coroutine stopVibrationCoroutine;

    // M�todo principal para llamar a la vibraci�n
    public static void Vibrate(float lowFrequency, float highFrequency, float duration)
    {
        // Comprueba si hay un gamepad conectado
        if (Gamepad.current == null) return;

        // Inicia la vibraci�n
        Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);

        // Detiene la vibraci�n despu�s de la duraci�n especificada
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

    // Peque�a clase auxiliar para poder ejecutar corrutinas desde una clase est�tica
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