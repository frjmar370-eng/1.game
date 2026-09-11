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
        bundle = AssetBundle.LoadFromFile(LocalPath);
        return bundle != null;
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
        string wanted = name.EndsWith(".prefab") ? name : name + ".prefab";
        foreach (string assetName in bundle.GetAllAssetNames())
        {
            string normalized = assetName.Replace('\\', '/');
            if (normalized.EndsWith("/" + wanted.ToLowerInvariant()))
                return bundle.LoadAsset<GameObject>(assetName);
        }
        return null;
    }
}
