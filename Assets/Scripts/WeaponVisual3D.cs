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
        mount=m.transform;
        mount.SetParent(cam.transform,false);
        mount.localPosition=new Vector3(.34f,-.32f,.62f);
        mount.localRotation=Quaternion.Euler(0,180,0);
        basePos=mount.localPosition;
        Rebuild();
    }

    void Update(){
        if(!mount)return;
        WeaponSystem w=WeaponSystem.Instance;
        if(w!=null&&shownWeapon!=w.currentWeapon)Rebuild();
        kick=Mathf.MoveTowards(kick,0,Time.deltaTime*5f);
        mount.localPosition=basePos+Vector3.back*kick;
    }

    void Rebuild(){
        if(!mount)return;
        if(visual)Destroy(visual);
        WeaponSystem w=WeaponSystem.Instance;
        int index=w!=null?w.currentWeapon:0;
        GameObject prefab=index==0?ArtAssetResolver.Shotgun():index==1?ArtAssetResolver.Rifle():ArtAssetResolver.Pistol();
        if(prefab)visual=Instantiate(prefab,mount);
        else{
            visual=GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.transform.SetParent(mount,false);
            visual.transform.localScale=new Vector3(.16f,.16f,.8f);
            visual.transform.localPosition=new Vector3(0,-.02f,.35f);
        }
        visual.name=index==0?"Weapon_Shotgun":index==1?"Weapon_Rifle":"Weapon_Pistol";
        shownWeapon=index;
        foreach(Collider c in visual.GetComponentsInChildren<Collider>())Destroy(c);
    }

    public void FireKick(){kick=.055f;Muzzle();}

    void Muzzle(){
        if(!mount)return;
        GameObject f=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        f.name="MuzzleFlash";
        f.transform.SetParent(mount,false);
        f.transform.localPosition=new Vector3(0,.02f,.85f);
        f.transform.localScale=Vector3.one*.09f;
        Collider c=f.GetComponent<Collider>();if(c)Destroy(c);
        Light l=f.AddComponent<Light>();l.type=LightType.Point;l.range=3;l.intensity=5;
        Destroy(f,.055f);
    }
}
