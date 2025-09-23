using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 200f;
    private Vector2 rotation;
    private Vector2 delta;


    private void OnEnable()
    {
        InputReader.OnDelta += MoveCamera;
    }
    private void OnDisable()
    {
        InputReader.OnDelta -= MoveCamera;
    }
    private void Update()
    {
        rotation += delta * sensitivity * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
        transform.rotation = Quaternion.Euler(-rotation.y, rotation.x, 0f);
    }
    private void MoveCamera(Vector2 value)
    {
        delta = value;
    }
}
