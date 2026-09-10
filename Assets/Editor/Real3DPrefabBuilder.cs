using System;
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
        Directory.CreateDirectory(Path.GetFullPath(OutputRoot+"/Characters"));
        Directory.CreateDirectory(Path.GetFullPath(OutputRoot+"/Weapons"));
        Directory.CreateDirectory(Path.GetFullPath(OutputRoot+"/Environment"));
        AssetDatabase.Refresh();

        GameObject player=Find(new[]{"soldier","player","character","robot"});
        GameObject enemy=Find(new[]{"enemy","robot","alien","zombie","character"});
        GameObject gun=Find(new[]{"shotgun","blaster","rifle","gun","weapon"});
        GameObject crate=Find(new[]{"crate","box","container"});
        GameObject barrel=Find(new[]{"barrel","drum"});

        Save(player,OutputRoot+"/Characters/Player.prefab");
        Save(enemy,OutputRoot+"/Characters/Enemy.prefab");
        Save(gun,OutputRoot+"/Weapons/Shotgun.prefab");
        Save(crate,OutputRoot+"/Environment/Crate.prefab");
        Save(barrel,OutputRoot+"/Environment/Barrel.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("3D Prefabs","تم بناء Prefabs حقيقية للاعب والعدو والسلاح والصناديق والبراميل.","حسناً");
    }

    static GameObject Find(string[] keys)
    {
        string[] ids=AssetDatabase.FindAssets("t:Model",new[]{SourceRoot});
        GameObject fallback=null;
        foreach(string id in ids)
        {
            string path=AssetDatabase.GUIDToAssetPath(id);
            GameObject g=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(g==null)continue;
            if(fallback==null)fallback=g;
            string n=g.name.ToLowerInvariant();
            foreach(string k in keys)if(n.Contains(k))return g;
        }
        return fallback;
    }

    static void Save(GameObject source,string path)
    {
        if(source==null)return;
        GameObject temp=PrefabUtility.InstantiatePrefab(source) as GameObject;
        if(temp==null)return;
        temp.name=Path.GetFileNameWithoutExtension(path);
        Normalize(temp);
        PrefabUtility.SaveAsPrefabAsset(temp,path);
        UnityEngine.Object.DestroyImmediate(temp);
    }

    static void Normalize(GameObject root)
    {
        root.transform.position=Vector3.zero;
        root.transform.rotation=Quaternion.identity;
        root.transform.localScale=Vector3.one;
        if(root.GetComponent<Collider>()==null)
        {
            Renderer[] rs=root.GetComponentsInChildren<Renderer>(true);
            if(rs.Length>0){BoxCollider c=root.AddComponent<BoxCollider>();Bounds b=rs[0].bounds;for(int i=1;i<rs.Length;i++)b.Encapsulate(rs[i].bounds);c.center=root.transform.InverseTransformPoint(b.center);c.size=root.transform.InverseTransformVector(b.size);}
        }
    }
}
