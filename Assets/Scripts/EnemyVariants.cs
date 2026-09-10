using UnityEngine;

public enum EnemyType { Grunt, Runner, Tank }

public class EnemyVariants : MonoBehaviour
{
    public EnemyType type = EnemyType.Grunt;
    TargetDummy ai;
    Vector3 baseScale;

    void Awake(){
        ai=GetComponent<TargetDummy>();
        baseScale=transform.localScale;
    }

    void Start(){
        if(!ai) ai=GetComponent<TargetDummy>();
        if(!ai) return;
        if(type==EnemyType.Runner){
            ai.health=Mathf.Max(1,Mathf.RoundToInt(ai.health*.65f));
            ai.moveSpeed*=1.9f;
            ai.damageToPlayer=Mathf.Max(1,Mathf.RoundToInt(ai.damageToPlayer*.75f));
            ai.attackInterval*=.72f;
            ai.attackDistance=2.1f;
            transform.localScale=baseScale*.85f;
        }
        else if(type==EnemyType.Tank){
            ai.health=Mathf.Max(1,Mathf.RoundToInt(ai.health*2.4f));
            ai.moveSpeed*=.55f;
            ai.damageToPlayer=Mathf.Max(1,Mathf.RoundToInt(ai.damageToPlayer*1.7f));
            ai.attackInterval*=1.2f;
            ai.attackDistance=2.7f;
            transform.localScale=baseScale*1.35f;
        }
    }
}