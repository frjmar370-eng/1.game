using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

public static class Real3DAssetInstaller
{
    const string Root = "Assets/Art/Packs";
    const string DownloadRoot = "Assets/Art/Downloads";

    [MenuItem("Shotgun 3D/3D Assets/Install Recommended Packs", priority = 10)]
    public static void InstallRecommendedPacks()
    {
        Directory.CreateDirectory(DownloadRoot);
        Directory.CreateDirectory(Root);
        AssetDatabase.Refresh();

        // Official Quaternius pages are used as the source of truth. The installer
        // intentionally opens the official download pages instead of embedding
        // fragile third-party mirrors into the project.
        var urls = new[]
        {
            "https://quaternius.com/packs/toonshootergamekit.html",
            "https://quaternius.com/packs/scifiessentialskit.html",
            "https://quaternius.com/packs/modularscifimegakit.html"
        };

        foreach (var url in urls) Application.OpenURL(url);

        EditorUtility.DisplayDialog(
            "3D packs ready",
            "تم فتح صفحات التحميل الرسمية للحزم المطلوبة. بعد تنزيل ملفات ZIP ضعها داخل:\n\nAssets/Art/Downloads\n\nثم استخدم Shotgun 3D > 3D Assets > Import Downloaded ZIPs.",
            "حسناً");
    }

    [MenuItem("Shotgun 3D/3D Assets/Import Downloaded ZIPs", priority = 11)]
    public static void ImportDownloadedZips()
    {
        Directory.CreateDirectory(DownloadRoot);
        Directory.CreateDirectory(Root);

        var zips = Directory.GetFiles(Path.GetFullPath(DownloadRoot), "*.zip", SearchOption.TopDirectoryOnly);
        if (zips.Length == 0)
        {
            EditorUtility.DisplayDialog("3D Assets", "لا توجد ملفات ZIP داخل Assets/Art/Downloads.", "حسناً");
            return;
        }

        int done = 0;
        foreach (var zip in zips)
        {
            try
            {
                string packName = Path.GetFileNameWithoutExtension(zip);
                string target = Path.Combine(Path.GetFullPath(Root), Safe(packName));
                Directory.CreateDirectory(target);
                ZipFile.ExtractToDirectory(zip, target, true);
                done++;
            }
            catch (Exception e)
            {
                Debug.LogError("3D pack import failed: " + zip + "\n" + e);
            }
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        EditorUtility.DisplayDialog("3D Assets", $"تم استيراد {done} حزمة 3D.\n\nالمكان: {Root}", "حسناً");
    }

    [MenuItem("Shotgun 3D/3D Assets/Open Asset Folder", priority = 12)]
    public static void OpenFolder()
    {
        Directory.CreateDirectory(Path.GetFullPath(Root));
        EditorUtility.RevealInFinder(Path.GetFullPath(Root));
    }

    static string Safe(string value)
    {
        foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
        return value.Trim();
    }
}
