using System.IO;
using UnityEditor;
using UnityEngine;

public static class Real3DProjectPipeline
{
    [MenuItem("Shotgun 3D/3D Assets/BUILD COMPLETE REAL 3D GAME", priority=1)]
    public static void BuildComplete(){Directory.CreateDirectory(Path.GetFullPath("Assets/Art/Downloads"));Directory.CreateDirectory(Path.GetFullPath("Assets/Art/Packs"));Directory.CreateDirectory(Path.GetFullPath("Assets/Resources/RealPacks"));AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);Real3DPrefabBuilder.Build();Real3DAnimationBuilder.Build();Validate(true);}
    [MenuItem("Shotgun 3D/3D Assets/Validate Real 3D Setup", priority=2)]
    public static void Validate(){Validate(false);}
    public static void Validate(bool failOnMissing){
        string[] required={"Assets/Scenes/Main.unity","Assets/Resources/RealPacks/Characters/Player.prefab","Assets/Resources/RealPacks/Characters/Enemy.prefab","Assets/Resources/RealPacks/Characters/Boss.prefab","Assets/Resources/RealPacks/Weapons/Shotgun.prefab","Assets/Resources/RealPacks/Weapons/Rifle.prefab","Assets/Resources/RealPacks/Weapons/Pistol.prefab","Assets/Resources/RealPacks/Environment/Crate.prefab","Assets/Resources/RealPacks/Environment/Barrel.prefab","Assets/Resources/RealPacks/Environment/Floor.prefab","Assets/Resources/RealPacks/Environment/Wall.prefab","Assets/Resources/RealPacks/Environment/Door.prefab","Assets/Resources/RealPacks/Environment/Cover.prefab","Assets/Resources/RealPacks/Animations/Player.controller","Assets/Resources/RealPacks/Animations/Enemy.controller"};
        int ok=0;foreach(string p in required){bool exists=File.Exists(Path.GetFullPath(p));if(exists)ok++;Debug.Log((exists?"OK   ":"MISS ")+p);}
        int models=AssetDatabase.FindAssets("t:Model",new[]{"Assets/Art/Packs"}).Length;int anims=AssetDatabase.FindAssets("t:AnimationClip",new[]{"Assets/Art/Packs"}).Length;
        string scenePath="Assets/Scenes/Main.unity";bool sceneOk=File.Exists(scenePath);string report=$"REAL 3D VALIDATION: required {ok}/{required.Length}, source models {models}, animation clips {anims}, main scene {sceneOk}";Debug.Log(report);
        bool valid=ok==required.Length&&models>0;
        if(failOnMissing&&!valid)throw new BuildFailedException(report+". Real 3D assets are missing; refusing to build the primitive fallback APK.");
        if(!Application.isBatchMode)EditorUtility.DisplayDialog("Real 3D Validation",$"Required: {ok}/{required.Length}\nSource models: {models}\nAnimation clips: {anims}\nMain scene: {(sceneOk?"OK":"MISSING")}\n\nراجع Console للتفاصيل.","حسناً");
    }
}
