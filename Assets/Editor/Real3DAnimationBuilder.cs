using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class Real3DAnimationBuilder
{
    const string SourceRoot="Assets/Art/Packs";
    const string OutRoot="Assets/Resources/RealPacks/Animations";

    [MenuItem("Shotgun 3D/3D Assets/Build Animation Controllers", priority=21)]
    public static void Build()
    {
        Directory.CreateDirectory(Path.GetFullPath(OutRoot));
        AssetDatabase.Refresh();
        List<AnimationClip> clips=FindClips();
        if(clips.Count==0){Debug.LogWarning("No imported AnimationClip assets were found under "+SourceRoot);return;}
        RuntimeAnimatorController player=BuildController("Player.controller",clips);
        RuntimeAnimatorController enemy=BuildController("Enemy.controller",clips);
        AssignController("Assets/Resources/RealPacks/Characters/Player.prefab",player);
        AssignController("Assets/Resources/RealPacks/Characters/Enemy.prefab",enemy);
        AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Animations","تم إنشاء Controllers حقيقية من Animation Clips المستوردة وربطها بالـPrefabs.","حسناً");
    }

    static List<AnimationClip> FindClips()
    {
        var result=new List<AnimationClip>();
        foreach(string id in AssetDatabase.FindAssets("t:AnimationClip",new[]{SourceRoot}))
        {
            string path=AssetDatabase.GUIDToAssetPath(id);UnityEngine.Object[] all=AssetDatabase.LoadAllAssetsAtPath(path);
            foreach(UnityEngine.Object o in all){AnimationClip c=o as AnimationClip;if(c!=null&&!c.name.StartsWith("__preview__"))result.Add(c);}
        }
        return result;
    }

    static RuntimeAnimatorController BuildController(string file,List<AnimationClip> clips)
    {
        string path=OutRoot+"/"+file;AnimatorController c=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if(c==null)c=AnimatorController.CreateAnimatorControllerAtPath(path);
        AnimatorControllerLayer layer=c.layers[0];AnimatorStateMachine sm=layer.stateMachine;
        string[] wanted={"idle","stand","walk","run","locomotion","attack","shoot","fire","hit","hurt","death","die"};
        var used=new HashSet<string>();
        foreach(string w in wanted)
        {
            AnimationClip clip=BestClip(clips,w,used);if(clip==null)continue;
            string stateName=Canonical(w);if(FindState(sm,stateName)!=null)continue;
            AnimatorState state=sm.AddState(stateName);state.motion=clip;used.Add(clip.name);
            if(stateName=="Idle"&&sm.defaultState==null)sm.defaultState=state;
        }
        if(sm.defaultState==null&&sm.states.Length>0)sm.defaultState=sm.states[0].state;
        EditorUtility.SetDirty(c);return c;
    }

    static AnimationClip BestClip(List<AnimationClip> clips,string keyword,HashSet<string> used)
    {
        AnimationClip best=null;int score=int.MinValue;
        foreach(AnimationClip c in clips){if(used.Contains(c.name))continue;string n=c.name.ToLowerInvariant();int s=0;if(n.Contains(keyword))s+=100;if(keyword=="locomotion"&&(n.Contains("move")||n.Contains("forward")))s+=40;if(keyword=="stand"&&n.Contains("idle"))s+=30;if(s>score){score=s;best=s>0?c:null;}}
        return best;
    }

    static string Canonical(string k){switch(k){case "stand":return "Idle";case "locomotion":return "Walk";case "shoot":case "fire":return "Attack";case "hurt":return "Hit";case "die":return "Death";default:return char.ToUpper(k[0])+k.Substring(1);}}
    static AnimatorState FindState(AnimatorStateMachine sm,string n){foreach(var s in sm.states)if(s.state.name==n)return s.state;return null;}

    static void AssignController(string prefabPath,RuntimeAnimatorController controller)
    {
        if(controller==null||!File.Exists(Path.GetFullPath(prefabPath)))return;
        GameObject root=PrefabUtility.LoadPrefabContents(prefabPath);if(root==null)return;
        Animator animator=root.GetComponentInChildren<Animator>(true);if(animator==null)animator=root.AddComponent<Animator>();animator.runtimeAnimatorController=controller;
        PrefabUtility.SaveAsPrefabAsset(root,prefabPath);PrefabUtility.UnloadPrefabContents(root);
    }
}
