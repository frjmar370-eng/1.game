using UnityEngine;
using UnityEngine.EventSystems;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Action { Fire, Reload }

    public Action action;
    public bool Held { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        Held = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Held = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Held = false;
    }

    void OnDisable()
    {
        Held = false;
    }
}
