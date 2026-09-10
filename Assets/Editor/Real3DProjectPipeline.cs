using System.IO;
using UnityEditor;
using UnityEngine;

public static class Real3DProjectPipeline
{
    [MenuItem("Shotgun 3D/3D Assets/BUILD COMPLETE REAL 3D GAME", priority=1)]
    public static void BuildComplete()
    {
        Directory.CreateDirectory(Path.GetFullPath("Assets/Art/Downloads"));
        Directory.CreateDirectory(Path.GetFullPath("Assets/Art/Packs"));
        Directory.CreateDirectory(Path.GetFullPath("Assets/Resources/RealPacks"));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Real3DPrefabBuilder.Build();
        Real3DAnimationBuilder.Build();
        Validate();
    }

    [MenuItem("Shotgun 3D/3D Assets/Validate Real 3D Setup", priority=2)]
    public static void Validate()
    {
        string[] required={
            "Assets/Resources/RealPacks/Characters/Player.prefab",
            "Assets/Resources/RealPacks/Characters/Enemy.prefab",
            "Assets/Resources/RealPacks/Weapons/Shotgun.prefab",
            "Assets/Resources/RealPacks/Environment/Crate.prefab",
            "Assets/Resources/RealPacks/Environment/Barrel.prefab"};
        int ok=0;
        foreach(string p in required){bool exists=File.Exists(Path.GetFullPath(p));if(exists)ok++;Debug.Log((exists?"OK   ":"MISS ")+p);}
        int models=AssetDatabase.FindAssets("t:Model",new[]{"Assets/Art/Packs"}).Length;
        int anims=AssetDatabase.FindAssets("t:AnimationClip",new[]{"Assets/Art/Packs"}).Length;
        Debug.Log($"REAL 3D VALIDATION: prefabs {ok}/{required.Length}, source models {models}, animation clips {anims}");
        EditorUtility.DisplayDialog("Real 3D Validation",$"Prefabs: {ok}/{required.Length}\nSource models: {models}\nAnimation clips: {anims}\n\nراجع Console للتفاصيل.","حسناً");
    }
}
