#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public static class ShotgunAndroidBuild
{
    private const string ScenePath="Assets/Scenes/Main.unity";
    private const string OutputPath="Builds/Shotgun3D.apk";
    private const string BundleManifestPath="Builds/Content/Android.manifest";

    [MenuItem("Shotgun 3D/Build Android APK")]
    public static void BuildAPK()
    {
        EnsureProjectSetup();
        Real3DAssetInstaller.InstallRecommendedPacks();
        Real3DProjectPipeline.BuildComplete();
        if(!File.Exists(ScenePath)) throw new BuildFailedException("Required scene is missing: "+ScenePath);

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle=false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject=false;
        Directory.CreateDirectory("Builds");

        // Convert the imported real 3D prefabs into a platform-specific runtime bundle.
        RuntimeAssetBundleBuilder.Build(BuildTarget.Android);
        if(!File.Exists(BundleManifestPath)) throw new BuildFailedException("Runtime AssetBundle manifest is missing: "+BundleManifestPath);

        // The APK contains only code/bootstrap. Gameplay 3D content is downloaded on first launch.
        RuntimeAssetBundleBuilder.RemoveEmbeddedRuntimePrefabs();

        if(File.Exists(OutputPath)) File.Delete(OutputPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        BuildReport report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{
            scenes=new[]{ScenePath},
            locationPathName=OutputPath,
            target=BuildTarget.Android,
            assetBundleManifestPath=BundleManifestPath,
            options=BuildOptions.None
        });
        if(report.summary.result!=BuildResult.Succeeded) throw new BuildFailedException($"Android build failed with result {report.summary.result}. See the Unity build log for the first error.");
        if(!File.Exists(OutputPath)||new FileInfo(OutputPath).Length==0) throw new BuildFailedException("Unity reported success but the APK was not created at "+OutputPath);
        Debug.Log($"Shotgun 3D APK created: {OutputPath} ({new FileInfo(OutputPath).Length} bytes)");
    }

    private static void EnsureProjectSetup(){
        if(!File.Exists(ScenePath))ShotgunProjectSetup.Setup();
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        PlayerSettings.productName="Shotgun 3D";
        PlayerSettings.companyName="Frjmar";
        PlayerSettings.applicationIdentifier="com.ammar.game";
        PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait=false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
        PlayerSettings.allowedAutorotateToLandscapeLeft=true;
        PlayerSettings.allowedAutorotateToLandscapeRight=true;
        PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)35;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.OpenGLES3});
        AssetDatabase.SaveAssets();
    }
}
#endif
