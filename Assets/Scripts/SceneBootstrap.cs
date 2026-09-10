using UnityEngine;
using UnityEngine.UI;

public class SceneBootstrap : MonoBehaviour
{
    public Material floorMaterial;
    public Material wallMaterial;

    void Awake()
    {
        BuildArena();
        BuildTargets();
        BuildPlayer();
        BuildLighting();
    }

    void BuildArena()
    {
        CreatePrimitive(PrimitiveType.Cube, "Arena Floor", new Vector3(0, -0.5f, 0), new Vector3(24, 1, 24), floorMaterial);
        CreatePrimitive(PrimitiveType.Cube, "North Wall", new Vector3(0, 2, 12), new Vector3(24, 5, 1), wallMaterial);
        CreatePrimitive(PrimitiveType.Cube, "South Wall", new Vector3(0, 2, -12), new Vector3(24, 5, 1), wallMaterial);
        CreatePrimitive(PrimitiveType.Cube, "East Wall", new Vector3(12, 2, 0), new Vector3(1, 5, 24), wallMaterial);
        CreatePrimitive(PrimitiveType.Cube, "West Wall", new Vector3(-12, 2, 0), new Vector3(1, 5, 24), wallMaterial);
    }

    void BuildTargets()
    {
        for (int i = 0; i < 6; i++)
        {
            float x = -7.5f + (i % 3) * 7.5f;
            float z = 5f + (i / 3) * 4.5f;
            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            t.name = "Target_" + (i + 1);
            t.transform.position = new Vector3(x, 1.1f, z);
            t.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            t.AddComponent<TargetDummy>();
        }
    }

    void BuildPlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0, 1.2f, -7f);
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        Camera cam = new GameObject("Player Camera").AddComponent<Camera>();
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0, 0.65f, 0);
        cam.fieldOfView = 70;
        ShotgunGame game = player.AddComponent<ShotgunGame>();
        game.playerCamera = cam;
    }

    void BuildLighting()
    {
        RenderSettings.ambientIntensity = 1.0f;
        GameObject lightObject = new GameObject("Key Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        light.transform.rotation = Quaternion.Euler(45, -30, 0);
    }

    GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        if (material != null) obj.GetComponent<Renderer>().material = material;
        return obj;
    }
}
