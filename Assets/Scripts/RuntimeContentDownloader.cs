using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RuntimeContentDownloader : MonoBehaviour
{
    [Serializable] public class Pack
    {
        public string name;
        public string url;
        public string fileName;
        public Pack(string n, string u, string f){name=n;url=u;fileName=f;}
    }

    public static bool Ready => PlayerPrefs.GetInt("SHOTGUN_CONTENT_READY", 0) == 1;
    public static void ResetContentState(){PlayerPrefs.DeleteKey("SHOTGUN_CONTENT_READY");PlayerPrefs.Save();}

    readonly Pack[] packs = {
        new Pack("بيئة Sci-Fi", "https://opengameart.org/sites/default/files/modular_scifi_megakitstandard.zip", "modular_scifi_megakitstandard.zip"),
        new Pack("نماذج الشخصيات والأسلحة", "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_models.zip", "sci-fi_essentials_kit_models.zip"),
        new Pack("الخامات والـTextures", "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_textures.zip", "sci-fi_essentials_kit_textures.zip")
    };

    Canvas canvas; Text title, detail, amount, speed, eta, status; Slider bar; Button retry;
    float startedAt; long downloadedAtStart; long totalBytes; long doneBytes; bool running;
    string root;

    public void Begin(Action finished)
    {
        root = Path.Combine(Application.persistentDataPath, "GameContent");
        Directory.CreateDirectory(root);
        BuildUI();
        StartCoroutine(DownloadAll(finished));
    }

    void BuildUI()
    {
        GameObject c = new GameObject("Content Download Canvas"); canvas = c.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler s=c.AddComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);c.AddComponent<GraphicRaycaster>();
        CreateText(c.transform,"تجهيز اللعبة",new Vector2(.5f,.74f),60,out title);
        CreateText(c.transform,"يتم تحميل ملفات اللعبة المطلوبة لأول تشغيل",new Vector2(.5f,.64f),28,out detail);
        CreateText(c.transform,"0 MB / 0 MB",new Vector2(.5f,.52f),34,out amount);
        CreateText(c.transform,"0.0 MB/s",new Vector2(.5f,.44f),25,out speed);
        CreateText(c.transform,"جارٍ الحساب...",new Vector2(.5f,.39f),23,out eta);
        CreateText(c.transform,"",new Vector2(.5f,.33f),24,out status);
        GameObject bo=new GameObject("Progress");bo.transform.SetParent(c.transform,false);RectTransform br=bo.AddComponent<RectTransform>();br.anchorMin=br.anchorMax=new Vector2(.5f,.48f);br.sizeDelta=new Vector2(1050,34);bar=bo.AddComponent<Slider>();bar.minValue=0;bar.maxValue=1;bar.value=0;
        retry=CreateButton(c.transform,"إعادة المحاولة",new Vector2(.5f,.23f));retry.gameObject.SetActive(false);retry.onClick.AddListener(()=>{retry.gameObject.SetActive(false);StartCoroutine(DownloadAll(null));});
    }

    IEnumerator DownloadAll(Action finished)
    {
        if(running)return;running=true;startedAt=Time.realtimeSinceStartup;downloadedAtStart=0;doneBytes=0;totalBytes=0;
        foreach(Pack p in packs){
            string path=Path.Combine(root,p.fileName);
            if(File.Exists(path) && new FileInfo(path).Length>1024*1024){doneBytes+=new FileInfo(path).Length;continue;}
            yield return DownloadPack(p,path);
            if(!File.Exists(path)){Fail("فشل تنزيل "+p.name);running=false;yield break;}
            doneBytes+=new FileInfo(path).Length;
        }
        PlayerPrefs.SetInt("SHOTGUN_CONTENT_READY",1);PlayerPrefs.Save();
        running=false;bar.value=1;amount.text=Format(doneBytes)+" / "+Format(Math.Max(totalBytes,doneBytes));speed.text=FormatSpeed(doneBytes/Math.Max(.1f,Time.realtimeSinceStartup-startedAt));eta.text="اكتمل التحميل";status.text="تم تجهيز ملفات اللعبة";
        yield return new WaitForSeconds(.35f);Destroy(canvas.gameObject);finished?.Invoke();
    }

    IEnumerator DownloadPack(Pack p,string path)
    {
        detail.text="تحميل: "+p.name;
        string temp=path+".part";long existing=File.Exists(temp)?new FileInfo(temp).Length:0;
        using(UnityWebRequest req=UnityWebRequest.Get(p.url)){
            req.downloadHandler=new DownloadHandlerFile(temp,true);
            if(existing>0) req.SetRequestHeader("Range","bytes="+existing+"-");
            yield return req.SendWebRequest();
            if(req.result!=UnityWebRequest.Result.Success){
                if(existing>0){try{File.Delete(temp);}catch{};yield return DownloadPack(p,path);yield break;}
                Fail(req.error);yield break;
            }
            long length=(long)req.downloadedBytes+existing;totalBytes+=length;
            try{if(File.Exists(path))File.Delete(path);File.Move(temp,path);}catch(Exception e){Fail(e.Message);yield break;}
            doneBytes+=0;
        }
    }

    void Update()
    {
        if(!running)return;
        long current=doneBytes;
        foreach(Pack p in packs){string path=Path.Combine(root,p.fileName+".part");if(File.Exists(path))current+=new FileInfo(path).Length;}
        float elapsed=Mathf.Max(.1f,Time.realtimeSinceStartup-startedAt);float mbps=current/1048576f/elapsed;speed.text=mbps.ToString("0.0")+" MB/s";
        if(totalBytes>0){bar.value=Mathf.Clamp01((float)current/totalBytes);amount.text=Format(current)+" / "+Format(totalBytes);float remaining=Mathf.Max(0,totalBytes-current);eta.text="متبقي تقريبًا "+TimeText(remaining/(mbps*1048576f));}
        status.text="جارٍ تنزيل الملفات...";
    }

    void Fail(string message){detail.text="تعذر تنزيل الملفات";status.text=message+"\nتأكد من الإنترنت ثم اضغط إعادة المحاولة";retry.gameObject.SetActive(true);}
    string Format(long b){return (b/1048576f).ToString("0.0")+" MB";}
    string FormatSpeed(double v){return (v/1048576d).ToString("0.0")+" MB/s";}
    string TimeText(float sec){if(float.IsInfinity(sec)||sec>86400)return "--";if(sec<60)return Mathf.CeilToInt(sec)+" ثانية";return Mathf.FloorToInt(sec/60)+" دقيقة";}

    void CreateText(Transform parent,string value,Vector2 a,int size,out Text text){GameObject o=new GameObject("Text");o.transform.SetParent(parent,false);RectTransform r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=a;r.sizeDelta=new Vector2(1500,90);text=o.AddComponent<Text>();text.text=value;text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");text.fontSize=size;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;}
    Button CreateButton(Transform parent,string label,Vector2 a){GameObject o=new GameObject("Button");o.transform.SetParent(parent,false);RectTransform r=o.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=a;r.sizeDelta=new Vector2(360,90);o.AddComponent<Image>();Button b=o.AddComponent<Button>();Text t=new GameObject("Label").AddComponent<Text>();t.transform.SetParent(o.transform,false);RectTransform tr=t.rectTransform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;t.text=label;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=30;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return b;}
}
