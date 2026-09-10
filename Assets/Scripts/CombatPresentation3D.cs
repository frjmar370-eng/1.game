using UnityEngine;
using UnityEngine.UI;

public class CombatPresentation3D : MonoBehaviour
{
    void Start(){
        foreach(TargetDummy d in FindObjectsOfType<TargetDummy>())Attach(d);
    }
    public void Attach(TargetDummy d){
        if(!d||d.GetComponent<WorldHealthBar3D>())return;
        d.gameObject.AddComponent<WorldHealthBar3D>();
    }
}

public class WorldHealthBar3D : MonoBehaviour
{
    TargetDummy target;
    Canvas canvas; Image fill;
    Camera cam;
    void Awake(){
        target=GetComponent<TargetDummy>();
        cam=Camera.main;
        GameObject c=new GameObject("HealthBar");c.transform.SetParent(transform,false);c.transform.localPosition=Vector3.up*2.25f;
        canvas=c.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=cam;canvas.sortingOrder=20;
        RectTransform rt=c.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(1.2f,.14f);c.transform.localScale=Vector3.one*.012f;
        GameObject bg=GameObject.CreatePrimitive(PrimitiveType.Quad);bg.name="BG";bg.transform.SetParent(c.transform,false);bg.transform.localScale=Vector3.one;Destroy(bg.GetComponent<Collider>());
        Image b=bg.AddComponent<Image>();b.color=Color.black;
        GameObject fg=new GameObject("Fill");fg.transform.SetParent(c.transform,false);RectTransform fr=fg.AddComponent<RectTransform>();fr.anchorMin=new Vector2(0,.1f);fr.anchorMax=new Vector2(1,.9f);fr.offsetMin=Vector2.zero;fr.offsetMax=Vector2.zero;fill=fg.AddComponent<Image>();
    }
    void LateUpdate(){
        if(!target){Destroy(gameObject);return;}
        if(!cam)cam=Camera.main;
        if(cam)canvas.transform.rotation=Quaternion.LookRotation(canvas.transform.position-cam.transform.position);
        float hp=Mathf.Clamp01(target.health/Mathf.Max(1f,10f));
        fill.fillAmount=hp;
        canvas.enabled=target.health>0;
    }
}
