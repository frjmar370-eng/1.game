using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool lookStick;
    RectTransform rt; Vector2 start; int finger=-1;
    void Awake(){rt=GetComponent<RectTransform>();}
    public void OnPointerDown(PointerEventData e){finger=e.pointerId; RectTransformUtility.ScreenPointToLocalPointInRectangle(rt,e.position,e.pressEventCamera,out start); UpdateValue(e);}
    public void OnDrag(PointerEventData e){if(e.pointerId==finger)UpdateValue(e);}
    public void OnPointerUp(PointerEventData e){if(e.pointerId!=finger)return; if(lookStick)MobileControls.Look=Vector2.zero;else MobileControls.Move=Vector2.zero;finger=-1;}
    void UpdateValue(PointerEventData e){Vector2 p;RectTransformUtility.ScreenPointToLocalPointInRectangle(rt,e.position,e.pressEventCamera,out p); Vector2 v=Vector2.ClampMagnitude((p-start)/(Mathf.Max(1,rt.rect.width*.32f)),1); if(lookStick)MobileControls.Look=v;else MobileControls.Move=v;}
}

public class MobileFireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
 public void OnPointerDown(PointerEventData e){MobileControls.Fire=true;}
 public void OnPointerUp(PointerEventData e){MobileControls.Fire=false;}
}
