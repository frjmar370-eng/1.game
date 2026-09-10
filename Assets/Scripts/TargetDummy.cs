using UnityEngine;
using System.Collections;

public class TargetDummy : MonoBehaviour
{
    public int health=3; public float moveRadius=2.5f,moveSpeed=1.2f; public int damageToPlayer=8; public float attackInterval=2.5f,chaseDistance=18f,attackDistance=2.4f;
    public int MaxHealth { get; private set; }
    Vector3 origin; float phase,nextAttack; Renderer body; Color baseColor; Vector3 baseScale; ShotgunGame player; CharacterController controller;
    void Start(){
        origin=transform.position;phase=Random.value*6.28f;nextAttack=Time.time+Random.Range(1f,attackInterval);body=GetComponentInChildren<Renderer>();if(body){baseColor=body.material.color;baseScale=transform.localScale;}
        MaxHealth=Mathf.Max(1,health);player=FindObjectOfType<ShotgunGame>();controller=GetComponent<CharacterController>();
    }
    void Update(){
        if(!player)player=FindObjectOfType<ShotgunGame>();if(!player||player.IsGameOver)return;
        Vector3 to=player.transform.position-transform.position;Vector3 flat=new Vector3(to.x,0,to.z);float dist=flat.magnitude;
        if(dist>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(flat),Time.deltaTime*6f);
        if(dist<=chaseDistance){
            if(dist>attackDistance){
                Vector3 dir=flat.normalized;Vector3 side=Vector3.Cross(Vector3.up,dir)*Mathf.Sin(Time.time*moveSpeed+phase)*.35f;Vector3 velocity=(dir*moveSpeed+side);
                if(controller){if(controller.isGrounded)controller.Move(velocity*Time.deltaTime);else controller.Move((velocity+Physics.gravity)*Time.deltaTime);}
                else MoveWithCollision(velocity*Time.deltaTime);
            }
            if(Time.time>=nextAttack&&dist<=attackDistance){nextAttack=Time.time+attackInterval+Random.Range(.2f,.8f);player.TakeDamage(damageToPlayer);if(VisualFX.Instance)VisualFX.Instance.Impact(player.transform.position+Vector3.up);if(AudioManager.Instance)AudioManager.Instance.Damage();}
        }else{
            Vector3 patrol=origin+new Vector3(Mathf.Sin(Time.time*moveSpeed+phase)*moveRadius,0,Mathf.Cos(Time.time*moveSpeed*.7f+phase)*moveRadius*.5f);Vector3 velocity=(patrol-transform.position);if(velocity.sqrMagnitude>.01f)velocity=velocity.normalized*moveSpeed;
            if(controller)controller.Move(velocity*Time.deltaTime);else MoveWithCollision(velocity*Time.deltaTime);
        }
    }
    void MoveWithCollision(Vector3 delta){if(delta.sqrMagnitude<=0f)return;Vector3 next=transform.position+delta;next.y=Mathf.Max(0,next.y);transform.position=next;}
    public bool Hit(int damage){
        health-=Mathf.Max(0,damage);if(body)StartCoroutine(HitFlash());if(VisualFX.Instance)VisualFX.Instance.Hit();if(AudioManager.Instance)AudioManager.Instance.Hit();
        if(health<=0){if(GameFlow.Instance)GameFlow.Instance.TargetKilled();if(MissionSystem.Instance)MissionSystem.Instance.Kill();if(ObjectiveSystem.Instance)ObjectiveSystem.Instance.Kill();if(VisualFX.Instance)VisualFX.Instance.Death(transform.position+Vector3.up*.8f);BossSystem b=BossSystem.Instance;if(b)b.NotifyDeath(this);Destroy(gameObject);return true;}return false;
    }
    IEnumerator HitFlash(){if(!body)yield break;body.material.color=Color.white;transform.localScale=baseScale*1.08f;yield return new WaitForSeconds(.07f);if(body)body.material.color=baseColor;transform.localScale=baseScale;}
}
