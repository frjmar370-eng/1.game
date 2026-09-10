using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneBootstrap : MonoBehaviour
{
    public Material floorMaterial;
    public Material wallMaterial;
    bool gameStarted;

    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        BuildStartScreen();
    }

    void BuildStartScreen()
    {
        GameObject canvasObj = new GameObject("Start Screen Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        if (FindObjectOfType<EventSystem>() == null) { GameObject es = new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>(); }
        Image background = CreatePanel(canvas.transform, new Color(.018f,.024f,.035f,1)); background.rectTransform.anchorMin=Vector2.zero; background.rectTransform.anchorMax=Vector2.one; background.rectTransform.offsetMin=Vector2.zero; background.rectTransform.offsetMax=Vector2.zero;
        Text title = CreateLabel(canvas.transform,"SHOTGUN 3D",new Vector2(.5f,.68f),82); title.fontStyle=FontStyle.Bold;
        Text subtitle = CreateLabel(canvas.transform,"TACTICAL COMBAT",new Vector2(.5f,.59f),30); subtitle.color=new Color(.72f,.78f,.86f);
        Button play = CreateButton(canvas.transform,"PLAY",new Vector2(.5f,.42f),360,100); play.onClick.AddListener(StartGame);
        Text hint=CreateLabel(canvas.transform,"Third-person tactical arena",new Vector2(.5f,.27f),24); hint.color=new Color(.55f,.62f,.72f);
    }

    void StartGame()
    {
        if (gameStarted) return; gameStarted=true;
        GameObject menu=GameObject.Find("Start Screen Canvas"); if(menu) Destroy(menu);
        if(floorMaterial==null) floorMaterial=MakeMaterial(new Color(.10f,.12f,.15f));
        if(wallMaterial==null) wallMaterial=MakeMaterial(new Color(.22f,.25f,.29f));
        BuildArena(); BuildProps(); BuildTargets(); BuildPlayer(); BuildLighting(); BuildFlow(); BuildHUD();
    }

    void BuildArena()
    {
        CreatePrimitive(PrimitiveType.Cube,"Arena Floor",new Vector3(0,-.5f,0),new Vector3(28,1,28),floorMaterial);
        CreatePrimitive(PrimitiveType.Cube,"North Wall",new Vector3(0,2,14),new Vector3(28,5,1),wallMaterial);
        CreatePrimitive(PrimitiveType.Cube,"South Wall",new Vector3(0,2,-14),new Vector3(28,5,1),wallMaterial);
        CreatePrimitive(PrimitiveType.Cube,"East Wall",new Vector3(14,2,0),new Vector3(1,5,28),wallMaterial);
        CreatePrimitive(PrimitiveType.Cube,"West Wall",new Vector3(-14,2,0),new Vector3(1,5,28),wallMaterial);
        GameObject screenPrefab=Resources.Load<GameObject>("Environment/Screen");
        if(screenPrefab!=null){ for(int i=0;i<3;i++) SpawnArt(screenPrefab,"TacticalScreen_"+i,new Vector3(-8+i*8,3.1f,13.35f),new Vector3(2,2,2)); }
        GameObject doorPrefab=Resources.Load<GameObject>("Environment/Door");
        if(doorPrefab!=null){ SpawnArt(doorPrefab,"NorthDoor",new Vector3(0,1.5f,13.25f),new Vector3(1.5f,1.5f,1.5f)); }
    }

    void BuildProps()
    {
        GameObject cratePrefab=Resources.Load<GameObject>("Environment/Crate"); GameObject barrelPrefab=Resources.Load<GameObject>("Environment/Barrel");
        for(int i=0;i<12;i++){
            float x=-10f+(i%4)*6.5f; float z=-1f+(i/4)*6f; Vector3 pos=new Vector3(x,.65f,z);
            GameObject c=cratePrefab?SpawnArt(cratePrefab,"Crate_"+i,pos,new Vector3(1.25f,1.25f,1.25f)):CreatePrimitive(PrimitiveType.Cube,"Crate_"+i,pos,new Vector3(1.4f,1.3f,1.4f),wallMaterial); c.transform.Rotate(0,(i*17f)%45f,0);
        }
        for(int i=0;i<6;i++){ Vector3 pos=new Vector3(-11f+i*4.4f,1f,10.5f); if(barrelPrefab)SpawnArt(barrelPrefab,"Barrel_"+i,pos,Vector3.one*1.0f); }
        // Simple cover walls create a more tactical third-person arena.
        for(int i=0;i<5;i++) CreatePrimitive(PrimitiveType.Cube,"Cover_"+i,new Vector3(-10+i*5f,1.0f,4.5f),new Vector3(2.8f,2f,.55f),wallMaterial);
    }

    GameObject SpawnArt(GameObject prefab,string name,Vector3 position,Vector3 scale)
    {
        GameObject obj=Instantiate(prefab,position,Quaternion.identity); obj.name=name; obj.transform.localScale=scale;
        if(obj.GetComponentInChildren<Collider>()==null) obj.AddComponent<BoxCollider>();
        return obj;
    }

    void BuildTargets()
    {
        GameObject enemyPrefab=Resources.Load<GameObject>("Characters/Enemy");
        for(int i=0;i<6;i++){
            float x=-8f+(i%3)*8f,z=5f+(i/3)*4.5f; Vector3 pos=new Vector3(x,0,z);
            if(enemyPrefab){ GameObject e=SpawnArt(enemyPrefab,"Enemy_"+(i+1),pos,Vector3.one); e.AddComponent<TargetDummy>(); }
            else { GameObject t=GameObject.CreatePrimitive(PrimitiveType.Capsule); t.name="Target_"+(i+1); t.transform.position=new Vector3(x,1.1f,z); t.transform.localScale=Vector3.one*1.1f; t.GetComponent<Renderer>().material=MakeMaterial(new Color(.85f,.18f,.12f)); t.AddComponent<TargetDummy>(); }
        }
    }

    void BuildPlayer()
    {
        GameObject player=new GameObject("Player"); player.transform.position=new Vector3(0,0,-8f);
        CharacterController cc=player.AddComponent<CharacterController>(); cc.height=1.8f; cc.radius=.35f; cc.center=new Vector3(0,.9f,0);
        GameObject visualPrefab=Resources.Load<GameObject>("Characters/Player");
        if(visualPrefab){ GameObject visual=Instantiate(visualPrefab,player.transform); visual.name="Player Visual"; visual.transform.localPosition=Vector3.zero; visual.transform.localRotation=Quaternion.identity; NormalizeVisual(visual,1.8f); }
        Camera cam=new GameObject("Third Person Camera").AddComponent<Camera>(); cam.tag="MainCamera"; cam.fieldOfView=68; cam.nearClipPlane=.05f;
        ShotgunGame game=player.AddComponent<ShotgunGame>(); game.playerCamera=cam; game.thirdPerson=true;
        ShotgunWeaponView weapon=player.AddComponent<ShotgunWeaponView>(); weapon.playerCamera=cam; weapon.thirdPerson=true;
    }

    void NormalizeVisual(GameObject visual,float targetHeight)
    {
        Renderer[] rs=visual.GetComponentsInChildren<Renderer>(true); if(rs.Length==0)return; Bounds b=rs[0].bounds; foreach(Renderer r in rs)b.Encapsulate(r.bounds);
        float h=Mathf.Max(.01f,b.size.y); float factor=targetHeight/h; visual.transform.localScale*=factor;
        b=rs[0].bounds; foreach(Renderer r in rs)b.Encapsulate(r.bounds); float bottom=b.min.y; visual.transform.position+=Vector3.up*(visual.transform.position.y-bottom);
    }

    void BuildHUD()
    {
        GameObject canvasObj=new GameObject("HUD Canvas"); Canvas canvas=canvasObj.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay; CanvasScaler scaler=canvasObj.AddComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080); canvasObj.AddComponent<GraphicRaycaster>();
        Text ammo=CreateLabel(canvas.transform,"AMMO  6/6",new Vector2(.5f,.92f),34); Text score=CreateLabel(canvas.transform,"SCORE  0",new Vector2(.5f,.98f),30); Text hp=CreateLabel(canvas.transform,"HP  100",new Vector2(.08f,.94f),30); Text round=CreateLabel(canvas.transform,"ROUND  1   60",new Vector2(.5f,.86f),24); Text message=CreateLabel(canvas.transform,"",new Vector2(.5f,.70f),38); CreateLabel(canvas.transform,"+",new Vector2(.5f,.5f),32);
        MobileHUD mobile=MobileHUD.Build(canvas); ShotgunGame game=FindObjectOfType<ShotgunGame>(); if(game){game.ammoText=ammo;game.scoreText=score;game.healthText=hp;game.messageText=message;game.mobileHUD=mobile;} GameFlow flow=FindObjectOfType<GameFlow>(); if(flow){flow.roundText=round;flow.messageText=message;}
    }

    void BuildFlow(){if(FindObjectOfType<GameFlow>()==null)new GameObject("Game Flow").AddComponent<GameFlow>();}
    Text CreateLabel(Transform parent,string value,Vector2 anchor,int size){GameObject obj=new GameObject("UI "+value);obj.transform.SetParent(parent,false);RectTransform rect=obj.AddComponent<RectTransform>();rect.anchorMin=rect.anchorMax=anchor;rect.sizeDelta=new Vector2(800,100);Text text=obj.AddComponent<Text>();text.text=value;text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");text.fontSize=size;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;return text;}
    Image CreatePanel(Transform parent,Color color){GameObject obj=new GameObject("Background");obj.transform.SetParent(parent,false);Image image=obj.AddComponent<Image>();image.color=color;return image;}
    Button CreateButton(Transform parent,string label,Vector2 anchor,float width,float height){GameObject obj=new GameObject("Button "+label);obj.transform.SetParent(parent,false);RectTransform rect=obj.AddComponent<RectTransform>();rect.anchorMin=rect.anchorMax=anchor;rect.sizeDelta=new Vector2(width,height);Image image=obj.AddComponent<Image>();image.color=new Color(.12f,.14f,.18f,1);Button button=obj.AddComponent<Button>();Text text=CreateLabel(obj.transform,label,new Vector2(.5f,.5f),38);text.fontStyle=FontStyle.Bold;return button;}
    Material MakeMaterial(Color color){Shader shader=Shader.Find("Standard");if(shader==null)shader=Shader.Find("Unlit/Color");if(shader==null)return null;Material m=new Material(shader);m.color=color;return m;}
    void BuildLighting(){RenderSettings.ambientIntensity=1f;GameObject o=new GameObject("Key Light");Light l=o.AddComponent<Light>();l.type=LightType.Directional;l.intensity=1.35f;l.shadows=LightShadows.Soft;l.transform.rotation=Quaternion.Euler(50,-35,0);}
    GameObject CreatePrimitive(PrimitiveType type,string name,Vector3 position,Vector3 scale,Material material){GameObject obj=GameObject.CreatePrimitive(type);obj.name=name;obj.transform.position=position;obj.transform.localScale=scale;if(material!=null)obj.GetComponent<Renderer>().material=material;return obj;}
}
