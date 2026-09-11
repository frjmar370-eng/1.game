using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Runtime content gate. The real 3D assets are imported and packed into the APK by the
// editor pipeline; raw FBX/ZIP source packs must never be downloaded and treated as runtime assets.
public class RuntimeContentDownloader : MonoBehaviour
{
    const string ReadyKey = "SHOTGUN_CONTENT_READY_VERSION";
    Canvas canvas; Slider bar; Text detail, amount, speed, eta, status;
    bool running;

    public static bool Ready => PlayerPrefs.GetString(ReadyKey, "") == BuildStamp.Version && HasRequiredRuntimeContent();
    public static void ResetContentState(){PlayerPrefs.DeleteKey(ReadyKey);PlayerPrefs.Save();}

    static bool HasRequiredRuntimeContent()
    {
        // These are the runtime Resources produced by Real3DPrefabBuilder.
        return Resources.Load<GameObject>("RealPacks/Characters/Player") != null
            || Resources.Load<GameObject>("RealPacks/Characters/PlayerCharacter") != null
            || Resources.Load<GameObject>("Characters/Player") != null;
    }

    public void Begin(Action finished)
    {
        if (running) return;
        BuildUI();
        StartCoroutine(Prepare(finished));
    }

    IEnumerator Prepare(Action finished)
    {
        running = true;
        float start = Time.realtimeSinceStartup;
        Set(0f, "فحص ملفات اللعبة المدمجة...");
        yield return null;

        bool player = HasRequiredRuntimeContent();
        Set(.35f, "فحص مجسم اللاعب...");
        yield return null;
        bool enemy = Resources.Load<GameObject>("RealPacks/Characters/Enemy") != null
                  || Resources.Load<GameObject>("RealPacks/Characters/EnemyCharacter") != null
                  || Resources.Load<GameObject>("Characters/Enemy") != null;
        Set(.65f, "فحص مجسمات الأعداء...");
        yield return null;
        bool weapon = Resources.Load<GameObject>("RealPacks/Weapons/Shotgun") != null
                   || Resources.Load<GameObject>("Weapons/Shotgun") != null;
        Set(.85f, "فحص الأسلحة والبيئة...");
        yield return null;

        if (!player || !enemy || !weapon)
        {
            running = false;
            detail.text = "محتوى اللعبة غير مكتمل";
            status.text = "البناء الحالي لم يضم كل أصول 3D المطلوبة.\nأعد بناء APK من GitHub Actions.";
            return;
        }

        PlayerPrefs.SetString(ReadyKey, BuildStamp.Version);
        PlayerPrefs.Save();
        Set(1f, "تم تجهيز المحتوى");
        amount.text = "المحتوى 100% داخل اللعبة";
        speed.text = ((1f / Mathf.Max(.1f, Time.realtimeSinceStartup - start))).ToString("0.0") + " فحص/ثانية";
        eta.text = "جاهز للتشغيل";
        status.text = "لا يوجد تنزيل ZIPات غير قابلة للتشغيل أثناء اللعب";
        running = false;
        yield return new WaitForSeconds(.2f);
        if (canvas) Destroy(canvas.gameObject);
        finished?.Invoke();
        Destroy(gameObject);
    }

    void BuildUI()
    {
        GameObject c = new GameObject("Content Preparation Canvas");
        canvas = c.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler s = c.AddComponent<CanvasScaler>(); s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; s.referenceResolution = new Vector2(1920,1080);
        c.AddComponent<GraphicRaycaster>();
        CreateText(c.transform,"تجهيز اللعبة",.74f,60,out detail);
        CreateText(c.transform,"يتم تجهيز المحتوى الحقيقي المدمج مع APK",.64f,28,out status);
        CreateText(c.transform,"0%",.52f,34,out amount);
        CreateText(c.transform,"--",.44f,25,out speed);
        CreateText(c.transform,"جارٍ الفحص...",.39f,23,out eta);
        GameObject p = new GameObject("Progress"); p.transform.SetParent(c.transform,false);
        RectTransform r = p.AddComponent<RectTransform>(); r.anchorMin=r.anchorMax=new Vector2(.5f,.48f); r.sizeDelta=new Vector2(1050,34);
        bar=p.AddComponent<Slider>(); bar.minValue=0; bar.maxValue=1; bar.value=0;
    }

    void Set(float value,string message)
    {
        if(bar) bar.value=value;
        if(amount) amount.text=Mathf.RoundToInt(value*100f)+"%";
        if(detail) detail.text=message;
        if(status) status.text="المحتوى الحقيقي مدمج داخل APK";
    }

    void CreateText(Transform parent,string value,float y,int size,out Text text)
    {
        GameObject o=new GameObject("Text"); o.transform.SetParent(parent,false);
        RectTransform r=o.AddComponent<RectTransform>(); r.anchorMin=r.anchorMax=new Vector2(.5f,y); r.sizeDelta=new Vector2(1500,90);
        text=o.AddComponent<Text>(); text.text=value; text.font=Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize=size; text.alignment=TextAnchor.MiddleCenter; text.color=Color.white;
    }
}
