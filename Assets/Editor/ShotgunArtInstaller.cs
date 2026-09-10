#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class ShotgunArtInstaller
{
    private const string ArtRoot = "Assets/Art/Imported";
    private const string ResourcesRoot = "Assets/Resources";
    private const string GunZipUrl = "https://opengameart.org/sites/default/files/ultimate_gun_pack_by_quaternius.zip";
    private const string SciFiModelsZipUrl = "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_models.zip";
    private const string SciFiTexturesZipUrl = "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_textures.zip";
    private const string ApiBase = "https://api.polyhaven.com/files/";

    [MenuItem("Shotgun 3D/Art/Install CC0 Art")]
    public static void InstallAndPrepareMenu() => InstallAndPrepare();

    public static void InstallAndPrepare()
    {
        Directory.CreateDirectory(ArtRoot);
        Directory.CreateDirectory(ResourcesRoot);

        DownloadAndExtract("QuaterniusGuns", GunZipUrl);
        DownloadAndExtract("SciFiEssentialsModels", SciFiModelsZipUrl);
        DownloadAndExtract("SciFiEssentialsTextures", SciFiTexturesZipUrl);
        InstallPolyHaven("barrel_01", "Barrel_01");
        InstallPolyHaven("wooden_military_crate", "MilitaryCrate");

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        CreatePrefabFromModel("shotgun", "Assets/Art/Imported/QuaterniusGuns", "Assets/Resources/Weapons/Shotgun.prefab");
        CreatePrefabFromModel("barrel", "Assets/Art/Imported/Barrel_01", "Assets/Resources/Environment/Barrel.prefab");
        CreatePrefabFromModel("crate", "Assets/Art/Imported/MilitaryCrate", "Assets/Resources/Environment/Crate.prefab");
        CreatePrefabFromModel("robot", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Characters/Player.prefab");
        CreatePrefabFromModel("robot", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Characters/Enemy.prefab");
        CreatePrefabFromModel("screen", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Environment/Screen.prefab");
        CreatePrefabFromModel("door", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Environment/Door.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void InstallPolyHaven(string slug, string folderName)
    {
        string json = DownloadText(ApiBase + slug, "Shotgun3D-AssetInstaller/2.0");
        if (string.IsNullOrEmpty(json)) return;
        string url = ExtractUrlNear(json, "\"1k\"");
        if (string.IsNullOrEmpty(url)) url = ExtractZipUrl(json);
        if (!string.IsNullOrEmpty(url)) DownloadAndExtract(folderName, url);
    }

    private static string ExtractUrlNear(string json, string marker)
    {
        int markerIndex = json.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0) return null;
        int end = Mathf.Min(json.Length, markerIndex + 12000);
        return ExtractZipUrl(json.Substring(markerIndex, end - markerIndex));
    }

    private static string ExtractZipUrl(string json)
    {
        Match match = Regex.Match(json, "\\\"url\\\"\\s*:\\s*\\\"([^\\\"]+\\.zip)\\\"", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Replace("\\u0026", "&") : null;
    }

    private static string DownloadText(string url, string userAgent)
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = userAgent;
                return client.DownloadString(url);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("ShotgunArtInstaller download failed: " + url + "\n" + ex.Message);
            return null;
        }
    }

    private static void DownloadAndExtract(string folderName, string url)
    {
        string targetDir = Path.Combine(ArtRoot, folderName).Replace('\\', '/');
        string marker = Path.Combine(targetDir, ".installed");
        if (File.Exists(marker)) return;
        Directory.CreateDirectory(targetDir);
        string tempZip = Path.Combine(Path.GetTempPath(), "shotgun3d_" + folderName + ".zip");
        try
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = "Shotgun3D-AssetInstaller/2.0";
                Debug.Log("ShotgunArtInstaller: downloading " + folderName);
                client.DownloadFile(url, tempZip);
            }
            ZipFile.ExtractToDirectory(tempZip, targetDir, true);
            File.WriteAllText(marker, "Installed from: " + url + "\nUTC: " + DateTime.UtcNow.ToString("O"));
        }
        catch (Exception ex)
        {
            Debug.LogError("ShotgunArtInstaller failed for " + folderName + ": " + ex);
        }
        finally
        {
            try { if (File.Exists(tempZip)) File.Delete(tempZip); } catch { }
        }
    }

    private static void CreatePrefabFromModel(string keyword, string searchFolder, string prefabPath)
    {
        if (File.Exists(prefabPath)) return;
        string directory = Path.GetDirectoryName(prefabPath)?.Replace('\\', '/');
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { searchFolder });
        string selected = null;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string file = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
            if (file.Contains(keyword.ToLowerInvariant())) { selected = path; break; }
        }
        if (selected == null) return;
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(selected);
        if (source == null) return;
        GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (instance == null) return;
        instance.name = Path.GetFileNameWithoutExtension(prefabPath);
        try { PrefabUtility.SaveAsPrefabAsset(instance, prefabPath); }
        finally { UnityEngine.Object.DestroyImmediate(instance); }
    }
}
#endif
