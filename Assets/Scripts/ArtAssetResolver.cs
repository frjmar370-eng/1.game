using UnityEngine;

public static class ArtAssetResolver
{
    public static GameObject Player() => LoadFirst("Characters/Player", "Characters/Soldier", "RealPacks/SciFiEssentials/Character", "RealPacks/KenneySurvival/character", "Player");
    public static GameObject Enemy() => LoadFirst("Characters/Enemy", "Characters/Zombie", "RealPacks/SciFiEssentials/Robot", "RealPacks/SciFiEssentials/Enemy", "RealPacks/KenneySurvival/zombie", "Enemy");
    public static GameObject Shotgun() => LoadFirst("Weapons/Shotgun", "Weapons/Shotgun01", "RealPacks/KenneyBlaster/blaster", "RealPacks/KenneyBlaster/weapon", "Shotgun");
    public static GameObject Crate() => LoadFirst("Environment/Crate", "Environment/Crate01", "RealPacks/SciFiEssentials/Crate", "RealPacks/KenneySurvival/crate", "Crate");
    public static GameObject Barrel() => LoadFirst("Environment/Barrel", "Environment/Barrel01", "RealPacks/SciFiEssentials/Barrel", "RealPacks/KenneySurvival/barrel", "Barrel");
    public static GameObject RandomReal(string folder)
    {
        Object[] assets=Resources.LoadAll(folder,typeof(GameObject));
        if(assets!=null&&assets.Length>0)return assets[Random.Range(0,assets.Length)] as GameObject;
        return null;
    }
    static GameObject LoadFirst(params string[] paths)
    {
        foreach(string path in paths){GameObject g=Resources.Load<GameObject>(path);if(g!=null)return g;}
        return null;
    }
}