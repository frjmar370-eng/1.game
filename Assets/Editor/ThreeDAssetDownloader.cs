#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ThreeDAssetDownloader : EditorWindow
{
    [Serializable]
    private class Pack
    {
        public string name;
        public string url;
        public string zipName;
        public string installFolder;
        public bool enabled = true;
        public bool downloaded;
    }

    private static readonly Pack[] Packs =
    {
        new Pack
        {
            name = "Quaternius Modular Sci-Fi MegaKit — Standard",
            url = "https://opengameart.org/sites/default/files/modular_scifi_megakitstandard.zip",
            zipName = "modular_scifi_megakitstandard.zip",
            installFolder = "ModularSciFiMegaKit"
        },
        new Pack
        {
            name = "Quaternius Sci-Fi Essentials Kit — Models",
            url = "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_models.zip",
            zipName = "sci-fi_essentials_kit_models.zip",
            installFolder = "SciFiEssentialsKit/Models"
        },
        new Pack
        {
            name = "Quaternius Sci-Fi Essentials Kit — Textures",
            url = "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_textures.zip",
            zipName = "sci-fi_essentials_kit_textures.zip",
            installFolder = "SciFiEssentialsKit/Textures"
        }
    };

    private Vector2 scroll;
    private string status = "جاهز لتنزيل المجسمات الحقيقية";
    private bool busy;

    private static string DownloadsRoot => Path.Combine(Application.dataPath, "Art", "Downloads");
    private static string PacksRoot => Path.Combine(Application.dataPath, "Art", "Packs");

    [MenuItem("Shotgun 3D/3D Asset Downloader", priority = 20)]
    public static void Open()
    {
        GetWindow<ThreeDAssetDownloader>("3D Asset Downloader").minSize = new Vector2(620, 420);
    }

    [MenuItem("Shotgun 3D/Download & Install 3D Packs", priority = 21)]
    public static void DownloadAllMenu()
    {
        GetWindow<ThreeDAssetDownloader>("3D Asset Downloader").DownloadSelected();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("3D GAME ASSETS", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("تنزيل المجسمات مرة واحدة داخل المشروع — بدون تنزيل أثناء تشغيل اللعبة.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(8);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var p in Packs)
        {
            EditorGUILayout.BeginVertical("box");
            p.enabled = EditorGUILayout.ToggleLeft(p.name, p.enabled);
            EditorGUILayout.LabelField("المصدر", p.url, EditorStyles.miniLabel);
            EditorGUILayout.LabelField("الحالة", File.Exists(Path.Combine(DownloadsRoot, p.zipName)) ? "ZIP موجود" : "غير منزّل");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(status, MessageType.Info);
        using (new EditorGUI.DisabledScope(busy))
        {
            if (GUILayout.Button("تنزيل وتثبيت المجسمات الآن", GUILayout.Height(42)))
                DownloadSelected();
        }
        if (GUILayout.Button("فتح مجلد Art/Packs"))
        {
            Directory.CreateDirectory(PacksRoot);
            EditorUtility.RevealInFinder(PacksRoot);
        }
    }

    private void DownloadSelected()
    {
        if (busy) return;
        Directory.CreateDirectory(DownloadsRoot);
        Directory.CreateDirectory(PacksRoot);
        busy = true;
        status = "بدء تنزيل حزم 3D...";
        Repaint();

        try
        {
            foreach (var pack in Packs)
            {
                if (!pack.enabled) continue;
                DownloadAndExtract(pack);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            status = "تم تنزيل وتثبيت حزم 3D داخل Assets/Art/Packs. افتح Unity وانتظر انتهاء الاستيراد.";
        }
        catch (Exception ex)
        {
            status = "فشل: " + ex.Message;
            Debug.LogException(ex);
        }
        finally
        {
            busy = false;
            Repaint();
        }
    }

    private static void DownloadAndExtract(Pack pack)
    {
        string zipPath = Path.Combine(DownloadsRoot, pack.zipName);
        string target = Path.Combine(PacksRoot, pack.installFolder);
        Directory.CreateDirectory(Path.GetDirectoryName(zipPath));
        Directory.CreateDirectory(target);

        if (!File.Exists(zipPath))
        {
            using (var request = UnityWebRequest.Get(pack.url))
            {
                request.downloadHandler = new DownloadHandlerFile(zipPath);
                var op = request.SendWebRequest();
                while (!op.isDone)
                    EditorUtility.DisplayProgressBar("3D Asset Downloader", "Downloading " + pack.name, request.downloadProgress);
                EditorUtility.ClearProgressBar();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    if (File.Exists(zipPath)) File.Delete(zipPath);
                    throw new InvalidOperationException(pack.name + " download failed: " + request.error);
                }
            }
        }

        string marker = Path.Combine(target, ".installed");
        if (!File.Exists(marker))
        {
            EditorUtility.DisplayProgressBar("3D Asset Downloader", "Extracting " + pack.name, 0.5f);
            ZipFile.ExtractToDirectory(zipPath, target, true);
            File.WriteAllText(marker, DateTime.UtcNow.ToString("O"));
            EditorUtility.ClearProgressBar();
        }
    }
}
#endif
