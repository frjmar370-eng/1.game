#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ShotgunAndroidBuild
{
    private const string ScenePath = "Assets/Scenes/Main.unity";
    private const string OutputPath = "Builds/Shotgun3D.apk";

    [MenuItem("Shotgun 3D/Build Android APK")]
    public static void BuildAPK()
    {
        EnsureProjectSetup();

        if (!File.Exists(ScenePath))
            throw new BuildFailedException("Required scene is missing: " + ScenePath);

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

        Directory.CreateDirectory("Builds");
        if (File.Exists(OutputPath))
            File.Delete(OutputPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new BuildFailedException($"Android build failed with result {report.summary.result}. See the Unity build log for the first error.");

        if (!File.Exists(OutputPath) || new FileInfo(OutputPath).Length == 0)
            throw new BuildFailedException("Unity reported success but the APK was not created at " + OutputPath);

        Debug.Log($"Shotgun 3D APK created: {OutputPath} ({new FileInfo(OutputPath).Length} bytes)");
    }

    private static void EnsureProjectSetup()
    {
        if (!File.Exists(ScenePath))
            ShotgunProjectSetup.Setup();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        PlayerSettings.productName = "Shotgun 3D";
        PlayerSettings.companyName = "Frjmar";
        PlayerSettings.applicationIdentifier = "com.frjmar.shotgun3d";

        // Android device profile: landscape FPS controls, ARM64, IL2CPP.
        PlayerSettings.defaultScreenOrientation = ScreenOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        AssetDatabase.SaveAssets();
    }
}
#endif
