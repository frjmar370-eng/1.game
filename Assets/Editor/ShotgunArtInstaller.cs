#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class ShotgunArtInstaller
{
    private const string ArtRoot = "Assets/Art/Imported";
    private const string ResourcesRoot = "Assets/Resources";
    private const string GunZipUrl = "https://opengameart.org/sites/default/files/ultimate_gun_pack_by_quaternius.zip";
    private const string HumanZipUrl = "https://opengameart.org/sites/default/files/Animated%20Human%20by%20%40Quaternius_0.zip";
    private const string SciFiModelsZipUrl = "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_models.zip";
    private const string SciFiTexturesZipUrl = "https://opengameart.org/sites/default/files/sci-fi_essentials_kit_textures.zip";
    private const string ApiBase = "https://api.polyhaven.com/files/";

    [MenuItem("Shotgun 3D/Art/Install CC0 Art")]
    public static void InstallAndPrepareMenu() => InstallAndPrepare();

    public static void InstallAndPrepare()
    {
        Directory.CreateDirectory(ArtRoot);
        Directory.CreateDirectory(ResourcesRoot);
        DownloadAndExtract("QuaterniusGuns", GunZipUrl);
        DownloadAndExtract("AnimatedHuman", HumanZipUrl);
        DownloadAndExtract("SciFiEssentialsModels", SciFiModelsZipUrl);
        DownloadAndExtract("SciFiEssentialsTextures", SciFiTexturesZipUrl);
        InstallPolyHaven("barrel_01", "Barrel_01");
        InstallPolyHaven("wooden_military_crate", "MilitaryCrate");

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        CreatePrefabFromModel("shotgun", "Assets/Art/Imported/QuaterniusGuns", "Assets/Resources/Weapons/Shotgun.prefab");
        CreatePrefabFromModel("barrel", "Assets/Art/Imported/Barrel_01", "Assets/Resources/Environment/Barrel.prefab");
        CreatePrefabFromModel("crate", "Assets/Art/Imported/MilitaryCrate", "Assets/Resources/Environment/Crate.prefab");
        CreatePrefabFromModel("human", "Assets/Art/Imported/AnimatedHuman", "Assets/Resources/Characters/Player.prefab");
        CreatePrefabFromModel("robot", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Characters/Enemy.prefab");
        CreatePrefabFromModel("screen", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Environment/Screen.prefab");
        CreatePrefabFromModel("door", "Assets/Art/Imported/SciFiEssentialsModels", "Assets/Resources/Environment/Door.prefab");
        ConfigureAnimator("Assets/Resources/Characters/Player.prefab", "Assets/Art/Imported/AnimatedHuman");
        ConfigureAnimator("Assets/Resources/Characters/Enemy.prefab", "Assets/Art/Imported/SciFiEssentialsModels");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void InstallPolyHaven(string slug,string folderName)
    {
        string json=DownloadText(ApiBase+slug,"Shotgun3D-AssetInstaller/2.1"); if(string.IsNullOrEmpty(json))return;
        string url=ExtractUrlNear(json,"\"1k\""); if(string.IsNullOrEmpty(url))url=ExtractZipUrl(json); if(!string.IsNullOrEmpty(url))DownloadAndExtract(folderName,url);
    }
    private static string ExtractUrlNear(string json,string marker){int i=json.IndexOf(marker,StringComparison.OrdinalIgnoreCase);if(i<0)return null;int end=Mathf.Min(json.Length,i+12000);return ExtractZipUrl(json.Substring(i,end-i));}
    private static string ExtractZipUrl(string json){Match m=Regex.Match(json,"\\\"url\\\"\\s*:\\s*\\\"([^\\\"]+\\.zip)\\\"",RegexOptions.IgnoreCase);return m.Success?m.Groups[1].Value.Replace("\\u0026","&"):null;}
    private static string DownloadText(string url,string userAgent){try{using(WebClient c=new WebClient()){c.Headers[HttpRequestHeader.UserAgent]=userAgent;return c.DownloadString(url);}}catch(Exception ex){Debug.LogWarning("Art download failed: "+url+"\n"+ex.Message);return null;}}
    private static void DownloadAndExtract(string folderName,string url)
    {
        string targetDir=Path.Combine(ArtRoot,folderName).Replace('\\','/');string marker=Path.Combine(targetDir,".installed");if(File.Exists(marker))return;Directory.CreateDirectory(targetDir);
        string tempZip=Path.Combine(Path.GetTempPath(),"shotgun3d_"+folderName+".zip");
        try{using(WebClient c=new WebClient()){c.Headers[HttpRequestHeader.UserAgent]="Shotgun3D-AssetInstaller/2.1";Debug.Log("Downloading "+folderName);c.DownloadFile(url,tempZip);}ZipFile.ExtractToDirectory(tempZip,targetDir,true);File.WriteAllText(marker,"Installed from: "+url+"\nUTC: "+DateTime.UtcNow.ToString("O"));}
        catch(Exception ex){Debug.LogError("Asset install failed for "+folderName+": "+ex);}finally{try{if(File.Exists(tempZip))File.Delete(tempZip);}catch{}}
    }
    private static void CreatePrefabFromModel(string keyword,string searchFolder,string prefabPath)
    {
        if(File.Exists(prefabPath))return;string directory=Path.GetDirectoryName(prefabPath)?.Replace('\\','/');if(!string.IsNullOrEmpty(directory))Directory.CreateDirectory(directory);
        string[] guids=AssetDatabase.FindAssets("t:Model",new[]{searchFolder});string selected=null;
        foreach(string guid in guids){string path=AssetDatabase.GUIDToAssetPath(guid);string file=Path.GetFileNameWithoutExtension(path).ToLowerInvariant();if(file.Contains(keyword.ToLowerInvariant())){selected=path;break;}}
        if(selected==null)return;GameObject source=AssetDatabase.LoadAssetAtPath<GameObject>(selected);if(source==null)return;GameObject instance=PrefabUtility.InstantiatePrefab(source) as GameObject;if(instance==null)return;instance.name=Path.GetFileNameWithoutExtension(prefabPath);
        try{PrefabUtility.SaveAsPrefabAsset(instance,prefabPath);}finally{UnityEngine.Object.DestroyImmediate(instance);}
    }
    private static void ConfigureAnimator(string prefabPath,string searchFolder)
    {
        GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);if(prefab==null)return;Animator animator=prefab.GetComponentInChildren<Animator>();if(animator==null)return;
        string[] guids=AssetDatabase.FindAssets("t:Model",new[]{searchFolder});if(guids.Length==0)return;string modelPath=AssetDatabase.GUIDToAssetPath(guids[0]);
        Object[] assets=AssetDatabase.LoadAllAssetsAtPath(modelPath);AnimationClip idle=null,walk=null,run=null,death=null;
        foreach(Object o in assets){AnimationClip clip=o as AnimationClip;if(clip==null||clip.name.StartsWith("__preview__"))continue;string n=clip.name.ToLowerInvariant();if(idle==null&&n.Contains("idle"))idle=clip;else if(walk==null&&n.Contains("walk"))walk=clip;else if(run==null&&(n.Contains("run")||n.Contains("jog")))run=clip;else if(death==null&&n.Contains("death"))death=clip;}
        if(idle==null&&walk==null&&run==null)return;
        string controllerPath=Path.GetDirectoryName(prefabPath)+"/"+Path.GetFileNameWithoutExtension(prefabPath)+"_Controller.controller";
        UnityEditor.Animations.AnimatorController controller=AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);if(controller==null)controller=UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var layer=controller.layers[0];var sm=layer.stateMachine;sm.states=new UnityEditor.Animations.ChildAnimatorState[0];
        AnimationClip defaultClip=idle??walk??run;var state=sm.AddState(defaultClip.name);state.motion=defaultClip;sm.defaultState=state;
        if(walk!=null&&walk!=defaultClip){var s=sm.AddState("Walk");s.motion=walk;}
        if(run!=null&&run!=defaultClip){var s=sm.AddState("Run");s.motion=run;}
        if(death!=null){var s=sm.AddState("Death");s.motion=death;}
        animator.runtimeAnimatorController=controller;PrefabUtility.SavePrefabAsset(prefab);
    }
}
#endif
