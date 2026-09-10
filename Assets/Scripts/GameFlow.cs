using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }
    public Text roundText, messageText;
    public float roundDuration=60f;
    float timeLeft; int round=1; int aliveTargets; bool paused,transitioning; float nextRoundAt;
    GameObject enemyPrefab;

    void Awake(){
        if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}
        Instance=this;enemyPrefab=ArtAssetResolver.Enemy();
    }

    void Start(){
        EnsureSystems();
        ArenaBuilder3D arena=FindObjectOfType<ArenaBuilder3D>();if(arena==null)arena=gameObject.AddComponent<ArenaBuilder3D>();arena.Build();
        timeLeft=roundDuration;DifficultySystem.Instance.SetWave(round);
        SpawnRoundTargets(6);SpawnPickups(4);Invoke(nameof(RefreshEnemyPresentation),.2f);UpdateUI();
    }

    void EnsureSystems(){
        if(WeaponSystem.Instance==null)gameObject.AddComponent<WeaponSystem>();
        if(MissionSystem.Instance==null)gameObject.AddComponent<MissionSystem>();
        if(AudioManager.Instance==null)gameObject.AddComponent<AudioManager>();
        if(DifficultySystem.Instance==null)gameObject.AddComponent<DifficultySystem>();
        if(BossSystem.Instance==null)gameObject.AddComponent<BossSystem>();
        if(!FindObjectOfType<CombatPresentation3D>())gameObject.AddComponent<CombatPresentation3D>();
        if(!FindObjectOfType<AndroidGamePolish>())gameObject.AddComponent<AndroidGamePolish>();
        if(!FindObjectOfType<WeaponVisual3D>())gameObject.AddComponent<WeaponVisual3D>();
    }

    void Update(){
        if(paused)return;
        if(transitioning){if(Time.time>=nextRoundAt){transitioning=false;EndRound();}UpdateUI();return;}
        ShotgunGame player=FindObjectOfType<ShotgunGame>();if(player&&player.IsGameOver)return;
        timeLeft-=Time.deltaTime;if(timeLeft<=0f||aliveTargets<=0)BeginNextRound();UpdateUI();
    }

    public void TargetKilled(){aliveTargets=Mathf.Max(0,aliveTargets-1);if(aliveTargets==0&&!transitioning)BeginNextRound();}
    void BeginNextRound(){if(transitioning)return;transitioning=true;nextRoundAt=Time.time+1.2f;if(messageText)messageText.text="WAVE CLEAR!";if(AudioManager.Instance)AudioManager.Instance.Click();}
    public void TogglePause(){paused=!paused;Time.timeScale=paused?0f:1f;if(messageText)messageText.text=paused?"PAUSED":"";}
    public void RestartGame(){Time.timeScale=1f;SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}

    void EndRound(){
        round++;timeLeft=Mathf.Max(30f,roundDuration-round*1.5f);DifficultySystem.Instance.SetWave(round);
        SpawnRoundTargets(Mathf.Min(24,4+round*2));SpawnPickups(3+round%3);
        if(round%5==0&&BossSystem.Instance!=null)BossSystem.Instance.SpawnBoss(new Vector3(0,0,12),round);
        if(messageText)messageText.text=round%5==0?"BOSS WAVE":"ROUND "+round;if(AudioManager.Instance)AudioManager.Instance.Click();
    }

    void SpawnRoundTargets(int count){
        aliveTargets=count;float healthMul=DifficultySystem.Instance?DifficultySystem.Instance.enemyHealth:1f;float speedMul=DifficultySystem.Instance?DifficultySystem.Instance.enemySpeed:1f;float damageMul=DifficultySystem.Instance?DifficultySystem.Instance.enemyDamage:1f;
        for(int i=0;i<count;i++){
            Vector3 pos=new Vector3(Random.Range(-14f,14f),0,Random.Range(1f,17f));
            GameObject t=enemyPrefab?Instantiate(enemyPrefab,pos,Quaternion.identity):GameObject.CreatePrimitive(PrimitiveType.Capsule);
            t.name="Enemy_R"+round+"_"+i;if(!enemyPrefab){t.transform.position=new Vector3(pos.x,1.1f,pos.z);t.transform.localScale=Vector3.one*1.1f;}
            if(t.GetComponent<Collider>()==null)t.AddComponent<CapsuleCollider>();
            TargetDummy d=t.GetComponent<TargetDummy>();if(d==null)d=t.AddComponent<TargetDummy>();
            d.health=Mathf.RoundToInt((2+Mathf.Min(6,round/2))*healthMul);d.moveSpeed=(1.2f+round*.08f)*speedMul;d.damageToPlayer=Mathf.RoundToInt((8+Mathf.Min(14,round))*damageMul);d.attackInterval=Mathf.Max(.65f,2.5f-round*.08f)/speedMul;d.chaseDistance=Mathf.Min(32f,18f+round*.5f);
            EnemyVariants v=t.GetComponent<EnemyVariants>();if(v==null)v=t.AddComponent<EnemyVariants>();int roll=(i+round)%10;v.type=roll<6?EnemyType.Grunt:(roll<9?EnemyType.Runner:EnemyType.Tank);
            if(!t.GetComponent<EnemyVisualPolish>())t.AddComponent<EnemyVisualPolish>();CombatPresentation3D cp=FindObjectOfType<CombatPresentation3D>();if(cp)cp.Attach(d);
        }
    }

    void SpawnPickups(int count){
        for(int i=0;i<count;i++){
            GameObject prefab=i%3==0?ArtAssetResolver.Crate():null;GameObject o=prefab?Instantiate(prefab):GameObject.CreatePrimitive(PrimitiveType.Sphere);o.name="Pickup_"+round+"_"+i;
            o.transform.position=new Vector3(Random.Range(-14f,14f),prefab?.6f:.65f,Random.Range(-7f,17f));if(!prefab)o.transform.localScale=Vector3.one*.38f;
            Collider old=o.GetComponent<Collider>();if(old)Destroy(old);SphereCollider trigger=o.AddComponent<SphereCollider>();trigger.isTrigger=true;trigger.radius=.6f;PickupItem p=o.GetComponent<PickupItem>();if(!p)p=o.AddComponent<PickupItem>();p.kind=(i%3==0)?PickupItem.Kind.Health:PickupItem.Kind.Ammo;p.amount=p.kind==PickupItem.Kind.Health?20:2;
        }
    }

    void RefreshEnemyPresentation(){CombatPresentation3D cp=FindObjectOfType<CombatPresentation3D>();if(cp==null)return;foreach(TargetDummy d in FindObjectsOfType<TargetDummy>()){cp.Attach(d);if(!d.GetComponent<EnemyVisualPolish>())d.gameObject.AddComponent<EnemyVisualPolish>();}}
    void UpdateUI(){if(roundText)roundText.text="ROUND  "+round+"   "+Mathf.CeilToInt(Mathf.Max(0,timeLeft))+"   ENEMIES "+aliveTargets;}
}
