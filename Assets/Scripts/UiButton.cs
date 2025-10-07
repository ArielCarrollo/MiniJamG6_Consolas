using UnityEngine;
using UnityEngine.EventSystems; 

public class UiButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Vector3 originalScale;
    public float scaleFactor = 1.2f; 

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = originalScale;
    }
}
