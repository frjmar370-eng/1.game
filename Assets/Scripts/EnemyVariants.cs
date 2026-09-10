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
        if(type==EnemyType.Runner){ ai.health=2; ai.moveSpeed=2.4f; ai.damageToPlayer=6; ai.attackInterval=1.8f; ai.attackDistance=2.1f; transform.localScale=baseScale*.85f; }
        else if(type==EnemyType.Tank){ ai.health=8; ai.moveSpeed=.65f; ai.damageToPlayer=15; ai.attackInterval=3.0f; ai.attackDistance=2.7f; transform.localScale=baseScale*1.35f; }
    }
} 
