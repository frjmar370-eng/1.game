using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// First-launch runtime content delivery. Unity imports the source FBX/ZIP files during CI,
// converts them to an Android AssetBundle, publishes that bundle as a GitHub Release asset,
// and the Android app downloads the ready-to-load bundle on first launch.
public class RuntimeContentDownloader : MonoBehaviour
{
    const string ReadyKey = "SHOTGUN_CONTENT_READY_VERSION";
    const string Repo = "frjmar370-eng/1.game";
    const string BundleName = "shotgun3d-content";
    Canvas canvas; Slider bar; Text detail, amount, speed, eta, status;
    bool running;

    public static bool Ready => PlayerPrefs.GetString(ReadyKey, "") == BuildStamp.Version && RuntimeAssetBundleStore.LoadCached();
    public static void ResetContentState()
    {
        PlayerPrefs.DeleteKey(ReadyKey);
        PlayerPrefs.Save();
        DeleteLocalFiles();
    }

    static string BuildId => BuildStamp.Version.Replace("BUILD ", "").Trim().ToLowerInvariant();
    static string ReleaseTag => "content-" + BuildId;
    static string BundleUrl => $"https://github.com/{Repo}/releases/download/{ReleaseTag}/{BundleName}";

    public void Begin(Action finished)
    {
        if (running) return;
        BuildUI();
        StartCoroutine(DownloadAndPrepare(finished));
    }

    IEnumerator DownloadAndPrepare(Action finished)
    {
        running = true;
        string path = RuntimeAssetBundleStore.LocalPath;
        string temp = path + ".part";
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // Android keeps app data when an APK is updated. Never reuse a bundle from another build.
        if (PlayerPrefs.GetString(ReadyKey, "") != BuildStamp.Version)
        {
            RuntimeAssetBundleStore.Unload();
            DeleteLocalFiles();
        }

        if (RuntimeAssetBundleStore.LoadCached())
        {
            MarkReady();
            Complete(finished);
            yield break;
        }

        long existing = 0;
        try { if (File.Exists(temp)) existing = new FileInfo(temp).Length; } catch { existing = 0; }
        Set(0f, existing > 0 ? "استئناف تنزيل ملفات اللعبة..." : "تنزيل محتوى اللعبة لأول مرة...");

        float started = Time.realtimeSinceStartup;
        bool success = false;
        for (int attempt = 0; attempt < 3 && !success; attempt++)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(BundleUrl))
            {
                req.timeout = 120;
                bool resume = existing > 0;
                if (resume) req.SetRequestHeader("Range", "bytes=" + existing + "-");
                req.downloadHandler = new DownloadHandlerFile(temp, resume);
                UnityWebRequestAsyncOperation op = req.SendWebRequest();

                while (!op.isDone)
                {
                    long bytes = existing + (long)req.downloadedBytes;
                    float seconds = Mathf.Max(.1f, Time.realtimeSinceStartup - started);
                    float bps = bytes / seconds;
                    float total = GetTotal(req, existing);
                    float progress = total > 0 ? bytes / total : 0f;
                    Set(progress, resume ? "استئناف تنزيل المحتوى..." : "تنزيل المحتوى...", bytes, total, bps);
                    yield return null;
                }

                if (req.result == UnityWebRequest.Result.Success)
                {
                    long finalLength = 0;
                    try { finalLength = new FileInfo(temp).Length; } catch { }
                    if (resume && req.responseCode == 200)
                    {
                        existing = 0;
                        try { File.Delete(temp); } catch { }
                        continue;
                    }
                    if (finalLength <= 0)
                    {
                        existing = 0;
                        try { File.Delete(temp); } catch { }
                        continue;
                    }
                    success = true;
                }
                else
                {
                    status.text = "فشل الاتصال. إعادة المحاولة...";
                    yield return new WaitForSeconds(1f);
                    try { if (File.Exists(temp)) existing = new FileInfo(temp).Length; } catch { }
                }
            }
        }

        if (!success)
        {
            running = false;
            detail.text = "تعذر تنزيل محتوى اللعبة";
            status.text = "تحقق من الإنترنت ثم اضغط PLAY مرة أخرى.";
            yield break;
        }

        Set(1f, "التحقق من ملفات اللعبة...");
        try
        {
            if (File.Exists(path)) File.Delete(path);
            File.Move(temp, path);
        }
        catch (Exception e)
        {
            running = false;
            detail.text = "تعذر حفظ المحتوى";
            status.text = e.Message;
            yield break;
        }

        if (!RuntimeAssetBundleStore.LoadCached())
        {
            running = false;
            detail.text = "ملف المحتوى غير صالح";
            status.text = "أعد المحاولة لتنزيل نسخة سليمة.";
            DeleteLocalFiles();
            yield break;
        }

        if (RuntimeAssetBundleStore.LoadPrefab("Player") == null || RuntimeAssetBundleStore.LoadPrefab("Enemy") == null || RuntimeAssetBundleStore.LoadPrefab("Shotgun") == null)
        {
            RuntimeAssetBundleStore.Unload();
            DeleteLocalFiles();
            running = false;
            detail.text = "محتوى اللعبة غير مكتمل";
            status.text = "أعد المحاولة لتنزيل الحزمة الصحيحة.";
            yield break;
        }

        MarkReady();
        Set(1f, "تم تنزيل محتوى اللعبة");
        amount.text = "المحتوى 100%";
        speed.text = "تم الحفظ على الجهاز";
        eta.text = "جاهز للتشغيل";
        Complete(finished);
    }

    static void DeleteLocalFiles()
    {
        try { if (File.Exists(RuntimeAssetBundleStore.LocalPath)) File.Delete(RuntimeAssetBundleStore.LocalPath); } catch { }
        try { if (File.Exists(RuntimeAssetBundleStore.LocalPath + ".part")) File.Delete(RuntimeAssetBundleStore.LocalPath + ".part"); } catch { }
    }

    static float GetTotal(UnityWebRequest req, long existing)
    {
        string length = req.GetResponseHeader("Content-Length");
        if (long.TryParse(length, out long n)) return n + (req.responseCode == 206 ? existing : 0);
        return 0;
    }

    void MarkReady()
    {
        PlayerPrefs.SetString(ReadyKey, BuildStamp.Version);
        PlayerPrefs.Save();
    }

    void Complete(Action finished)
    {
        running = false;
        StartCoroutine(FinishNextFrame(finished));
    }

    IEnumerator FinishNextFrame(Action finished)
    {
        yield return new WaitForSeconds(.25f);
        if (canvas) Destroy(canvas.gameObject);
        finished?.Invoke();
        Destroy(gameObject);
    }

    void BuildUI()
    {
        GameObject c = new GameObject("Content Download Canvas");
        canvas = c.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler s = c.AddComponent<CanvasScaler>(); s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; s.referenceResolution = new Vector2(1920,1080);
        c.AddComponent<GraphicRaycaster>();
        CreateText(c.transform,"تنزيل محتوى اللعبة",.74f,60,out detail);
        CreateText(c.transform,"يتم تنزيل المجسمات والأسلحة والبيئة لأول مرة فقط",.64f,28,out status);
        CreateText(c.transform,"0%",.52f,34,out amount);
        CreateText(c.transform,"--",.44f,25,out speed);
        CreateText(c.transform,"جارٍ التحضير...",.39f,23,out eta);
        GameObject p = new GameObject("Progress"); p.transform.SetParent(c.transform,false);
        RectTransform r = p.AddComponent<RectTransform>(); r.anchorMin=r.anchorMax=new Vector2(.5f,.48f); r.sizeDelta=new Vector2(1050,34);
        bar=p.AddComponent<Slider>(); bar.minValue=0; bar.maxValue=1; bar.value=0;
    }

    void Set(float value,string message,long bytes=0,long total=0,float bps=0)
    {
        if(bar) bar.value=Mathf.Clamp01(value);
        if(amount) amount.text=Mathf.RoundToInt(Mathf.Clamp01(value)*100f)+"%";
        if(detail) detail.text=message;
        if(status) status.text=total>0 ? FormatBytes(bytes)+" / "+FormatBytes(total) : "الاتصال بخادم المحتوى...";
        if(speed) speed.text=bps>0 ? FormatBytes((long)bps)+"/ث" : "--";
        if(eta && bps>0 && total>bytes) eta.text="متبقي "+FormatTime((total-bytes)/bps); else if(eta) eta.text="جارٍ التنزيل...";
    }

    static string FormatBytes(long n)
    {
        if(n<1024) return n+" B";
        if(n<1024*1024) return (n/1024f).ToString("0.0")+" KB";
        if(n<1024L*1024*1024) return (n/(1024f*1024f)).ToString("0.0")+" MB";
        return (n/(1024f*1024f*1024f)).ToString("0.00")+" GB";
    }
    static string FormatTime(float seconds)
    {
        if(seconds<60) return Mathf.CeilToInt(seconds)+" ث";
        int m=Mathf.FloorToInt(seconds/60); int s=Mathf.CeilToInt(seconds%60); return m+" د "+s+" ث";
    }

    void CreateText(Transform parent,string value,float y,int size,out Text text)
    {
        GameObject o=new GameObject("Text"); o.transform.SetParent(parent,false);
        RectTransform r=o.AddComponent<RectTransform>(); r.anchorMin=r.anchorMax=new Vector2(.5f,y); r.sizeDelta=new Vector2(1500,90);
        text=o.AddComponent<Text>(); text.text=value; text.font=Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize=size; text.alignment=TextAnchor.MiddleCenter; text.color=Color.white;
    }
}
