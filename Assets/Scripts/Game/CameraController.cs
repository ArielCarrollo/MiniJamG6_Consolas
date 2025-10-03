using UnityEngine;
using UnityEngine.InputSystem; // Necesario para PlayerInput

public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInput playerInput; // NUEVO: Arrastra aquí el componente PlayerInput

    [SerializeField] private float sensitivity = 200f;
    private Vector2 rotation;
    private Vector2 delta;

    // El booleano 'isFrozen' ya no es necesario

    private void OnEnable() => InputReader.OnDelta += MoveCamera;
    private void OnDisable() => InputReader.OnDelta -= MoveCamera;

    private void Update()
    {
        // --- SOLUCIÓN AL MOVIMIENTO ---
        // Si el PlayerInput está desactivado, no hacemos absolutamente nada.
        if (playerInput != null && !playerInput.actions.enabled)
        {
            return;
        }

        rotation += delta * sensitivity * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
        transform.rotation = Quaternion.Euler(-rotation.y, rotation.x, 0f);
    }

    private void MoveCamera(Vector2 value)
    {
        delta = value;
    }
}
