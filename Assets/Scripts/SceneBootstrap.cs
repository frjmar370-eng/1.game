using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneBootstrap : MonoBehaviour
{
    public Material floorMaterial;
    public Material wallMaterial;

    void Awake()
    {
        BuildArena(); BuildProps(); BuildTargets(); BuildPlayer(); BuildLighting(); BuildHUD();
    }

    void BuildArena()
    {
        CreatePrimitive(PrimitiveType.Cube, "Arena Floor", new Vector3(0, -.5f, 0), new Vector3(24, 1, 24), floorMaterial);
        CreatePrimitive(PrimitiveType.Cube, "North Wall", new Vector3(0, 2, 12), new Vector3(24, 5, 1), wallMaterial);
        CreatePrimitive(PrimitiveType.Cube, "South Wall", new Vector3(0, 2, -12), new Vector3(24, 5, 1), wallMaterial);
        CreatePrimitive(PrimitiveType.Cube, "East Wall", new Vector3(12, 2, 0), new Vector3(1, 5, 24), wallMaterial);
        CreatePrimitive(PrimitiveType.Cube, "West Wall", new Vector3(-12, 2, 0), new Vector3(1, 5, 24), wallMaterial);
    }

    void BuildProps()
    {
        for (int i = 0; i < 8; i++)
        {
            float x = -9f + (i % 4) * 6f;
            float z = -1f + (i / 4) * 8f;
            GameObject crate = CreatePrimitive(PrimitiveType.Cube, "Crate_" + i, new Vector3(x, .65f, z), new Vector3(1.4f, 1.3f, 1.4f), wallMaterial);
            crate.transform.Rotate(0, (i * 17f) % 45f, 0);
        }
        for (int i = 0; i < 4; i++) CreatePrimitive(PrimitiveType.Cylinder, "Barrel_" + i, new Vector3(-8f + i * 5f, 1f, 9f), new Vector3(.8f, 1f, .8f), wallMaterial);
    }

    void BuildTargets()
    {
        for (int i = 0; i < 6; i++)
        {
            float x = -7.5f + (i % 3) * 7.5f;
            float z = 4.5f + (i / 3) * 4.2f;
            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            t.name = "Target_" + (i + 1);
            t.transform.position = new Vector3(x, 1.1f, z);
            t.transform.localScale = Vector3.one * 1.1f;
            t.GetComponent<Renderer>().material = MakeMaterial(new Color(.85f, .18f, .12f));
            t.AddComponent<TargetDummy>();
        }
    }

    void BuildPlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0, 1.2f, -7f);
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f; cc.radius = .35f;
        Camera cam = new GameObject("Player Camera").AddComponent<Camera>();
        cam.transform.SetParent(player.transform); cam.transform.localPosition = new Vector3(0, .65f, 0);
        cam.fieldOfView = 70; cam.tag = "MainCamera";
        ShotgunGame game = player.AddComponent<ShotgunGame>(); game.playerCamera = cam;
    }

    void BuildHUD()
    {
        GameObject canvasObj = new GameObject("HUD Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        if (FindObjectOfType<EventSystem>() == null) { GameObject es = new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>(); }

        Text ammo = CreateLabel(canvas.transform, "AMMO  6/6", new Vector2(.5f, .92f), 34);
        Text score = CreateLabel(canvas.transform, "SCORE  0", new Vector2(.5f, .98f), 30);
        Text hp = CreateLabel(canvas.transform, "HP  100", new Vector2(.08f, .94f), 30);
        CreateLabel(canvas.transform, "+", new Vector2(.5f, .5f), 32);
        MobileHUD mobile = MobileHUD.Build(canvas);
        ShotgunGame game = FindObjectOfType<ShotgunGame>();
        if (game != null) { game.ammoText = ammo; game.scoreText = score; game.healthText = hp; game.mobileHUD = mobile; }
    }

    Text CreateLabel(Transform parent, string value, Vector2 anchor, int size)
    {
        GameObject obj = new GameObject("UI " + value); obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = new Vector2(500, 70);
        Text text = obj.AddComponent<Text>(); text.text = value; text.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize = size; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white;
        return text;
    }

    Material MakeMaterial(Color color) { Material m = new Material(Shader.Find("Standard")); m.color = color; return m; }

    void BuildLighting()
    {
        RenderSettings.ambientIntensity = 1.0f;
        GameObject lightObject = new GameObject("Key Light"); Light light = lightObject.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.25f; light.transform.rotation = Quaternion.Euler(45, -30, 0);
    }

    GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(type); obj.name = name; obj.transform.position = position; obj.transform.localScale = scale;
        if (material != null) obj.GetComponent<Renderer>().material = material; return obj;
    }
}
