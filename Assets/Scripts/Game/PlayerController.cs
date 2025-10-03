using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInput playerInput; // NUEVO: Arrastra aquí el componente PlayerInput

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private Transform mainCamera;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 move;

    // El booleano 'isFrozen' ya no es necesario

    private void OnEnable() => InputReader.OnMove += Move;
    private void OnDisable() => InputReader.OnMove -= Move;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        // --- SOLUCIÓN AL MOVIMIENTO ---
        // Si el PlayerInput está desactivado, no hacemos absolutamente nada.
        if (playerInput != null && !playerInput.actions.enabled)
        {
            return;
        }

        MovePlayer();
    }

    private void Move(Vector2 value)
    {
        move = value;
    }

    private void MovePlayer()
    {
        Vector3 direction = (mainCamera.right * move.x + mainCamera.forward * move.y).normalized;
        direction.y = 0;
        controller.Move(direction * moveSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}