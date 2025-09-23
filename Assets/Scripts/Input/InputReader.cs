using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public static event Action<Vector2> OnMove;
    public static event Action<Vector2> OnDelta;
    public static event Action OnInteract;
    public void InputPlayerMove(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>().normalized);
    }
    public void InputMouseDelta(InputAction.CallbackContext context)
    {
        OnDelta?.Invoke(context.ReadValue<Vector2>().normalized);
    }
    public void InputInteract(InputAction.CallbackContext context)
    {
        if(context.performed) 
            OnInteract?.Invoke();
    }
}
