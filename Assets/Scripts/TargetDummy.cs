using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    public int health=3;
    public float moveRadius=2.5f;
    public float moveSpeed=1.2f;
    public int damageToPlayer=8;
    public float attackInterval=2.5f;
    Vector3 origin;float phase;float nextAttack;Renderer body;Color baseColor;Vector3 baseScale;

    void Start(){origin=transform.position;phase=Random.value*6.28f;nextAttack=Time.time+Random.Range(1f,attackInterval);body=GetComponentInChildren<Renderer>();if(body){baseColor=body.material.color;baseScale=transform.localScale;}}
    void Update(){Vector3 p=origin;p.x+=Mathf.Sin(Time.time*moveSpeed+phase)*moveRadius;transform.position=p;ShotgunGame player=FindObjectOfType<ShotgunGame>();if(player){Vector3 flat=player.transform.position-transform.position;flat.y=0;if(flat.sqrMagnitude>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(flat),Time.deltaTime*5f);if(Time.time>=nextAttack&&flat.sqrMagnitude<18f*18f){nextAttack=Time.time+attackInterval+Random.Range(.25f,1f);player.TakeDamage(damageToPlayer);}}}
    public bool Hit(int damage){health-=damage;if(body)StartCoroutine(HitFlash());if(health<=0){if(GameFlow.Instance)GameFlow.Instance.TargetKilled();SpawnDeathFX();Destroy(gameObject);return true;}return false;}
    System.Collections.IEnumerator HitFlash(){if(!body)yield break;body.material.color=Color.white;transform.localScale=baseScale*1.08f;yield return new WaitForSeconds(.07f);if(body)body.material.color=baseColor;transform.localScale=baseScale;}
    void SpawnDeathFX(){GameObject fx=GameObject.CreatePrimitive(PrimitiveType.Sphere);fx.name="HitFX";fx.transform.position=transform.position+Vector3.up*.8f;fx.transform.localScale=Vector3.one*.35f;Renderer r=fx.GetComponent<Renderer>();if(r)r.material=MakeMaterial(new Color(1f,.55f,.08f));Destroy(fx.GetComponent<Collider>());Destroy(fx,.12f);}
    Material MakeMaterial(Color color){Shader shader=Shader.Find("Standard");if(shader==null)shader=Shader.Find("Unlit/Color");if(shader==null)return null;Material m=new Material(shader);m.color=color;return m;}
}
