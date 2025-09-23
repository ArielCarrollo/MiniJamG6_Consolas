using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private Transform mainCamera;
    private CharacterController controller;
    private Vector3 velocity;

    private Vector2 move;
    private void OnEnable()
    {
        InputReader.OnMove += Move;
    }
    private void OnDisable()
    {
        InputReader.OnMove -= Move;
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
    }
    private void Update()
    {
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