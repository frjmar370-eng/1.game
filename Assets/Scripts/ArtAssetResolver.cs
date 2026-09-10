using UnityEngine;

public static class ArtAssetResolver
{
    public static GameObject Player()=>LoadFirst("RealPacks/Characters/Player","Characters/Player","Characters/Soldier","RealPacks/SciFiEssentials/Character","Player");
    public static GameObject Enemy()=>LoadByKeywords(new[]{"enemy","robot","alien","zombie","character"},"RealPacks/Characters");
    public static GameObject Shotgun()=>LoadByKeywords(new[]{"shotgun","blaster","rifle","gun","weapon"},"RealPacks/Weapons");
    public static GameObject Crate()=>LoadByKeywords(new[]{"crate","box","container"},"RealPacks/Environment");
    public static GameObject Barrel()=>LoadByKeywords(new[]{"barrel","drum"},"RealPacks/Environment");

    public static GameObject RandomReal(string folder)
    {
        Object[] assets=Resources.LoadAll(folder,typeof(GameObject));
        return assets!=null&&assets.Length>0?assets[Random.Range(0,assets.Length)] as GameObject:null;
    }

    static GameObject LoadFirst(params string[] paths){foreach(string p in paths){GameObject g=Resources.Load<GameObject>(p);if(g!=null)return g;}return null;}

    static GameObject LoadByKeywords(string[] keys,string folder)
    {
        Object[] assets=Resources.LoadAll(folder,typeof(GameObject));
        if(assets==null||assets.Length==0)return null;
        foreach(Object a in assets){string n=a.name.ToLowerInvariant();foreach(string k in keys)if(n.Contains(k))return a as GameObject;}
        return assets[Random.Range(0,assets.Length)] as GameObject;
    }
}
