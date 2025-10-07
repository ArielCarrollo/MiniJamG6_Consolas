using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInput playerInput;

    [SerializeField] private float sensitivity = 200f;
    private Vector2 rotation;
    private Vector2 delta;

    // NUEVO: Flag para controlar la rotación.
    private bool rotacionPermitida = true;

    private void OnEnable() => InputReader.OnDelta += MoveCamera;
    private void OnDisable() => InputReader.OnDelta -= MoveCamera;

    private void Update()
    {
        if (playerInput != null && !playerInput.actions.enabled)
        {
            return;
        }

        // NUEVO: Si la rotación no está permitida, salimos del método aquí.
        if (!rotacionPermitida)
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

    // NUEVO: Método público para que otros scripts puedan bloquear/desbloquear la rotación.
    public void PermitirRotacion(bool permitido)
    {
        rotacionPermitida = permitido;
    }
}