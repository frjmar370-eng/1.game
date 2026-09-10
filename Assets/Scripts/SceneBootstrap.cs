using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneBootstrap : MonoBehaviour
{
    bool gameStarted;

    void Awake(){Application.targetFrameRate=60;QualitySettings.vSyncCount=0;BuildStartScreen();}

    void BuildStartScreen(){
        GameObject canvasObj=new GameObject("Start Screen Canvas");
        Canvas canvas=canvasObj.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=canvasObj.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        if(FindObjectOfType<EventSystem>()==null){GameObject es=new GameObject("EventSystem");es.AddComponent<EventSystem>();es.AddComponent<StandaloneInputModule>();}
        Image background=CreatePanel(canvas.transform,new Color(.018f,.024f,.035f,1));background.rectTransform.anchorMin=Vector2.zero;background.rectTransform.anchorMax=Vector2.one;background.rectTransform.offsetMin=Vector2.zero;background.rectTransform.offsetMax=Vector2.zero;
        Text title=CreateLabel(canvas.transform,"SHOTGUN 3D",new Vector2(.5f,.68f),82);title.fontStyle=FontStyle.Bold;
        Text subtitle=CreateLabel(canvas.transform,"TACTICAL COMBAT",new Vector2(.5f,.59f),30);subtitle.color=new Color(.72f,.78f,.86f);
        Button play=CreateButton(canvas.transform,"PLAY",new Vector2(.5f,.42f),360,100);play.onClick.AddListener(StartGame);
        Text hint=CreateLabel(canvas.transform,"Third-person tactical arena",new Vector2(.5f,.27f),24);hint.color=new Color(.55f,.62f,.72f);
    }

    void StartGame(){
        if(gameStarted)return;gameStarted=true;
        GameObject menu=GameObject.Find("Start Screen Canvas");if(menu)Destroy(menu);
        BuildPlayer();
        BuildHUD();
        GameFlow flow=FindObjectOfType<GameFlow>();
        if(flow==null){GameObject flowObject=new GameObject("Game Flow");flow=flowObject.AddComponent<GameFlow>();}
        flow.BeginGame();
    }

    void BuildPlayer(){
        if(FindObjectOfType<ShotgunGame>()!=null)return;
        GameObject player=new GameObject("Player");player.transform.position=new Vector3(0,0,-8f);
        CharacterController cc=player.AddComponent<CharacterController>();cc.height=1.8f;cc.radius=.35f;cc.center=new Vector3(0,.9f,0);
        GameObject visualPrefab=ArtAssetResolver.Player();
        if(visualPrefab){GameObject visual=Instantiate(visualPrefab,player.transform);visual.name="Player Visual";visual.transform.localPosition=Vector3.zero;visual.transform.localRotation=Quaternion.identity;NormalizeVisual(visual,1.8f);}
        Camera cam=new GameObject("Third Person Camera").AddComponent<Camera>();cam.tag="MainCamera";cam.fieldOfView=68;cam.nearClipPlane=.05f;
        ShotgunGame game=player.AddComponent<ShotgunGame>();game.playerCamera=cam;game.thirdPerson=true;
        ShotgunWeaponView weapon=player.AddComponent<ShotgunWeaponView>();weapon.playerCamera=cam;weapon.thirdPerson=true;
    }

    void BuildHUD(){
        if(GameObject.Find("HUD Canvas")!=null)return;
        GameObject canvasObj=new GameObject("HUD Canvas");Canvas canvas=canvasObj.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=canvasObj.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);canvasObj.AddComponent<GraphicRaycaster>();
        Text ammo=CreateLabel(canvas.transform,"AMMO  6/6",new Vector2(.5f,.92f),34);Text score=CreateLabel(canvas.transform,"SCORE  0",new Vector2(.5f,.98f),30);Text hp=CreateLabel(canvas.transform,"HP  100",new Vector2(.08f,.94f),30);Text round=CreateLabel(canvas.transform,"ROUND  1",new Vector2(.5f,.86f),24);Text message=CreateLabel(canvas.transform,"",new Vector2(.5f,.70f),38);CreateLabel(canvas.transform,"+",new Vector2(.5f,.5f),32);
        MobileHUD mobile=MobileHUD.Build(canvas);ShotgunGame game=FindObjectOfType<ShotgunGame>();if(game){game.ammoText=ammo;game.scoreText=score;game.healthText=hp;game.messageText=message;game.mobileHUD=mobile;}
        GameFlow flow=FindObjectOfType<GameFlow>();if(flow){flow.roundText=round;flow.messageText=message;}
    }

    void NormalizeVisual(GameObject visual,float targetHeight){Renderer[] rs=visual.GetComponentsInChildren<Renderer>(true);if(rs.Length==0)return;Bounds b=rs[0].bounds;foreach(Renderer r in rs)b.Encapsulate(r.bounds);float factor=targetHeight/Mathf.Max(.01f,b.size.y);visual.transform.localScale*=factor;}
    Text CreateLabel(Transform parent,string value,Vector2 anchor,int size){GameObject obj=new GameObject("UI "+value);obj.transform.SetParent(parent,false);RectTransform rect=obj.AddComponent<RectTransform>();rect.anchorMin=rect.anchorMax=anchor;rect.sizeDelta=new Vector2(800,100);Text text=obj.AddComponent<Text>();text.text=value;text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");text.fontSize=size;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;return text;}
    Image CreatePanel(Transform parent,Color color){GameObject obj=new GameObject("Background");obj.transform.SetParent(parent,false);Image image=obj.AddComponent<Image>();image.color=color;return image;}
    Button CreateButton(Transform parent,string label,Vector2 anchor,float width,float height){GameObject obj=new GameObject("Button "+label);obj.transform.SetParent(parent,false);RectTransform rect=obj.AddComponent<RectTransform>();rect.anchorMin=rect.anchorMax=anchor;rect.sizeDelta=new Vector2(width,height);Image image=obj.AddComponent<Image>();image.color=new Color(.12f,.14f,.18f,1);Button button=obj.AddComponent<Button>();Text text=CreateLabel(obj.transform,label,new Vector2(.5f,.5f),38);text.fontStyle=FontStyle.Bold;return button;}
}
