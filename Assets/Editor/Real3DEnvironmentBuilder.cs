using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Real3DEnvironmentBuilder
{
    const string Root="Assets/Art/Packs";

    [MenuItem("Shotgun 3D/3D Assets/Build 3D Combat Arena", priority=21)]
    public static void BuildArena()
    {
        GameObject old=GameObject.Find("REAL_3D_ARENA");
        if(old!=null)UnityEngine.Object.DestroyImmediate(old);
        GameObject arena=new GameObject("REAL_3D_ARENA");

        GameObject floor=GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name="Arena_Floor";floor.transform.SetParent(arena.transform);floor.transform.position=new Vector3(0,-.25f,6);floor.transform.localScale=new Vector3(28,.5f,34);

        CreateWall(arena,new Vector3(0,2,23),new Vector3(28,4,.6f));
        CreateWall(arena,new Vector3(0,2,-11),new Vector3(28,4,.6f));
        CreateWall(arena,new Vector3(-14,2,6),new Vector3(.6f,4,34));
        CreateWall(arena,new Vector3(14,2,6),new Vector3(.6f,4,34));

        for(int i=0;i<18;i++)
        {
            Vector3 p=new Vector3(UnityEngine.Random.Range(-11f,11f),0,UnityEngine.Random.Range(-6f,18f));
            GameObject prefab=FindModel(i%3==0?new[]{"crate","box","container"}:i%3==1?new[]{"barrel","drum"}:new[]{"wall","pillar","cover","container"});
            GameObject o=prefab!=null?PrefabUtility.InstantiatePrefab(prefab) as GameObject:GameObject.CreatePrimitive(PrimitiveType.Cube);
            if(o==null)continue;
            o.name="Cover_"+i;o.transform.SetParent(arena.transform);o.transform.position=p;o.transform.rotation=Quaternion.Euler(0,UnityEngine.Random.Range(0,360),0);
            if(prefab==null)o.transform.localScale=new Vector3(UnityEngine.Random.Range(1.2f,2.8f),UnityEngine.Random.Range(1f,2.2f),UnityEngine.Random.Range(1.2f,2.8f));
        }

        CreateLight(arena,new Vector3(0,10,6),new Color(0.7f,0.85f,1f),1000,28);
        CreateLight(arena,new Vector3(-10,6,15),new Color(0.3f,0.6f,1f),700,18);
        CreateLight(arena,new Vector3(10,6,0),new Color(1f,0.45f,0.2f),700,18);
        RenderSettings.ambientIntensity=.65f;
        EditorSceneManagerMarkDirty();
        EditorUtility.DisplayDialog("3D Arena","تم إنشاء ساحة 3D كبيرة مع جدران وغطاء وإضاءة ومواقع قتال.","حسناً");
    }

    static void CreateWall(GameObject parent,Vector3 p,Vector3 s){GameObject w=GameObject.CreatePrimitive(PrimitiveType.Cube);w.name="Arena_Wall";w.transform.SetParent(parent.transform);w.transform.position=p;w.transform.localScale=s;}
    static void CreateLight(GameObject parent,Vector3 p,Color color,float intensity,float range){GameObject g=new GameObject("Arena_Light");g.transform.SetParent(parent.transform);g.transform.position=p;Light l=g.AddComponent<Light>();l.type=LightType.Point;l.color=color;l.intensity=intensity;l.range=range;}

    static GameObject FindModel(string[] keys)
    {
        string[] ids=AssetDatabase.FindAssets("t:Model",new[]{Root});
        foreach(string id in ids){string path=AssetDatabase.GUIDToAssetPath(id);GameObject g=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(g==null)continue;string n=g.name.ToLowerInvariant();foreach(string k in keys)if(n.Contains(k))return g;}
        return null;
    }
    static void EditorSceneManagerMarkDirty(){UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());}
}
