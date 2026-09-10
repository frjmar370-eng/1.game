#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ShotgunProjectSetup
{
    [MenuItem("Shotgun 3D/Setup Main Scene + Android")]
    public static void Setup()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject bootstrap = new GameObject("Scene Bootstrap");
        bootstrap.AddComponent<SceneBootstrap>();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };

        PlayerSettings.productName = "Shotgun 3D";
        PlayerSettings.companyName = "Frjmar";
        PlayerSettings.applicationIdentifier = "com.ammar.game";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        AssetDatabase.SaveAssets();
        Debug.Log("Shotgun 3D setup complete. Main.unity is ready.");
    }
}
#endif
