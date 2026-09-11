#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildPreparation
{
    const string ScenePath = "Assets/Scenes/Main.unity";

    [MenuItem("Shotgun 3D/FINAL PREPARE/Prepare Android Build")]
    public static void PrepareAndroid()
    {
        Directory.CreateDirectory("Assets/Scenes");
        EditorSettings.serializationMode = SerializationMode.ForceText;
        PlayerSettings.companyName = "Frjmar";
        PlayerSettings.productName = "Shotgun 3D";
        PlayerSettings.applicationIdentifier = "com.ammar.game";
        PlayerSettings.defaultScreenOrientation = ScreenOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        if (!File.Exists(ScenePath))
            FirstPlayableScene.RebuildMainScene();
        else
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SHOTGUN 3D: Android build settings prepared. Now use Shotgun 3D > FINAL BUILD > Build Android APK.");
    }

    [MenuItem("Shotgun 3D/FINAL PREPARE/Validate Project")]
    public static void ValidateProject()
    {
        string[] required = {
            "Assets/Art/Environment/CityKit.obj",
            "Assets/Art/Environment/CityKit.mtl",
            "Assets/Art/Characters/Hero.obj",
            "Assets/Art/Characters/Actors.mtl",
            "Assets/Art/Weapons/Shotgun.obj",
            "Assets/Scripts/ThirdPersonController.cs",
            "Assets/Scripts/ThirdPersonCamera.cs",
            "Assets/Scripts/MobileControls.cs",
            "Assets/Scripts/WeaponController.cs",
            "Assets/Scripts/Health.cs"
        };
        bool ok = true;
        foreach (var path in required)
        {
            bool exists = File.Exists(path);
            Debug.Log((exists ? "OK   " : "MISS ") + path);
            ok &= exists;
        }
        Debug.Log(ok ? "SHOTGUN 3D VALIDATION: PASS" : "SHOTGUN 3D VALIDATION: FAIL - missing required files");
    }
}

public static class FinalBuildMenu
{
    [MenuItem("Shotgun 3D/FINAL BUILD/Build Android APK")]
    public static void BuildAndroid()
    {
        BuildPreparation.PrepareAndroid();
        string output = "Builds/Android/Shotgun3D.apk";
        Directory.CreateDirectory("Builds/Android");
        string scene = "Assets/Scenes/Main.unity";
        if (!File.Exists(scene))
        {
            Debug.LogError("Main.unity was not created. Open the project in Unity and run Rebuild Main Scene first.");
            return;
        }
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { scene },
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });
        Debug.Log("SHOTGUN 3D BUILD RESULT: " + report.summary.result + " | errors=" + report.summary.totalErrors + " | warnings=" + report.summary.totalWarnings);
    }
}
#endif
