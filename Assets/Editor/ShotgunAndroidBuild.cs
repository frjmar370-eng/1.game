#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ShotgunAndroidBuild
{
    [MenuItem("Shotgun 3D/Build Android APK")]
    public static void BuildAPK()
    {
        if (!System.IO.File.Exists("Assets/Scenes/Main.unity"))
            ShotgunProjectSetup.Setup();

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = false;
        string output = "Builds/Shotgun3D.apk";
        System.IO.Directory.CreateDirectory("Builds");
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("Shotgun 3D APK created: " + output);
        else
            Debug.LogError("Android APK build failed. Check the Console for the first error.");
    }
}
#endif
