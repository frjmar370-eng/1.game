using UnityEngine;
using UnityEngine.UI;

public class MobileHUD : MonoBehaviour
{
    public MobileInput moveStick,lookStick;
    public MobileActionButton fireButton,reloadButton;
    public Button switchButton,pauseButton,restartButton;
    public static MobileHUD Instance{get;private set;}
    public Vector2 Move=>moveStick?moveStick.Value:Vector2.zero;
    public Vector2 Look=>lookStick?lookStick.Value:Vector2.zero;
    public bool FireHeld=>fireButton&&fireButton.Held;
    public bool ReloadHeld=>reloadButton&&reloadButton.Held;
    void Awake(){Instance=this;}
    public static MobileHUD Build(Canvas canvas){
        GameObject root=new GameObject("Mobile HUD");root.transform.SetParent(canvas.transform,false);MobileHUD hud=root.AddComponent<MobileHUD>();
        hud.moveStick=CreateStick(root.transform,"Move Stick",new Vector2(.16f,.18f),MobileInput.Mode.Move);
        hud.lookStick=CreateStick(root.transform,"Look Stick",new Vector2(.84f,.18f),MobileInput.Mode.Look);
        hud.fireButton=CreateButton(root.transform,"FIRE",new Vector2(.86f,.52f),MobileActionButton.Action.Fire,92);
        hud.reloadButton=CreateButton(root.transform,"RELOAD",new Vector2(.72f,.43f),MobileActionButton.Action.Reload,72);
        hud.switchButton=CreateUtilityButton(root.transform,"WEAPON",new Vector2(.72f,.58f),82);
        hud.pauseButton=CreateUtilityButton(root.transform,"PAUSE",new Vector2(.92f,.92f),68);
        hud.restartButton=CreateUtilityButton(root.transform,"RESTART",new Vector2(.50f,.38f),150);hud.restartButton.gameObject.SetActive(false);
        hud.switchButton.onClick.AddListener(()=>{ShotgunGame p=Object.FindObjectOfType<ShotgunGame>();if(p)p.SwitchWeapon();});
        if(GameFlow.Instance){hud.pauseButton.onClick.AddListener(GameFlow.Instance.TogglePause);hud.restartButton.onClick.AddListener(GameFlow.Instance.RestartGame);}return hud;
    }
    public void ShowRestart(){if(restartButton)restartButton.gameObject.SetActive(true);}
    static MobileInput CreateStick(Transform parent,string name,Vector2 anchor,MobileInput.Mode mode){GameObject o=new GameObject(name);o.transform.SetParent(parent,false);RectTransform r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.sizeDelta=new Vector2(150,150);Image bg=o.AddComponent<Image>();bg.sprite=Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");bg.color=new Color(1,1,1,.22f);MobileInput i=o.AddComponent<MobileInput>();i.mode=mode;GameObject k=new GameObject("Handle");k.transform.SetParent(o.transform,false);RectTransform kr=k.AddComponent<RectTransform>();kr.anchorMin=kr.anchorMax=new Vector2(.5f,.5f);kr.sizeDelta=new Vector2(58,58);Image ki=k.AddComponent<Image>();ki.sprite=bg.sprite;ki.color=new Color(1,1,1,.55f);i.handle=kr;return i;}
    static MobileActionButton CreateButton(Transform parent,string label,Vector2 anchor,MobileActionButton.Action action,float size){GameObject o=new GameObject(label+" Button");o.transform.SetParent(parent,false);RectTransform r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.sizeDelta=new Vector2(size,size);Image im=o.AddComponent<Image>();im.color=new Color(.08f,.09f,.12f,.72f);MobileActionButton b=o.AddComponent<MobileActionButton>();b.action=action;GameObject t=new GameObject("Label");t.transform.SetParent(o.transform,false);RectTransform tr=t.AddComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;Text tx=t.AddComponent<Text>();tx.text=label;tx.alignment=TextAnchor.MiddleCenter;tx.font=Resources.GetBuiltinResource<Font>("Arial.ttf");tx.fontSize=action==MobileActionButton.Action.Fire?20:14;tx.color=Color.white;return b;}
    static Button CreateUtilityButton(Transform parent,string label,Vector2 anchor,float size){GameObject o=new GameObject(label+" Button");o.transform.SetParent(parent,false);RectTransform r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.sizeDelta=new Vector2(size,size*.55f);Image im=o.AddComponent<Image>();im.color=new Color(.05f,.06f,.08f,.78f);Button b=o.AddComponent<Button>();GameObject t=new GameObject("Label");t.transform.SetParent(o.transform,false);RectTransform tr=t.AddComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;Text tx=t.AddComponent<Text>();tx.text=label;tx.alignment=TextAnchor.MiddleCenter;tx.font=Resources.GetBuiltinResource<Font>("Arial.ttf");tx.fontSize=13;tx.color=Color.white;return b;}
}