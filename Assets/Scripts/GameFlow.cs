using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }
    public Text roundText,messageText;
    public float roundDuration=60f;
    float timeLeft;int round=1;int aliveTargets;bool paused,transitioning;float nextRoundAt;GameObject enemyPrefab;
    void Awake(){Instance=this;enemyPrefab=Resources.Load<GameObject>("Characters/Enemy");}
    void Start(){timeLeft=roundDuration;aliveTargets=FindObjectsOfType<TargetDummy>().Length;SpawnPickups(4);UpdateUI();}
    void Update(){if(paused)return;if(transitioning){if(Time.time>=nextRoundAt){transitioning=false;EndRound();}UpdateUI();return;}ShotgunGame player=FindObjectOfType<ShotgunGame>();if(player&&player.IsGameOver)return;timeLeft-=Time.deltaTime;if(timeLeft<=0f||aliveTargets<=0)BeginNextRound();UpdateUI();}
    public void TargetKilled(){aliveTargets=Mathf.Max(0,aliveTargets-1);if(aliveTargets==0&&!transitioning)BeginNextRound();}
    void BeginNextRound(){if(transitioning)return;transitioning=true;nextRoundAt=Time.time+1.2f;if(messageText)messageText.text="WAVE CLEAR!";}
    public void TogglePause(){paused=!paused;Time.timeScale=paused?0f:1f;if(messageText)messageText.text=paused?"PAUSED":"";}
    public void RestartGame(){Time.timeScale=1f;SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
    void EndRound(){round++;timeLeft=Mathf.Max(30f,roundDuration-round*1.5f);SpawnRoundTargets(Mathf.Min(18,4+round*2));SpawnPickups(3+round%3);if(messageText)messageText.text="ROUND "+round;}
    void SpawnRoundTargets(int count){aliveTargets=count;for(int i=0;i<count;i++){Vector3 pos=new Vector3(Random.Range(-10f,10f),0,Random.Range(3f,11f));GameObject t=enemyPrefab?Instantiate(enemyPrefab,pos,Quaternion.identity):GameObject.CreatePrimitive(PrimitiveType.Capsule);t.name="Target_R"+round+"_"+i;if(!enemyPrefab){t.transform.position=new Vector3(pos.x,1.1f,pos.z);t.transform.localScale=Vector3.one*1.1f;}TargetDummy d=t.GetComponent<TargetDummy>();if(d==null)d=t.AddComponent<TargetDummy>();d.health=2+Mathf.Min(4,round/2);d.moveSpeed=1.2f+round*.08f;d.attackInterval=Mathf.Max(.9f,2.5f-round*.08f);}}
    void SpawnPickups(int count){for(int i=0;i<count;i++){GameObject o=GameObject.CreatePrimitive(PrimitiveType.Sphere);o.name="Pickup_"+round+"_"+i;o.transform.position=new Vector3(Random.Range(-10f,10f),.65f,Random.Range(-5f,10f));o.transform.localScale=Vector3.one*.38f;Destroy(o.GetComponent<Collider>());SphereCollider trigger=o.AddComponent<SphereCollider>();trigger.isTrigger=true;PickupItem p=o.AddComponent<PickupItem>();p.kind=(i%3==0)?PickupItem.Kind.Health:PickupItem.Kind.Ammo;p.amount=p.kind==PickupItem.Kind.Health?20:2;}}
    void UpdateUI(){if(roundText)roundText.text="ROUND  "+round+"   "+Mathf.CeilToInt(Mathf.Max(0,timeLeft));}
}
