using UnityEngine;

abstract public class InteractiveObject : MonoBehaviour
{
    protected bool input;
    protected abstract void Interactive();
    public void Input(bool value)
    {
        if (value)
        {
            InputReader.OnInteract += Interactive;
        }
        else
        {
            InputReader.OnInteract -= Interactive;
        }
    }
}