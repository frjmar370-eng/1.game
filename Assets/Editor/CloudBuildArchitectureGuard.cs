#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Keeps Android CPU architecture explicitly selected for Unity Cloud Build and
/// command-line builds, even when a build target overrides the serialized
/// Player Settings value with None.
/// </summary>
[InitializeOnLoad]
public sealed class CloudBuildArchitectureGuard : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    static CloudBuildArchitectureGuard()
    {
        EditorApplication.delayCall += EnsureAndroidArchitecture;
    }

    [InitializeOnLoadMethod]
    private static void InitializeOnLoadGuard()
    {
        EditorApplication.delayCall += EnsureAndroidArchitecture;
    }

    [MenuItem("Shotgun 3D/Ensure Android ARM64")]
    public static void EnsureAndroidArchitecture()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
            EditorUserBuildSettings.activeBuildTarget != BuildTarget.NoTarget)
            return;

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.buildApkPerCpuArchitecture = false;
        AssetDatabase.SaveAssets();
        Debug.Log("Shotgun 3D: Android target architecture forced to ARM64.");
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.buildApkPerCpuArchitecture = false;
        AssetDatabase.SaveAssets();
        Debug.Log("Shotgun 3D Cloud Build guard: Android target architecture = ARM64.");
    }
}
#endif
