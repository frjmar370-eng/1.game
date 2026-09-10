using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class Real3DPrefabBuilder
{
    const string SourceRoot="Assets/Art/Packs";
    const string OutputRoot="Assets/Resources/RealPacks";
    const string ControllerRoot="Assets/Resources/RealPacks/Animations";

    [MenuItem("Shotgun 3D/3D Assets/Build Gameplay Prefabs", priority=20)]
    public static void Build()
    {
        EnsureFolders(); AssetDatabase.Refresh();
        Save(FindBest(Role.Player),OutputRoot+"/Characters/Player.prefab",true);
        Save(FindBest(Role.Enemy),OutputRoot+"/Characters/Enemy.prefab",true);
        Save(FindBest(Role.WeaponShotgun),OutputRoot+"/Weapons/Shotgun.prefab",false);
        Save(FindBest(Role.WeaponRifle),OutputRoot+"/Weapons/Rifle.prefab",false);
        Save(FindBest(Role.WeaponPistol),OutputRoot+"/Weapons/Pistol.prefab",false);
        Save(FindBest(Role.Crate),OutputRoot+"/Environment/Crate.prefab",false);
        Save(FindBest(Role.Barrel),OutputRoot+"/Environment/Barrel.prefab",false);
        BuildControllers();
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("3D Build","تم بناء الشخصيات والأسلحة والبيئة والـAnimator Controllers. راجع Console لأي عنصر مفقود.","حسناً");
    }

    enum Role { Player, Enemy, WeaponShotgun, WeaponRifle, WeaponPistol, Crate, Barrel }

    static GameObject FindBest(Role role)
    {
        string[] ids=AssetDatabase.FindAssets("t:Model",new[]{SourceRoot}); GameObject best=null; int bestScore=int.MinValue;
        foreach(string id in ids){string path=AssetDatabase.GUIDToAssetPath(id);if(IsIgnored(path))continue;GameObject g=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(g==null)continue;int score=Score(path,g.name,role);if(score>bestScore){bestScore=score;best=g;}}
        Debug.Log($"[3D Catalog] {role}: {Name(best)} score={bestScore} path={(best?AssetDatabase.GetAssetPath(best):"MISSING")}");
        return best;
    }

    static int Score(string path,string name,Role role)
    {
        string s=(path+"/"+name).ToLowerInvariant();int score=0;
        string[] strong;
        switch(role){
            case Role.Player:strong=new[]{"universal base","soldier","player","marine","character","humanoid","robot"};break;
            case Role.Enemy:strong=new[]{"enemy","alien","zombie","robot","monster","character"};break;
            case Role.WeaponShotgun:strong=new[]{"shotgun","pump","scatter","sci-fi gun"};break;
            case Role.WeaponRifle:strong=new[]{"rifle","assault","smg","carbine","blaster"};break;
            case Role.WeaponPistol:strong=new[]{"pistol","handgun","sidearm"};break;
            case Role.Crate:strong=new[]{"crate","box","container","cargo"};break;
            default:strong=new[]{"barrel","drum","tank"};break;
        }
        for(int i=0;i<strong.Length;i++)if(s.Contains(strong[i]))score+=150-i*15;
        if(s.Contains("icon")||s.Contains("preview")||s.Contains("thumbnail")||s.Contains("material")||s.Contains("sample"))score-=400;
        if(path.EndsWith(".fbx",StringComparison.OrdinalIgnoreCase)||path.EndsWith(".glb",StringComparison.OrdinalIgnoreCase)||path.EndsWith(".gltf",StringComparison.OrdinalIgnoreCase))score+=10;
        if((role==Role.Player||role==Role.Enemy)&&s.Contains("environment"))score-=150;
        if(role>=Role.WeaponShotgun&&role<=Role.WeaponPistol&&(s.Contains("character")||s.Contains("environment")))score-=120;
        return score;
    }

    static bool IsIgnored(string path){string p=path.ToLowerInvariant();return p.Contains("/preview")||p.Contains("/previews/")||p.Contains("/icon")||p.Contains("/icons/")||p.Contains("/demo/");}

    static void Save(GameObject source,string path,bool character)
    {
        if(source==null){Debug.LogWarning("[3D Catalog] Missing source: "+path);return;}
        GameObject temp=PrefabUtility.InstantiatePrefab(source) as GameObject;if(temp==null)return;
        temp.name=Path.GetFileNameWithoutExtension(path);Normalize(temp,character);if(character)EnsureAnimator(temp);
        PrefabUtility.SaveAsPrefabAsset(temp,path);UnityEngine.Object.DestroyImmediate(temp);
    }

    static void Normalize(GameObject root,bool character)
    {
        root.transform.position=Vector3.zero;root.transform.rotation=Quaternion.identity;root.transform.localScale=Vector3.one;
        Renderer[] rs=root.GetComponentsInChildren<Renderer>(true);
        if(root.GetComponent<Collider>()==null&&rs.Length>0){Bounds b=rs[0].bounds;for(int i=1;i<rs.Length;i++)b.Encapsulate(rs[i].bounds);BoxCollider c=root.AddComponent<BoxCollider>();c.center=root.transform.InverseTransformPoint(b.center);Vector3 size=root.transform.InverseTransformVector(b.size);c.size=new Vector3(Mathf.Abs(size.x),Mathf.Abs(size.y),Mathf.Abs(size.z));}
    }

    static void EnsureAnimator(GameObject root)
    {
        Animator a=root.GetComponentInChildren<Animator>(true);if(a==null&&root.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length>0)a=root.AddComponent<Animator>();
        if(a!=null){string controller=AssetDatabase.FindAssets("t:AnimatorController",new[]{ControllerRoot}).Length>0?AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:AnimatorController",new[]{ControllerRoot})[0]):null;if(!string.IsNullOrEmpty(controller))a.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controller);}
    }

    static void BuildControllers()
    {
        string[] roles={"Player","Enemy"};
        foreach(string role in roles)
        {
            string path=ControllerRoot+"/"+role+"Controller.controller";AnimatorController c=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);if(c==null)c=AnimatorController.CreateAnimatorControllerAtPath(path);
            AnimatorStateMachine sm=c.layers[0].stateMachine;while(sm.states.Length>0)sm.RemoveState(sm.states[0].state);
            AddState(c,sm,"Idle",FindClip(new[]{"idle","stand","breath"}));AddState(c,sm,"Walk",FindClip(new[]{"walk","locomotion"}));AddState(c,sm,"Run",FindClip(new[]{"run","sprint"}));AddState(c,sm,"Attack",FindClip(new[]{"attack","shoot","fire","punch","hit"}));AddState(c,sm,"Hit",FindClip(new[]{"hit","damage","hurt"}));AddState(c,sm,"Death",FindClip(new[]{"death","die","dead"}));
            AssetDatabase.SaveAssets();
        }
    }

    static void AddState(AnimatorController c,AnimatorStateMachine sm,string name,AnimationClip clip)
    {
        AnimatorState state=sm.AddState(name);if(clip!=null)state.motion=clip;if(name=="Idle")sm.defaultState=state;
    }

    static AnimationClip FindClip(string[] keys)
    {
        string[] ids=AssetDatabase.FindAssets("t:AnimationClip",new[]{SourceRoot});AnimationClip best=null;int scoreBest=int.MinValue;
        foreach(string id in ids){string path=AssetDatabase.GUIDToAssetPath(id);UnityEngine.Object[] assets=AssetDatabase.LoadAllAssetsAtPath(path);foreach(UnityEngine.Object o in assets){AnimationClip clip=o as AnimationClip;if(clip==null||clip.name.StartsWith("__preview__",StringComparison.OrdinalIgnoreCase))continue;string s=(path+"/"+clip.name).ToLowerInvariant();int score=0;for(int i=0;i<keys.Length;i++)if(s.Contains(keys[i]))score+=100-i*10;if(score>scoreBest){scoreBest=score;best=clip;}}}
        return best;
    }

    static void EnsureFolders(){foreach(string f in new[]{OutputRoot,OutputRoot+"/Characters",OutputRoot+"/Weapons",OutputRoot+"/Environment",ControllerRoot})Directory.CreateDirectory(Path.GetFullPath(f));}
    static string Name(GameObject g)=>g?g.name:"MISSING";
}
