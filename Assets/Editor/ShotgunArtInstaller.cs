#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ShotgunArtInstaller
{
    private const string ModelRoot = "Assets/Art/Models";
    private const string ResourcesRoot = "Assets/Resources";

    [MenuItem("Shotgun 3D/Art/Build Local Art Prefabs")]
    public static void InstallAndPrepareMenu() => InstallAndPrepare();

    public static void InstallAndPrepare()
    {
        Directory.CreateDirectory(ResourcesRoot + "/Weapons");
        Directory.CreateDirectory(ResourcesRoot + "/Characters");
        Directory.CreateDirectory(ResourcesRoot + "/Environment");
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        CreatePrefabFromGlb("Player", ModelRoot + "/Player/Player.glb", ResourcesRoot + "/Characters/Player.prefab");
        CreatePrefabFromGlb("Enemy", ModelRoot + "/Enemy/Enemy.glb", ResourcesRoot + "/Characters/Enemy.prefab");
        CreatePrefabFromGlb("Shotgun", ModelRoot + "/Weapons/Shotgun.glb", ResourcesRoot + "/Weapons/Shotgun.prefab");
        CreatePrefabFromGlb("Crate", ModelRoot + "/Environment/Crate.glb", ResourcesRoot + "/Environment/Crate.prefab");
        CreatePrefabFromGlb("Barrel", ModelRoot + "/Environment/Barrel.glb", ResourcesRoot + "/Environment/Barrel.prefab");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreatePrefabFromGlb(string modelName, string glbPath, string prefabPath)
    {
        if (!File.Exists(glbPath))
        {
            Debug.LogWarning("Local GLB not found: " + glbPath);
            return;
        }

        string tempFolder = "Assets/Art/ImportedGLB/" + modelName;
        Directory.CreateDirectory(tempFolder);
        AssetDatabase.Refresh();

        GameObject instance = null;
        try
        {
            instance = LocalGlbImporter.Import(glbPath, tempFolder, modelName);
            if (instance == null) return;
            instance.name = modelName;
            string directory = Path.GetDirectoryName(prefabPath)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to import GLB " + glbPath + ": " + ex.Message);
        }
        finally
        {
            if (instance != null) Object.DestroyImmediate(instance);
        }
    }
}
#endif