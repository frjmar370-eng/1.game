using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public static GameFlow Instance { get; private set; }
    public Text roundText;
    public Text messageText;
    public float roundDuration = 60f;
    float timeLeft;
    int round = 1;
    int targetsAtStart;
    bool paused;

    void Awake() { Instance = this; }
    void Start()
    {
        timeLeft = roundDuration;
        targetsAtStart = FindObjectsOfType<TargetDummy>().Length;
        UpdateUI();
    }

    void Update()
    {
        if (paused) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) EndRound();
        UpdateUI();
    }

    public void TogglePause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        if (messageText) messageText.text = paused ? "متوقف مؤقتاً" : "";
    }

    void EndRound()
    {
        round++;
        timeLeft = roundDuration;
        SpawnRoundTargets(4 + round * 2);
        if (messageText) messageText.text = "الجولة " + round;
    }

    void SpawnRoundTargets(int count)
    {
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
