using UnityEngine;
using System.Collections;

public class BossSystem : MonoBehaviour
{
    public static BossSystem Instance{get;private set;}
    public bool Active{get;private set;}
    TargetDummy boss;
    void Awake(){if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}Instance=this;}
    public void SpawnBoss(Vector3 position,int wave){if(Active)return;StartCoroutine(Spawn(position,wave));}
    IEnumerator Spawn(Vector3 position,int wave){yield return null;GameObject o=GameObject.CreatePrimitive(PrimitiveType.Capsule);o.name="BOSS";o.transform.position=position;o.transform.localScale=new Vector3(1.8f,2.4f,1.8f);boss=o.AddComponent<TargetDummy>();boss.health=30+wave*5;boss.moveSpeed=.9f;boss.damageToPlayer=22;boss.attackInterval=2f;boss.attackDistance=3f;boss.chaseDistance=40f;o.AddComponent<EnemyVariants>().type=EnemyType.Tank;Active=true;}
    public void NotifyDeath(TargetDummy dead){if(dead!=boss)return;Active=false;}
}