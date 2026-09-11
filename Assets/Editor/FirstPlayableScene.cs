#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

[InitializeOnLoad]
public static class FirstPlayableScene
{
    const string ScenePath="Assets/Scenes/Main.unity";
    static FirstPlayableScene(){ EditorApplication.delayCall += EnsureScene; }
    [MenuItem("Shotgun 3D/Build First Playable Scene")]
    public static void EnsureScene(){
        if(!Application.isBatchMode && File.Exists(ScenePath)) return;
        if(!AssetExists("Assets/Art/Environment/CityKit.obj") || !AssetExists("Assets/Art/Characters/Hero.obj")) { EditorApplication.delayCall += EnsureScene; return; }
        Directory.CreateDirectory("Assets/Scenes");
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor=new Color(.25f,.30f,.38f);
        RenderSettings.ambientEquatorColor=new Color(.12f,.15f,.18f);
        RenderSettings.ambientGroundColor=new Color(.045f,.05f,.055f);
        RenderSettings.fog=true; RenderSettings.fogColor=new Color(.035f,.05f,.075f); RenderSettings.fogDensity=.0025f;
        var cityPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Environment/CityKit.obj");
        var city=Object.Instantiate(cityPrefab); city.name="CITY_REAL_3D"; city.AddComponent<MeshCollider>();
        var heroPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Characters/Hero.obj");
        var hero=Object.Instantiate(heroPrefab); hero.name="PLAYER_3D"; hero.transform.position=new Vector3(0,1f,-70); hero.transform.localScale=Vector3.one;
        var cc=hero.AddComponent<CharacterController>(); cc.height=3.8f; cc.radius=.55f; cc.center=new Vector3(0,1.9f,0); cc.stepOffset=.45f; cc.slopeLimit=48f;
        var controller=hero.AddComponent<ThirdPersonController>();
        var target=new GameObject("CameraTarget"); target.transform.SetParent(hero.transform); target.transform.localPosition=new Vector3(0,1.8f,0); controller.cameraTarget=target.transform;
        var camObj=new GameObject("Main Camera"); var cam=camObj.AddComponent<Camera>(); cam.tag="MainCamera"; cam.fieldOfView=62; cam.nearClipPlane=.1f; cam.farClipPlane=340; cam.allowHDR=true;
        var follow=camObj.AddComponent<ThirdPersonCamera>(); follow.target=hero.transform; camObj.transform.position=hero.transform.position+new Vector3(0,4,-7);
        var light=new GameObject("Sun"); var dl=light.AddComponent<Light>(); dl.type=LightType.Directional; dl.intensity=1.25f; dl.shadows=LightShadows.Soft; dl.shadowStrength=.85f; light.transform.rotation=Quaternion.Euler(48,-32,0);
        CreateHUD();
        EditorSceneManager.SaveScene(scene,ScenePath); AssetDatabase.SaveAssets();
        Debug.Log("SHOTGUN 3D: enhanced first playable city scene created.");
        if(Application.isBatchMode) EditorApplication.Exit(0);
    }
    static bool AssetExists(string p)=>File.Exists(p);
    static void CreateHUD(){
        var es=new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>();
        var c=new GameObject("HUD"); var canvas=c.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay; canvas.sortingOrder=20; var scaler=c.AddComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080); c.AddComponent<GraphicRaycaster>();
        Text("SHOTGUN 3D",new Vector2(55,-38),new Vector2(400,70),44,FontStyle.Bold);
        var mission=Text("MISSION 01  •  دخول المنطقة",new Vector2(55,-95),new Vector2(560,55),24,FontStyle.Bold); mission.color=new Color(.35f,.85f,1);
        Text health=Text("HP  ██████████  100",new Vector2(-300,-42),new Vector2(500,60),28,FontStyle.Bold); health.alignment=TextAnchor.MiddleRight;
        Text ammo=Text("12 / 48",new Vector2(-70,55),new Vector2(260,70),36,FontStyle.Bold); ammo.alignment=TextAnchor.MiddleRight;
        Panel(new Vector2(190,180),new Vector2(300,300),new Color(1,1,1,.08f),"MoveStick").AddComponent<MobileStick>();
        var look=Panel(new Vector2(-190,180),new Vector2(330,300),new Color(1,1,1,.05f),"LookStick"); look.AddComponent<MobileStick>().lookStick=true;
        var fire=Panel(new Vector2(-90,115),new Vector2(150,150),new Color(1,.18f,.12f,.75f),"FIRE"); fire.AddComponent<MobileFireButton>(); Text("FIRE",new Vector2(-90,115),new Vector2(150,150),25,FontStyle.Bold).alignment=TextAnchor.MiddleCenter;
        var jump=Panel(new Vector2(-270,115),new Vector2(120,90),new Color(.15f,.55f,1,.72f),"JUMP"); jump.AddComponent<MobileJumpButton>(); Text("JUMP",new Vector2(-270,115),new Vector2(120,90),19,FontStyle.Bold).alignment=TextAnchor.MiddleCenter;
        Text("RELOAD",new Vector2(-270,255),new Vector2(180,65),20,FontStyle.Bold).alignment=TextAnchor.MiddleCenter;
    }
    static Text Text(string s,Vector2 pos,Vector2 size,int font,FontStyle style){var go=new GameObject("UI_Text");go.transform.SetParent(GameObject.Find("HUD").transform,false);var t=go.AddComponent<Text>();t.text=s;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=font;t.fontStyle=style;t.color=Color.white;t.raycastTarget=false;var r=t.rectTransform;r.anchorMin=r.anchorMax=new Vector2(pos.x>=0?0:1,pos.y>=0?0:1);r.pivot=new Vector2(pos.x>=0?0:1,pos.y>=0?0:1);r.anchoredPosition=new Vector2(pos.x>=0?pos.x:-pos.x,pos.y>=0?-pos.y:pos.y);r.sizeDelta=size;return t;}
    static Image Panel(Vector2 pos,Vector2 size,Color color,string name){var go=new GameObject(name);go.transform.SetParent(GameObject.Find("HUD").transform,false);var i=go.AddComponent<Image>();i.color=color;i.raycastTarget=true;var r=i.rectTransform;r.anchorMin=r.anchorMax=new Vector2(pos.x>=0?0:1,pos.y>=0?0:1);r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=new Vector2(pos.x>=0?pos.x:-pos.x,pos.y>=0?-pos.y:pos.y);r.sizeDelta=size;return i;}
}
#endif
