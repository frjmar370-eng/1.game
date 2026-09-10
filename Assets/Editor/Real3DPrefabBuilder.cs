using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Real3DPrefabBuilder
{
    const string SourceRoot="Assets/Art/Packs";
    const string OutputRoot="Assets/Resources/RealPacks";

    [MenuItem("Shotgun 3D/3D Assets/Build Gameplay Prefabs", priority=20)]
    public static void Build()
    {
        EnsureFolders();
        AssetDatabase.Refresh();
        GameObject player=FindBest(Role.Player);
        GameObject enemy=FindBest(Role.Enemy);
        GameObject weapon=FindBest(Role.Weapon);
        GameObject crate=FindBest(Role.Crate);
        GameObject barrel=FindBest(Role.Barrel);
        Save(player,OutputRoot+"/Characters/Player.prefab");
        Save(enemy,OutputRoot+"/Characters/Enemy.prefab");
        Save(weapon,OutputRoot+"/Weapons/Shotgun.prefab");
        Save(crate,OutputRoot+"/Environment/Crate.prefab");
        Save(barrel,OutputRoot+"/Environment/Barrel.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Real 3D prefabs: player={Name(player)}, enemy={Name(enemy)}, weapon={Name(weapon)}, crate={Name(crate)}, barrel={Name(barrel)}");
        EditorUtility.DisplayDialog("3D Prefabs","تم اختيار وبناء الـPrefabs الحقيقية بطريقة ثابتة. راجع Console لأي عنصر غير موجود.","حسناً");
    }

    enum Role { Player, Enemy, Weapon, Crate, Barrel }

    static GameObject FindBest(Role role)
    {
        string[] ids=AssetDatabase.FindAssets("t:Model",new[]{SourceRoot});
        GameObject best=null; int bestScore=int.MinValue;
        foreach(string id in ids)
        {
            string path=AssetDatabase.GUIDToAssetPath(id);
            if(IsIgnored(path))continue;
            GameObject g=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(g==null)continue;
            int score=Score(path,g.name,role);
            if(score>bestScore){bestScore=score;best=g;}
        }
        if(best!=null)Debug.Log($"3D role {role}: {AssetDatabase.GetAssetPath(best)} score={bestScore}");
        return best;
    }

    static int Score(string path,string name,Role role)
    {
        string s=(path+"/"+name).ToLowerInvariant(); int score=0;
        string[] strong;
        switch(role){
            case Role.Player: strong=new[]{"universal base","soldier","player","character"}; break;
            case Role.Enemy: strong=new[]{"enemy","robot","alien","zombie","character"}; break;
            case Role.Weapon: strong=new[]{"shotgun","sci-fi gun","blaster","rifle","gun","weapon"}; break;
            case Role.Crate: strong=new[]{"crate","box","container"}; break;
            default: strong=new[]{"barrel","drum"}; break;
        }
        for(int i=0;i<strong.Length;i++)if(s.Contains(strong[i]))score+=120-i*12;
        if(s.Contains("icon")||s.Contains("preview")||s.Contains("thumbnail")||s.Contains("material"))score-=250;
        if(path.EndsWith(".fbx",StringComparison.OrdinalIgnoreCase)||path.EndsWith(".glb",StringComparison.OrdinalIgnoreCase))score+=8;
        if(role==Role.Player&&s.Contains("environment"))score-=100;
        if(role==Role.Weapon&&(s.Contains("character")||s.Contains("environment")))score-=80;
        return score;
    }

    static bool IsIgnored(string path)
    {
        string p=path.ToLowerInvariant();
        return p.Contains("/preview")||p.Contains("/previews/")||p.Contains("/icon")||p.Contains("/icons/");
    }

    static void Save(GameObject source,string path)
    {
        if(source==null){Debug.LogWarning("Missing 3D source for "+path);return;}
        GameObject temp=PrefabUtility.InstantiatePrefab(source) as GameObject;
        if(temp==null){Debug.LogWarning("Could not instantiate "+AssetDatabase.GetAssetPath(source));return;}
        temp.name=Path.GetFileNameWithoutExtension(path);
        Normalize(temp);
        EnsureAnimator(temp);
        PrefabUtility.SaveAsPrefabAsset(temp,path);
        UnityEngine.Object.DestroyImmediate(temp);
    }

    static void Normalize(GameObject root)
    {
        root.transform.position=Vector3.zero;root.transform.rotation=Quaternion.identity;root.transform.localScale=Vector3.one;
        Renderer[] rs=root.GetComponentsInChildren<Renderer>(true);
        if(root.GetComponent<Collider>()==null&&rs.Length>0)
        {
            Bounds b=rs[0].bounds;for(int i=1;i<rs.Length;i++)b.Encapsulate(rs[i].bounds);
            BoxCollider c=root.AddComponent<BoxCollider>();c.center=root.transform.InverseTransformPoint(b.center);c.size=root.transform.InverseTransformVector(b.size);
        }
    }

    static void EnsureAnimator(GameObject root)
    {
        if(root.GetComponentInChildren<Animator>(true)==null&&root.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length>0)root.AddComponent<Animator>();
    }

    static void EnsureFolders(){
        foreach(string f in new[]{OutputRoot,OutputRoot+"/Characters",OutputRoot+"/Weapons",OutputRoot+"/Environment"})Directory.CreateDirectory(Path.GetFullPath(f));
    }
    static string Name(GameObject g)=>g?g.name:"MISSING";
}
