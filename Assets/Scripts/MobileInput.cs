using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum Mode { Move, Look }
    public Mode mode;
    public RectTransform handle;
    public float radius = 70f;
    public Vector2 Value { get; private set; }

    public void OnPointerDown(PointerEventData eventData) => UpdateValue(eventData);
    public void OnDrag(PointerEventData eventData) => UpdateValue(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector2.zero;
        if (handle) handle.anchoredPosition = Vector2.zero;
    }

    void UpdateValue(PointerEventData eventData)
    {
        RectTransform rect = transform as RectTransform;
        if (!rect) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out Vector2 local);
        Value = Vector2.ClampMagnitude(local / radius, 1f);
        if (handle) handle.anchoredPosition = Value * radius;
    }
}

public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Action { Fire, Reload }
    public Action action;
    public bool Held { get; private set; }

    public void OnPointerDown(PointerEventData eventData) => Held = true;
    public void OnPointerUp(PointerEventData eventData) => Held = false;
}
