using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileHUD : MonoBehaviour
{
    public MobileInput moveStick;
    public MobileInput lookStick;
    public MobileActionButton fireButton;
    public MobileActionButton reloadButton;
    public Button pauseButton;
    public Button restartButton;

    public static MobileHUD Instance { get; private set; }
    public Vector2 Move => moveStick ? moveStick.Value : Vector2.zero;
    public Vector2 Look => lookStick ? lookStick.Value : Vector2.zero;
    public bool FireHeld => fireButton && fireButton.Held;
    public bool ReloadHeld => reloadButton && reloadButton.Held;

    void Awake() => Instance = this;

    public static MobileHUD Build(Canvas canvas)
    {
        GameObject root = new GameObject("Mobile HUD");
        root.transform.SetParent(canvas.transform, false);
        MobileHUD hud = root.AddComponent<MobileHUD>();
        hud.moveStick = CreateStick(root.transform, "Move Stick", new Vector2(0.16f, 0.18f), MobileInput.Mode.Move);
        hud.lookStick = CreateStick(root.transform, "Look Stick", new Vector2(0.84f, 0.18f), MobileInput.Mode.Look);
        hud.fireButton = CreateButton(root.transform, "FIRE", new Vector2(0.86f, 0.52f), MobileActionButton.Action.Fire, 92);
        hud.reloadButton = CreateButton(root.transform, "RELOAD", new Vector2(0.72f, 0.43f), MobileActionButton.Action.Reload, 72);
        hud.pauseButton = CreateUtilityButton(root.transform, "PAUSE", new Vector2(.92f,.92f), 68);
        hud.restartButton = CreateUtilityButton(root.transform, "RESTART", new Vector2(.50f,.38f), 150);
        hud.restartButton.gameObject.SetActive(false);
        if (GameFlow.Instance)
        {
            hud.pauseButton.onClick.AddListener(GameFlow.Instance.TogglePause);
            hud.restartButton.onClick.AddListener(GameFlow.Instance.RestartGame);
        }
        return hud;
    }

    public void ShowRestart()
    {
        if (restartButton) restartButton.gameObject.SetActive(true);
    }

    static MobileInput CreateStick(Transform parent, string name, Vector2 anchor, MobileInput.Mode mode)
    {
        GameObject baseObj = new GameObject(name);
        baseObj.transform.SetParent(parent, false);
        RectTransform rect = baseObj.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(150, 150);
        Image bg = baseObj.AddComponent<Image>();
        bg.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
        bg.color = new Color(1, 1, 1, 0.22f);
        MobileInput input = baseObj.AddComponent<MobileInput>();
        input.mode = mode;
        GameObject knob = new GameObject("Handle");
        knob.transform.SetParent(baseObj.transform, false);
        RectTransform k = knob.AddComponent<RectTransform>();
        k.anchorMin = k.anchorMax = new Vector2(.5f, .5f);
        k.sizeDelta = new Vector2(58, 58);
        Image ki = knob.AddComponent<Image>();
        ki.sprite = bg.sprite;
        ki.color = new Color(1, 1, 1, .55f);
        input.handle = k;
        return input;
    }

    static MobileActionButton CreateButton(Transform parent, string label, Vector2 anchor, MobileActionButton.Action action, float size)
    {
        GameObject obj = new GameObject(label + " Button");
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(size, size);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(.08f, .09f, .12f, .72f);
        MobileActionButton button = obj.AddComponent<MobileActionButton>();
        button.action = action;
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform tr = textObj.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero;
        Text text = textObj.AddComponent<Text>();
        text.text = label; text.alignment = TextAnchor.MiddleCenter; text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = action == MobileActionButton.Action.Fire ? 20 : 14; text.color = Color.white;
        return button;
    }

    static Button CreateUtilityButton(Transform parent, string label, Vector2 anchor, float size)
    {
        GameObject obj = new GameObject(label + " Button");
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = new Vector2(size, size * .55f);
        Image image = obj.AddComponent<Image>(); image.color = new Color(.05f,.06f,.08f,.78f);
        Button button = obj.AddComponent<Button>();
        GameObject textObj = new GameObject("Label"); textObj.transform.SetParent(obj.transform,false);
        RectTransform tr=textObj.AddComponent<RectTransform>(); tr.anchorMin=Vector2.zero; tr.anchorMax=Vector2.one; tr.offsetMin=tr.offsetMax=Vector2.zero;
        Text text=textObj.AddComponent<Text>(); text.text=label; text.alignment=TextAnchor.MiddleCenter; text.font=Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize=14; text.color=Color.white;
        return button;
    }
}
