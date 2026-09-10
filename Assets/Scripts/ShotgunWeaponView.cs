using UnityEngine;

public class ShotgunWeaponView : MonoBehaviour
{
    public Camera playerCamera;
    Transform weapon;
    Vector3 restPos = new Vector3(.32f, -.28f, .55f);
    Vector3 recoilPos;
    float recoil;

    void Start()
    {
        weapon = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        weapon.name = "Shotgun Model";
        weapon.SetParent(playerCamera.transform, false);
        weapon.localPosition = restPos;
        weapon.localRotation = Quaternion.Euler(4, -8, 0);
        weapon.localScale = new Vector3(.16f, .16f, .75f);
        weapon.GetComponent<Renderer>().material = MakeMaterial(new Color(.06f,.07f,.08f));
        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Shotgun Barrel";
        barrel.transform.SetParent(weapon, false);
        barrel.transform.localPosition = new Vector3(0,0,.62f);
        barrel.transform.localRotation = Quaternion.Euler(90,0,0);
        barrel.transform.localScale = new Vector3(.07f,.45f,.07f);
        barrel.GetComponent<Renderer>().material = MakeMaterial(new Color(.12f,.13f,.14f));
    }

    void Update()
    {
        if (!weapon) return;
        recoil = Mathf.MoveTowards(recoil, 0f, Time.deltaTime * 4f);
        weapon.localPosition = restPos + Vector3.back * recoil;
    }

    public void Kick()
    {
        recoil = .14f;
    }

    Material MakeMaterial(Color color)
    {
        Material m = new Material(Shader.Find("Standard"));
        m.color = color;
        return m;
    }
}
