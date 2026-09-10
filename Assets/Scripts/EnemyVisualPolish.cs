using UnityEngine;

public class EnemyVisualPolish : MonoBehaviour
{
    TargetDummy ai;
    Vector3 startScale;
    float lastHealth;
    float flash;
    Renderer[] renderers;

    void Awake(){
        ai=GetComponent<TargetDummy>();
        startScale=transform.localScale;
        renderers=GetComponentsInChildren<Renderer>();
    }
    void Start(){if(ai)lastHealth=ai.health;}
    void Update(){
        if(!ai)return;
        if(ai.health<lastHealth){flash=.12f;transform.localScale=startScale*1.08f;lastHealth=ai.health;}
        if(flash>0){flash-=Time.deltaTime;}
        else transform.localScale=Vector3.Lerp(transform.localScale,startScale,Time.deltaTime*10f);
        float bob=Mathf.Sin(Time.time*5f+GetInstanceID())*.025f;
        transform.position+=Vector3.up*bob*Time.deltaTime;
    }
}
