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

        CreatePrefabFromModel("Player", ModelRoot + "/Player", ResourcesRoot + "/Characters/Player.prefab");
        CreatePrefabFromModel("Enemy", ModelRoot + "/Enemy", ResourcesRoot + "/Characters/Enemy.prefab");
        CreatePrefabFromModel("Shotgun", ModelRoot + "/Weapons", ResourcesRoot + "/Weapons/Shotgun.prefab");
        CreatePrefabFromModel("Crate", ModelRoot + "/Environment", ResourcesRoot + "/Environment/Crate.prefab");
        CreatePrefabFromModel("Barrel", ModelRoot + "/Environment", ResourcesRoot + "/Environment/Barrel.prefab");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreatePrefabFromModel(string modelName, string searchFolder, string prefabPath)
    {
        string directory = Path.GetDirectoryName(prefabPath)?.Replace('\\', '/');
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { searchFolder });
        string selected = null;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path).Equals(modelName, System.StringComparison.OrdinalIgnoreCase))
            {
                selected = path;
                break;
            }
        }

        if (selected == null)
        {
            Debug.LogWarning("Local model not found: " + modelName + " in " + searchFolder);
            return;
        }

        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(selected);
        if (source == null)
        {
            Debug.LogWarning("Could not import model: " + selected);
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (instance == null) return;

        instance.name = modelName;
        try
        {
            if (instance.GetComponentInChildren<Collider>() == null)
            {
                MeshRenderer renderer = instance.GetComponentInChildren<MeshRenderer>();
                if (renderer != null)
                {
                    BoxCollider box = instance.AddComponent<BoxCollider>();
                    box.center = instance.transform.InverseTransformPoint(renderer.bounds.center);
                    box.size = instance.transform.InverseTransformVector(renderer.bounds.size);
                }
            }
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        }
        finally
        {
            Object.DestroyImmediate(instance);
        }
    }
}
#endif
