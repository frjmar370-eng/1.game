using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    public int health=3;
    public float moveRadius=2.5f;
    public float moveSpeed=1.2f;
    public int damageToPlayer=8;
    public float attackInterval=2.5f;
    public float chaseDistance=18f;
    public float attackDistance=2.4f;
    Vector3 origin; float phase; float nextAttack;
    Renderer body; Color baseColor; Vector3 baseScale;
    ShotgunGame player;

    void Start(){
        origin=transform.position; phase=Random.value*6.28f;
        nextAttack=Time.time+Random.Range(1f,attackInterval);
        body=GetComponentInChildren<Renderer>();
        if(body){baseColor=body.material.color;baseScale=transform.localScale;}
        player=FindObjectOfType<ShotgunGame>();
    }

    void Update(){
        if(!player) player=FindObjectOfType<ShotgunGame>();
        if(!player || player.IsGameOver) return;
        Vector3 toPlayer=player.transform.position-transform.position;
        Vector3 flat=new Vector3(toPlayer.x,0,toPlayer.z);
        float dist=flat.magnitude;
        if(dist>.01f) transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(flat),Time.deltaTime*6f);
        if(dist<=chaseDistance){
            if(dist>attackDistance){
                Vector3 dir=flat.normalized;
                float strafe=Mathf.Sin(Time.time*moveSpeed+phase)*0.35f;
                Vector3 side=Vector3.Cross(Vector3.up,dir)*strafe;
                transform.position+=(dir*moveSpeed+side)*Time.deltaTime;
            } else {
                Vector3 home=origin-transform.position; home.y=0;
                if(home.magnitude>moveRadius*2f) transform.position+=home.normalized*moveSpeed*.25f*Time.deltaTime;
            }
            if(Time.time>=nextAttack&&dist<=attackDistance){
                nextAttack=Time.time+attackInterval+Random.Range(.2f,.8f);
                player.TakeDamage(damageToPlayer);
                if(VisualFX.Instance)VisualFX.Instance.Impact(player.transform.position+Vector3.up);
                if(AudioManager.Instance)AudioManager.Instance.Damage();
            }
        } else {
            Vector3 patrol=origin;
            patrol.x+=Mathf.Sin(Time.time*moveSpeed+phase)*moveRadius;
            patrol.z+=Mathf.Cos(Time.time*moveSpeed*.7f+phase)*moveRadius*.5f;
            transform.position=Vector3.MoveTowards(transform.position,patrol,moveSpeed*Time.deltaTime);
        }
    }

    public bool Hit(int damage){
        health-=damage;
        if(body)StartCoroutine(HitFlash());
        if(VisualFX.Instance)VisualFX.Instance.Hit();
        if(AudioManager.Instance)AudioManager.Instance.Hit();
        if(health<=0){
            if(GameFlow.Instance)GameFlow.Instance.TargetKilled();
            if(MissionSystem.Instance)MissionSystem.Instance.Kill();
            if(VisualFX.Instance)VisualFX.Instance.Death(transform.position+Vector3.up*.8f);
            Destroy(gameObject);return true;
        }
        return false;
    }

    System.Collections.IEnumerator HitFlash(){
        if(!body)yield break;
        body.material.color=Color.white;transform.localScale=baseScale*1.08f;
        yield return new WaitForSeconds(.07f);
        if(body)body.material.color=baseColor;transform.localScale=baseScale;
    }
}
