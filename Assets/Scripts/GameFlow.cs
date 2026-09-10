using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }
    public Text roundText;
    public Text messageText;
    public float roundDuration = 60f;
    float timeLeft;
    int round=1;
    int aliveTargets;
    bool paused,transitioning;
    float nextRoundAt;
    GameObject enemyPrefab;

    void Awake(){Instance=this;enemyPrefab=Resources.Load<GameObject>("Characters/Enemy");}
    void Start(){timeLeft=roundDuration;aliveTargets=FindObjectsOfType<TargetDummy>().Length;UpdateUI();}

    void Update()
    {
        if(paused)return;
        if(transitioning){if(Time.time>=nextRoundAt){transitioning=false;EndRound();}UpdateUI();return;}
        ShotgunGame player=FindObjectOfType<ShotgunGame>();if(player&&player.IsGameOver)return;
        timeLeft-=Time.deltaTime;if(timeLeft<=0f||aliveTargets<=0)BeginNextRound();UpdateUI();
    }

    public void TargetKilled(){aliveTargets=Mathf.Max(0,aliveTargets-1);if(aliveTargets==0&&!transitioning)BeginNextRound();}
    void BeginNextRound(){if(transitioning)return;transitioning=true;nextRoundAt=Time.time+1.2f;if(messageText)messageText.text="WAVE CLEAR!";}
    public void TogglePause(){paused=!paused;Time.timeScale=paused?0f:1f;if(messageText)messageText.text=paused?"PAUSED":"";}
    public void RestartGame(){Time.timeScale=1f;SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
    void EndRound(){round++;timeLeft=roundDuration;SpawnRoundTargets(4+round*2);if(messageText)messageText.text="ROUND "+round;}

    void SpawnRoundTargets(int count)
    {
        aliveTargets=count;
        for(int i=0;i<count;i++)
        {
            Vector3 pos=new Vector3(Random.Range(-10f,10f),0,Random.Range(3f,11f));
            GameObject t=enemyPrefab?Instantiate(enemyPrefab,pos,Quaternion.identity):GameObject.CreatePrimitive(PrimitiveType.Capsule);
            t.name="Target_R"+round+"_"+i;
            if(!enemyPrefab){t.transform.position=new Vector3(pos.x,1.1f,pos.z);t.transform.localScale=Vector3.one*1.1f;}
            if(t.GetComponent<TargetDummy>()==null)t.AddComponent<TargetDummy>();
        }
    }

    void UpdateUI(){if(roundText)roundText.text="ROUND  "+round+"   "+Mathf.CeilToInt(Mathf.Max(0,timeLeft));}
}
