using UnityEngine;

public class WeaponVisual3D : MonoBehaviour
{
    Transform mount;
    GameObject visual;
    Vector3 basePos;
    float kick;
    int shownWeapon=-1;

    void Start(){
        Camera cam=Camera.main;
        if(!cam)return;
        GameObject m=new GameObject("Weapon_Mount");
        mount=m.transform;m.parent=cam.transform;mount.localPosition=new Vector3(.34f,-.32f,.62f);mount.localRotation=Quaternion.Euler(0,180,0);basePos=mount.localPosition;
        Rebuild();
    }

    void Update(){
        if(!mount)return;
        WeaponSystem w=WeaponSystem.Instance;
        if(w!=null&&shownWeapon!=w.currentWeapon)Rebuild();
        kick=Mathf.Lerp(kick,0,Time.deltaTime*14f);
        mount.localPosition=basePos+Vector3.back*kick;
        if(Input.GetMouseButtonDown(0)&&w!=null&&w.CanFire()){kick=.055f;Muzzle();}
    }

    void Rebuild(){
        if(!mount)return;
        if(visual)Destroy(visual);
        visual=ArtAssetResolver.Shotgun();
        if(visual)visual=Instantiate(visual,mount);
        else{
            visual=GameObject.CreatePrimitive(PrimitiveType.Cube);visual.transform.SetParent(mount,false);visual.transform.localScale=new Vector3(.16f,.16f,.8f);visual.transform.localPosition=new Vector3(0,-.02f,.35f);
        }
        visual.name="Weapon_Real_Visual";shownWeapon=WeaponSystem.Instance?WeaponSystem.Instance.currentWeapon:0;
        foreach(Collider c in visual.GetComponentsInChildren<Collider>())Destroy(c);
    }

    void Muzzle(){
        GameObject f=GameObject.CreatePrimitive(PrimitiveType.Sphere);f.name="MuzzleFlash";f.transform.SetParent(mount,false);f.transform.localPosition=new Vector3(0,.02f,.85f);f.transform.localScale=Vector3.one*.09f;Destroy(f.GetComponent<Collider>());
        Light l=f.AddComponent<Light>();l.type=LightType.Point;l.range=3;l.intensity=5;
        Destroy(f,.055f);
    }
}
