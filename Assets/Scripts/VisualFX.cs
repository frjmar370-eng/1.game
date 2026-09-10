using UnityEngine;
using UnityEngine.UI;

public class VisualFX : MonoBehaviour
{
    public static VisualFX Instance { get; private set; }
    Image damageOverlay; Text hitMarker; float damageAlpha; float hitTime;
    void Awake(){Instance=this;BuildOverlay();}
    void Update(){if(damageOverlay){damageAlpha=Mathf.MoveTowards(damageAlpha,0f,Time.deltaTime*1.8f);Color c=damageOverlay.color;c.a=damageAlpha;damageOverlay.color=c;}if(hitMarker&&Time.time>hitTime)hitMarker.enabled=false;}
    void BuildOverlay(){GameObject c=new GameObject("Combat FX Canvas");Canvas canvas=c.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=100;CanvasScaler s=c.AddComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);c.AddComponent<GraphicRaycaster>();GameObject d=new GameObject("Damage Vignette");d.transform.SetParent(c.transform,false);RectTransform dr=d.AddComponent<RectTransform>();dr.anchorMin=Vector2.zero;dr.anchorMax=Vector2.one;dr.offsetMin=dr.offsetMax=Vector2.zero;damageOverlay=d.AddComponent<Image>();damageOverlay.color=new Color(1f,.02f,.02f,0f);damageOverlay.raycastTarget=false;GameObject h=new GameObject("Hit Marker");h.transform.SetParent(c.transform,false);RectTransform hr=h.AddComponent<RectTransform>();hr.anchorMin=hr.anchorMax=new Vector2(.5f,.5f);hr.sizeDelta=new Vector2(90,90);hitMarker=h.AddComponent<Text>();hitMarker.text="×";hitMarker.font=Resources.GetBuiltinResource<Font>("Arial.ttf");hitMarker.fontSize=58;hitMarker.alignment=TextAnchor.MiddleCenter;hitMarker.color=Color.white;hitMarker.enabled=false;hitMarker.raycastTarget=false;}
    public void PlayerDamaged(){damageAlpha=Mathf.Max(damageAlpha,.58f);if(CombatFeedback.Instance)CombatFeedback.Instance.Shake(.11f);}
    public void Hit(){if(hitMarker){hitMarker.enabled=true;hitTime=Time.time+.09f;}}
    public void Muzzle(Vector3 p){Burst(p,new Color(1f,.65f,.12f),.16f,8,.06f);}
    public void Impact(Vector3 p){Burst(p,Color.white,.11f,6,.05f);}
    public void Death(Vector3 p){Burst(p,new Color(1f,.25f,.05f),.28f,16,.12f);}
    public void Pickup(Vector3 p){Burst(p,new Color(.2f,1f,.55f),.2f,10,.1f);}
    void Burst(Vector3 p,Color color,float size,int count,float lifetime){GameObject go=new GameObject("FX Burst");go.transform.position=p;ParticleSystem ps=go.AddComponent<ParticleSystem>();var main=ps.main;main.startLifetime=lifetime;main.startSpeed=Random.Range(2f,4f);main.startSize=size;main.startColor=color;main.simulationSpace=ParticleSystemSimulationSpace.World;var em=ps.emission;em.enabled=false;ps.Emit(count);var r=ps.GetComponent<ParticleSystemRenderer>();Shader sh=Shader.Find("Legacy Shaders/Particles/Alpha Blended");if(sh==null)sh=Shader.Find("Particles/Standard Unlit");if(sh)r.material=new Material(sh);Destroy(go,lifetime+.2f);}
}
