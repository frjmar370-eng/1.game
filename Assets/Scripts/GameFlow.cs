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
    int round = 1;
    int aliveTargets;
    bool paused;
    bool transitioning;
    float nextRoundAt;

    void Awake() { Instance = this; }

    void Start()
    {
        timeLeft = roundDuration;
        aliveTargets = FindObjectsOfType<TargetDummy>().Length;
        UpdateUI();
    }

    void Update()
    {
        if (paused) return;
        if (transitioning)
        {
            if (Time.time >= nextRoundAt) { transitioning = false; EndRound(); }
            UpdateUI();
            return;
        }
        ShotgunGame player = FindObjectOfType<ShotgunGame>();
        if (player && player.IsGameOver) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f || aliveTargets <= 0) BeginNextRound();
        UpdateUI();
    }

    public void TargetKilled()
    {
        aliveTargets = Mathf.Max(0, aliveTargets - 1);
        if (aliveTargets == 0 && !transitioning) BeginNextRound();
    }

    void BeginNextRound()
    {
        if (transitioning) return;
        transitioning = true;
        nextRoundAt = Time.time + 1.2f;
        if (messageText) messageText.text = "WAVE CLEAR!";
    }

    public void TogglePause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        if (messageText) messageText.text = paused ? "PAUSED" : "";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void EndRound()
    {
        round++;
        timeLeft = roundDuration;
        SpawnRoundTargets(4 + round * 2);
        if (messageText) messageText.text = "ROUND " + round;
    }

    void SpawnRoundTargets(int count)
    {
        aliveTargets = count;
        for (int i = 0; i < count; i++)
        {
            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            t.name = "Target_R" + round + "_" + i;
            t.transform.position = new Vector3(Random.Range(-8f, 8f), 1.1f, Random.Range(2f, 10f));
            t.transform.localScale = Vector3.one * 1.1f;
            Renderer r = t.GetComponent<Renderer>();
            r.material = MakeMaterial(new Color(.9f, .16f, .08f));
            t.AddComponent<TargetDummy>();
        }
    }

    void UpdateUI()
    {
        if (roundText) roundText.text = "ROUND  " + round + "   " + Mathf.CeilToInt(Mathf.Max(0, timeLeft));
    }

    Material MakeMaterial(Color color)
    {
        Material m = new Material(Shader.Find("Standard"));
        m.color = color;
        return m;
    }
}
