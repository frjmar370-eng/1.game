using System.IO;
using UnityEngine;

public static class RuntimeAssetBundleStore
{
    const string BundleName = "shotgun3d-content";
    static AssetBundle bundle;

    public static bool IsLoaded => bundle != null;
    public static string LocalPath => Path.Combine(Application.persistentDataPath, BundleName);

    public static bool LoadCached()
    {
        if (bundle != null) return true;
        if (!File.Exists(LocalPath)) return false;
        try
        {
            bundle = AssetBundle.LoadFromFile(LocalPath);
            if (bundle == null) return false;
            return true;
        }
        catch
        {
            bundle = null;
            return false;
        }
    }

    public static bool HasRequiredContent()
    {
        if (bundle == null && !LoadCached()) return false;
        foreach (string required in new[] { "Player", "Enemy", "Boss", "Shotgun", "Rifle", "Pistol", "Crate", "Barrel", "Floor", "Wall", "Door", "Cover" })
            if (LoadPrefab(required) == null) return false;
        return true;
    }

    public static void Unload(bool unloadAllLoadedObjects = false)
    {
        if (bundle == null) return;
        bundle.Unload(unloadAllLoadedObjects);
        bundle = null;
    }

    public static GameObject LoadPrefab(string name)
    {
        if (bundle == null && !LoadCached()) return null;
        string wanted = name.EndsWith(".prefab") ? name.ToLowerInvariant() : name.ToLowerInvariant() + ".prefab";
        foreach (string assetName in bundle.GetAllAssetNames())
        {
            string normalized = assetName.Replace('\\', '/').ToLowerInvariant();
            if (normalized.EndsWith("/" + wanted) || normalized == wanted)
                return bundle.LoadAsset<GameObject>(assetName);
        }
        return null;
    }
}
