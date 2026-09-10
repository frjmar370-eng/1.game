using UnityEngine;
using UnityEngine.UI;

public class CombatPresentation3D : MonoBehaviour
{
    void Start(){foreach(TargetDummy d in FindObjectsOfType<TargetDummy>())Attach(d);}
    public void Attach(TargetDummy d){if(!d||d.GetComponent<WorldHealthBar3D>())return;d.gameObject.AddComponent<WorldHealthBar3D>();}
}

public class WorldHealthBar3D : MonoBehaviour
{
    TargetDummy target; Canvas canvas; Image fill; Camera cam; RectTransform fillRect;
    void Awake(){
        target=GetComponent<TargetDummy>();cam=Camera.main;
        GameObject c=new GameObject("HealthBar");c.transform.SetParent(transform,false);c.transform.localPosition=Vector3.up*2.25f;canvas=c.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=cam;canvas.sortingOrder=20;
        RectTransform rt=c.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(1.2f,.14f);c.transform.localScale=Vector3.one*.012f;
        GameObject bg=new GameObject("BG");bg.transform.SetParent(c.transform,false);RectTransform br=bg.AddComponent<RectTransform>();br.anchorMin=Vector2.zero;br.anchorMax=Vector2.one;br.offsetMin=Vector2.zero;br.offsetMax=Vector2.zero;Image bi=bg.AddComponent<Image>();bi.color=Color.black;
        GameObject fg=new GameObject("Fill");fg.transform.SetParent(c.transform,false);fillRect=fg.AddComponent<RectTransform>();fillRect.anchorMin=new Vector2(0,.1f);fillRect.anchorMax=new Vector2(1,.9f);fillRect.offsetMin=Vector2.zero;fillRect.offsetMax=Vector2.zero;fill=fg.AddComponent<Image>();fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;fill.fillOrigin=0;fill.fillAmount=1f;
    }
    void LateUpdate(){
        if(!target){Destroy(gameObject);return;}if(!cam)cam=Camera.main;
        if(cam){canvas.worldCamera=cam;canvas.transform.rotation=Quaternion.LookRotation(canvas.transform.position-cam.transform.position);}
        float hp=Mathf.Clamp01(target.health/Mathf.Max(1f,target.MaxHealth));fill.fillAmount=hp;canvas.enabled=target.health>0;
    }
}
