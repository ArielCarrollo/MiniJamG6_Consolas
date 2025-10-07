using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInput playerInput;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private Transform mainCamera;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 move;

    // NUEVO: Flag para controlar el movimiento lateral.
    private bool movimientoLateralPermitido = true;

    private void OnEnable() => InputReader.OnMove += Move;
    private void OnDisable() => InputReader.OnMove -= Move;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
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
        // CORRECCIÓN: Usamos el flag para decidir si aplicar el movimiento en X.
        float inputHorizontal = movimientoLateralPermitido ? move.x : 0f;

        Vector3 direction = (mainCamera.right * inputHorizontal + mainCamera.forward * move.y).normalized;
        direction.y = 0;
        controller.Move(direction * moveSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // NUEVO: Método público para que otros scripts puedan restringir el movimiento.
    public void PermitirMovimientoLateral(bool permitido)
    {
        movimientoLateralPermitido = permitido;
    }
}