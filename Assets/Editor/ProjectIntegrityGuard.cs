#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ProjectIntegrityGuard
{
    const string ScenePath = "Assets/Scenes/Main.unity";
    static ProjectIntegrityGuard()
    {
        EditorApplication.delayCall += Ensure;
    }

    [MenuItem("Shotgun 3D/Validate Project Integrity")]
    public static void Ensure()
    {
        Directory.CreateDirectory("Assets/Scenes");
        Directory.CreateDirectory("Assets/Art/Downloads");
        Directory.CreateDirectory("Assets/Art/Packs");
        Directory.CreateDirectory("Assets/Resources/RealPacks");
        Directory.CreateDirectory("Builds");

        if (!File.Exists(ScenePath))
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject bootstrap = new GameObject("Scene Bootstrap");
            bootstrap.AddComponent<SceneBootstrap>();
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        PlayerSettings.productName = "Shotgun 3D";
        PlayerSettings.companyName = "Frjmar";
        PlayerSettings.applicationIdentifier = "com.ammar.game";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)35;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Shotgun 3D project integrity validated.");
    }
}
#endif
