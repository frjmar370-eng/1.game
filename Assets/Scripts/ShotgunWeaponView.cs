using System.Collections;
using UnityEngine;

public class ShotgunWeaponView : MonoBehaviour
{
    public Camera playerCamera;
    Transform weapon;
    Light muzzleFlash;
    Vector3 restPos = new Vector3(.32f, -.28f, .55f);
    float recoil;

    void Start()
    {
        if (!playerCamera) return;
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
        GameObject flash = new GameObject("Muzzle Flash");
        flash.transform.SetParent(barrel.transform, false);
        flash.transform.localPosition = new Vector3(0,0,.48f);
        muzzleFlash = flash.AddComponent<Light>();
        muzzleFlash.type = LightType.Point;
        muzzleFlash.range = 4f;
        muzzleFlash.intensity = 0f;
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

    Material MakeMaterial(Color color)
    {
        Material m = new Material(Shader.Find("Standard"));
        m.color = color;
        return m;
    }
}
