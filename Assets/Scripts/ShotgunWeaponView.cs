using System.Collections;
using UnityEngine;

public class ShotgunWeaponView : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject weaponPrefab;

    Transform weapon;
    Light muzzleFlash;
    Vector3 restPos = new Vector3(.32f, -.28f, .55f);
    float recoil;

    void Start()
    {
        if (!playerCamera) return;

        GameObject prefab = weaponPrefab;
        if (prefab == null)
            prefab = Resources.Load<GameObject>("Weapons/Shotgun");

        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, playerCamera.transform);
            instance.name = "Shotgun Model";
            weapon = instance.transform;
            weapon.localPosition = restPos;
            weapon.localRotation = Quaternion.Euler(4, -8, 0);
            NormalizeImportedModel(weapon);
        }
        else
        {
            BuildFallbackWeapon();
        }

        Transform muzzle = FindDeepChild(weapon, "muzzle", "muzzleflash", "muzzle_flash", "barrel_end");
        if (muzzle == null && weapon != null)
        {
            GameObject flash = new GameObject("Muzzle Flash");
            flash.transform.SetParent(weapon, false);
            flash.transform.localPosition = new Vector3(0, 0, .8f);
            muzzle = flash.transform;
        }

        if (muzzle != null)
        {
            muzzleFlash = muzzle.GetComponent<Light>();
            if (muzzleFlash == null) muzzleFlash = muzzle.gameObject.AddComponent<Light>();
            muzzleFlash.type = LightType.Point;
            muzzleFlash.range = 4f;
            muzzleFlash.intensity = 0f;
        }
    }

    void Update()
    {
        if (!weapon) return;
        recoil = Mathf.MoveTowards(recoil, 0f, Time.deltaTime * 4f);
        weapon.localPosition = restPos + Vector3.back * recoil;
    }

    public void FireKick()
    {
        recoil = .14f;
        if (muzzleFlash) StartCoroutine(Flash());
    }

    public void Kick() => FireKick();

    IEnumerator Flash()
    {
        muzzleFlash.intensity = 5f;
        yield return new WaitForSeconds(.045f);
        if (muzzleFlash) muzzleFlash.intensity = 0f;
    }

    void NormalizeImportedModel(Transform root)
    {
        Bounds bounds = new Bounds(root.position, Vector3.zero);
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;
        foreach (Renderer r in renderers) bounds.Encapsulate(r.bounds);

        float size = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        if (size > .001f)
            root.localScale *= .95f / size;

        root.localPosition = restPos;
    }

    void BuildFallbackWeapon()
    {
        weapon = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        weapon.name = "Shotgun Model Fallback";
        weapon.SetParent(playerCamera.transform, false);
        weapon.localPosition = restPos;
        weapon.localRotation = Quaternion.Euler(4, -8, 0);
        weapon.localScale = new Vector3(.16f, .16f, .75f);
        Renderer body = weapon.GetComponent<Renderer>();
        if (body != null) body.material = MakeMaterial(new Color(.06f, .07f, .08f));

        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Shotgun Barrel Fallback";
        barrel.transform.SetParent(weapon, false);
        barrel.transform.localPosition = new Vector3(0, 0, .62f);
        barrel.transform.localRotation = Quaternion.Euler(90, 0, 0);
        barrel.transform.localScale = new Vector3(.07f, .45f, .07f);
        Renderer barrelRenderer = barrel.GetComponent<Renderer>();
        if (barrelRenderer != null) barrelRenderer.material = MakeMaterial(new Color(.12f, .13f, .14f));
    }

    Transform FindDeepChild(Transform root, params string[] names)
    {
        if (root == null) return null;
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            string n = t.name.ToLowerInvariant().Replace(" ", "").Replace("-", "_");
            foreach (string wanted in names)
                if (n.Contains(wanted)) return t;
        }
        return null;
    }

    Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) return null;
        Material m = new Material(shader);
        m.color = color;
        return m;
    }
}
