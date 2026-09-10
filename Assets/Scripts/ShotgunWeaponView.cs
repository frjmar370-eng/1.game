using System.Collections;
using UnityEngine;

public class ShotgunWeaponView : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject weaponPrefab;
    public bool thirdPerson;
    Transform weapon;
    Light muzzleFlash;
    Transform rightHand;
    Vector3 restPos = new Vector3(.42f,1.18f,.42f);
    float recoil;

    void Start()
    {
        GameObject prefab=weaponPrefab;
        if(prefab==null)prefab=Resources.Load<GameObject>("Weapons/Shotgun");
        rightHand=FindDeepChild(transform,"RightHand","mixamorig:RightHand","mixamorigRightHand","Hand_R","hand_r","R_Hand");
        if(prefab!=null)
        {
            GameObject instance=Instantiate(prefab,transform);
            instance.name="Shotgun Model";weapon=instance.transform;
            if(thirdPerson)
            {
                if(rightHand!=null)
                {
                    weapon.SetParent(rightHand,false);
                    weapon.localPosition=new Vector3(.03f,-.03f,.02f);
                    weapon.localRotation=Quaternion.Euler(0,90,0);
                }
                else {weapon.localPosition=restPos;weapon.localRotation=Quaternion.identity;}
                NormalizeImportedModel(weapon,.95f);
            }
            else if(playerCamera)
            {
                weapon.SetParent(playerCamera.transform,false);weapon.localPosition=new Vector3(.32f,-.28f,.55f);weapon.localRotation=Quaternion.Euler(4,-8,0);NormalizeImportedModel(weapon,.95f);
            }
        }
        else if(!thirdPerson)BuildFallbackWeapon();
        Transform muzzle=FindDeepChild(weapon,"muzzle","muzzleflash","muzzle_flash","barrel_end","barrelend");
        if(muzzle==null&&weapon!=null){GameObject flash=new GameObject("Muzzle Flash");flash.transform.SetParent(weapon,false);flash.transform.localPosition=new Vector3(0,0,.8f);muzzle=flash.transform;}
        if(muzzle!=null){muzzleFlash=muzzle.GetComponent<Light>();if(muzzleFlash==null)muzzleFlash=muzzle.gameObject.AddComponent<Light>();muzzleFlash.type=LightType.Point;muzzleFlash.range=4f;muzzleFlash.intensity=0;}
    }

    void Update(){if(!weapon)return;recoil=Mathf.MoveTowards(recoil,0,Time.deltaTime*4f);if(rightHand!=null&&thirdPerson)weapon.localPosition+=Vector3.back*recoil;else weapon.localPosition=restPos+Vector3.back*recoil;}
    public void FireKick(){recoil=.14f;if(muzzleFlash)StartCoroutine(Flash());}
    public void Kick()=>FireKick();
    IEnumerator Flash(){muzzleFlash.intensity=5f;yield return new WaitForSeconds(.045f);if(muzzleFlash)muzzleFlash.intensity=0;}

    void NormalizeImportedModel(Transform root,float targetSize){Bounds bounds=new Bounds(root.position,Vector3.zero);Renderer[] rs=root.GetComponentsInChildren<Renderer>(true);if(rs.Length==0)return;foreach(Renderer r in rs)bounds.Encapsulate(r.bounds);float size=Mathf.Max(bounds.size.x,Mathf.Max(bounds.size.y,bounds.size.z));if(size>.001f)root.localScale*=targetSize/size;}
    void BuildFallbackWeapon(){weapon=GameObject.CreatePrimitive(PrimitiveType.Cube).transform;weapon.name="Shotgun Model Fallback";weapon.SetParent(transform,false);weapon.localPosition=restPos;weapon.localScale=new Vector3(.16f,.16f,.75f);Renderer body=weapon.GetComponent<Renderer>();if(body)body.material=MakeMaterial(new Color(.06f,.07f,.08f));}
    Transform FindDeepChild(Transform root,params string[] names){if(root==null)return null;foreach(Transform t in root.GetComponentsInChildren<Transform>(true)){string n=t.name.ToLowerInvariant().Replace(" ","").Replace("-","_");foreach(string wanted in names){string w=wanted.ToLowerInvariant().Replace(" ","").Replace("-","_");if(n.Contains(w))return t;}}return null;}
    Material MakeMaterial(Color color){Shader shader=Shader.Find("Standard");if(shader==null)shader=Shader.Find("Unlit/Color");if(shader==null)return null;Material m=new Material(shader);m.color=color;return m;}
}
