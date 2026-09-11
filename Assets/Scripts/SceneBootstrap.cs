using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneBootstrap : MonoBehaviour
{
    bool started;
    void Awake(){ Application.targetFrameRate=60; QualitySettings.vSyncCount=0; BuildStartScreen(); }

    void BuildStartScreen()
    {
        var c=new GameObject("Start Screen Canvas"); var canvas=c.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        var scaler=c.AddComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080); c.AddComponent<GraphicRaycaster>();
        if(FindObjectOfType<EventSystem>()==null){var e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();}
        var bg=Panel(canvas.transform,new Color(.015f,.02f,.03f,1));bg.rectTransform.anchorMin=Vector2.zero;bg.rectTransform.anchorMax=Vector2.one;bg.rectTransform.offsetMin=Vector2.zero;bg.rectTransform.offsetMax=Vector2.zero;
        Label(canvas.transform,"SHOTGUN 3D",new Vector2(.5f,.68f),86,FontStyle.Bold);
        Label(canvas.transform,"OPEN CITY TEST MAP",new Vector2(.5f,.59f),30,FontStyle.Normal);
        var b=Button(canvas.transform,"ENTER CITY",new Vector2(.5f,.43f),400,110);b.onClick.AddListener(StartGame);
        Label(canvas.transform,"Mobile: left joystick + right look + FIRE",new Vector2(.5f,.27f),25,FontStyle.Normal);
    }

    void StartGame()
    {
        if(started)return; started=true;
        var menu=GameObject.Find("Start Screen Canvas");if(menu)Destroy(menu);
        if(FindObjectOfType<CityTestMap>()==null){var map=new GameObject("CITY TEST MAP");map.AddComponent<CityTestMap>().Build();}
        BuildHUD();
    }

    void BuildHUD()
    {
        var c=new GameObject("HUD Canvas");var canvas=c.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scaler=c.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);c.AddComponent<GraphicRaycaster>();
        Label(canvas.transform,"CITY TEST",new Vector2(.5f,.96f),28,FontStyle.Bold);
        Label(canvas.transform,"HP 100",new Vector2(.08f,.94f),30,FontStyle.Bold);
        Label(canvas.transform,"AMMO 6/6",new Vector2(.9f,.94f),30,FontStyle.Bold);
        Label(canvas.transform,"+",new Vector2(.5f,.5f),38,FontStyle.Bold);
        var mobile=MobileHUD.Build(canvas);var game=FindObjectOfType<ShotgunGame>();if(game){game.mobileHUD=mobile;var cam=game.playerCamera;if(cam==null){cam=new GameObject("Third Person Camera").AddComponent<Camera>();cam.tag="MainCamera";game.playerCamera=cam;game.thirdPerson=true;}game.ammoText=FindText(canvas.transform,"AMMO 6/6");game.healthText=FindText(canvas.transform,"HP 100");}
    }

    Text FindText(Transform p,string s){foreach(var t in p.GetComponentsInChildren<Text>(true))if(t.text==s)return t;return null;}
    Text Label(Transform p,string value,Vector2 anchor,int size,FontStyle style){var o=new GameObject("UI "+value);o.transform.SetParent(p,false);var r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.sizeDelta=new Vector2(900,100);var t=o.AddComponent<Text>();t.text=value;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=size;t.fontStyle=style;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return t;}
    Image Panel(Transform p,Color color){var o=new GameObject("Background");o.transform.SetParent(p,false);var i=o.AddComponent<Image>();i.color=color;return i;}
    Button Button(Transform p,string label,Vector2 anchor,float w,float h){var o=new GameObject("Button "+label);o.transform.SetParent(p,false);var r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.sizeDelta=new Vector2(w,h);var i=o.AddComponent<Image>();i.color=new Color(.1f,.15f,.2f,1);var b=o.AddComponent<Button>();Label(o.transform,label,new Vector2(.5f,.5f),40,FontStyle.Bold);return b;}
}
