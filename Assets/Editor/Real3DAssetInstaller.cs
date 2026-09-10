using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEditor;
using UnityEngine;

public static class Real3DAssetInstaller
{
    const string Root="Assets/Art/Packs";const string DownloadRoot="Assets/Art/Downloads";
    [MenuItem("Shotgun 3D/3D Assets/Download Free Sci-Fi Packs", priority=10)]public static void DownloadRecommendedPacks(){Directory.CreateDirectory(Path.GetFullPath(DownloadRoot));Download("Modular Sci-Fi MegaKit","https://opengameart.org/sites/default/files/modular_scifi_megakitstandard.zip","modular_scifi_megakitstandard.zip");Download("Sci-Fi Essentials Models","https://opengameart.org/sites/default/files/sci-fi_essentials_kit_models.zip","sci-fi_essentials_kit_models.zip");Download("Sci-Fi Essentials Textures","https://opengameart.org/sites/default/files/sci-fi_essentials_kit_textures.zip","sci-fi_essentials_kit_textures.zip");AssetDatabase.Refresh();if(!Application.isBatchMode)EditorUtility.DisplayDialog("3D Assets","تم تنزيل حزم البيئة والأسلحة والأعداء.","حسناً");}
    [MenuItem("Shotgun 3D/3D Assets/Install Recommended Packs", priority=11)]public static void InstallRecommendedPacks(){DownloadRecommendedPacks();ImportDownloadedZips();}
    [MenuItem("Shotgun 3D/3D Assets/INSTALL + BUILD COMPLETE", priority=0)]public static void InstallAndBuildComplete(){InstallRecommendedPacks();Real3DPrefabBuilder.Build();Real3DAnimationBuilder.Build();Real3DProjectPipeline.Validate();}
    [MenuItem("Shotgun 3D/3D Assets/Import Downloaded ZIPs", priority=12)]public static void ImportDownloadedZips(){Directory.CreateDirectory(Path.GetFullPath(DownloadRoot));Directory.CreateDirectory(Path.GetFullPath(Root));string[] zips=Directory.GetFiles(Path.GetFullPath(DownloadRoot),"*.zip",SearchOption.TopDirectoryOnly);if(zips.Length==0){if(!Application.isBatchMode)EditorUtility.DisplayDialog("3D Assets","لا توجد ZIP داخل Assets/Art/Downloads.","حسناً");return;}int done=0;foreach(string zip in zips){try{string target=Path.Combine(Path.GetFullPath(Root),Safe(Path.GetFileNameWithoutExtension(zip)));Directory.CreateDirectory(target);ZipFile.ExtractToDirectory(zip,target,true);done++;}catch(Exception e){Debug.LogError("3D pack import failed: "+zip+"\n"+e);}}AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);if(!Application.isBatchMode)EditorUtility.DisplayDialog("3D Assets",$"تم استيراد {done} حزمة 3D داخل Assets/Art/Packs.","حسناً");}
    static void Download(string label,string url,string fileName){string path=Path.Combine(Path.GetFullPath(DownloadRoot),fileName);if(File.Exists(path)&&new FileInfo(path).Length>1024*1024){Debug.Log(label+" موجود بالفعل");return;}try{using(var client=new WebClient()){client.Headers.Add(HttpRequestHeader.UserAgent,"Shotgun3DAssetInstaller/3.0");client.DownloadFile(url,path);}Debug.Log("Downloaded "+label+" -> "+path);}catch(Exception e){Debug.LogError("Download failed "+label+": "+e);if(File.Exists(path))File.Delete(path);}}
    [MenuItem("Shotgun 3D/3D Assets/Open Asset Folder", priority=13)]public static void OpenFolder(){Directory.CreateDirectory(Path.GetFullPath(Root));EditorUtility.RevealInFinder(Path.GetFullPath(Root));}
    static string Safe(string value){foreach(char c in Path.GetInvalidFileNameChars())value=value.Replace(c,'_');return value.Trim();}
}
