#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class RuntimeAssetBundleBuilder
{
    public const string OutputRoot = "Builds/Content/Android";
    public const string BundleName = "shotgun3d-content";

    static readonly string[] Prefabs =
    {
        "Assets/Resources/RealPacks/Characters/Player.prefab",
        "Assets/Resources/RealPacks/Characters/Enemy.prefab",
        "Assets/Resources/RealPacks/Characters/Boss.prefab",
        "Assets/Resources/RealPacks/Weapons/Shotgun.prefab",
        "Assets/Resources/RealPacks/Weapons/Rifle.prefab",
        "Assets/Resources/RealPacks/Weapons/Pistol.prefab",
        "Assets/Resources/RealPacks/Environment/Crate.prefab",
        "Assets/Resources/RealPacks/Environment/Barrel.prefab",
        "Assets/Resources/RealPacks/Environment/Floor.prefab",
        "Assets/Resources/RealPacks/Environment/Wall.prefab",
        "Assets/Resources/RealPacks/Environment/Door.prefab",
        "Assets/Resources/RealPacks/Environment/Cover.prefab"
    };

    [MenuItem("Shotgun 3D/3D Assets/Build Android Runtime Content", priority = 30)]
    public static void BuildAndroidContent()
    {
        Build(BuildTarget.Android);
    }

    public static string Build(BuildTarget target)
    {
        Directory.CreateDirectory(OutputRoot);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var assets = new List<string>();
        foreach (string path in Prefabs)
        {
            if (!File.Exists(path))
                throw new BuildFailedException("Runtime prefab missing: " + path);
            assets.Add(path);
        }

        var map = new AssetBundleBuild
        {
            assetBundleName = BundleName,
            assetNames = assets.ToArray()
        };

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
            OutputRoot,
            new[] { map },
            BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode,
            target);

        if (manifest == null)
            throw new BuildFailedException("Android runtime AssetBundle build returned null.");

        string bundlePath = Path.Combine(OutputRoot, BundleName);
        if (!File.Exists(bundlePath) || new FileInfo(bundlePath).Length == 0)
            throw new BuildFailedException("Runtime AssetBundle was not created: " + bundlePath);

        Debug.Log($"[RuntimeContent] Android bundle ready: {bundlePath} ({new FileInfo(bundlePath).Length} bytes)");
        return bundlePath;
    }

    public static void RemoveEmbeddedRuntimePrefabs()
    {
        string root = "Assets/Resources/RealPacks";
        if (Directory.Exists(root))
        {
            FileUtil.DeleteFileOrDirectory(root);
            FileUtil.DeleteFileOrDirectory(root + ".meta");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
    }
}
#endif
