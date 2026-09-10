using UnityEngine;
using System.Collections;

public class BossSystem : MonoBehaviour
{
    public static BossSystem Instance{get;private set;}
    public bool Active{get;private set;}
    TargetDummy boss;
    void Awake(){if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}Instance=this;}
    public void SpawnBoss(Vector3 position,int wave){if(Active)return;StartCoroutine(Spawn(position,wave));}
    IEnumerator Spawn(Vector3 position,int wave)
    {
        yield return null;
        GameObject root=new GameObject("BOSS_W"+wave);
        root.transform.position=position;

        GameObject visual=ArtAssetResolver.Enemy();
        if(visual!=null){
            GameObject v=Instantiate(visual,root.transform);
            v.name="Boss_Real_Model";
            v.transform.localPosition=new Vector3(0,0,0);
            v.transform.localRotation=Quaternion.identity;
            v.transform.localScale=Vector3.one*1.35f;
        }else{
            GameObject body=GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name="Boss_Fallback_Visual";
            body.transform.SetParent(root.transform,false);
            body.transform.localPosition=Vector3.zero;
            body.transform.localScale=new Vector3(1.8f,2.4f,1.8f);
        }

        boss=root.AddComponent<TargetDummy>();
        boss.health=30+wave*5;
        boss.moveSpeed=.9f;
        boss.damageToPlayer=22;
        boss.attackInterval=2f;
        boss.attackDistance=3f;
        boss.chaseDistance=40f;
        Active=true;
    }
    public void NotifyDeath(TargetDummy dead){if(dead!=boss)return;Active=false;}
}
