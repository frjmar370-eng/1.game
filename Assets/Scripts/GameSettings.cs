using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }
    public float sensitivity=2.2f;
    public bool vibration=true;
    public bool highQuality=true;
    public bool autoReload=true;

    void Awake(){if(Instance!=null){Destroy(gameObject);return;}Instance=this;DontDestroyOnLoad(gameObject);Load();Apply();}
    public void Apply(){Application.targetFrameRate=60;QualitySettings.vSyncCount=0;QualitySettings.antiAliasing=highQuality?2:0;QualitySettings.shadows=highQuality?ShadowQuality.All:ShadowQuality.HardOnly;}
    public void Save(){PlayerPrefs.SetFloat("sensitivity",sensitivity);PlayerPrefs.SetInt("vibration",vibration?1:0);PlayerPrefs.SetInt("quality",highQuality?1:0);PlayerPrefs.SetInt("autoReload",autoReload?1:0);PlayerPrefs.Save();Apply();}
    void Load(){sensitivity=PlayerPrefs.GetFloat("sensitivity",2.2f);vibration=PlayerPrefs.GetInt("vibration",1)==1;highQuality=PlayerPrefs.GetInt("quality",1)==1;autoReload=PlayerPrefs.GetInt("autoReload",1)==1;}
}
