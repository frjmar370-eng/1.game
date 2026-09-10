using UnityEngine;

/// <summary>Central resolver for imported art.</summary>
public static class ArtAssetResolver
{
    public static GameObject Player() => LoadFirst("Characters/Player", "Characters/Soldier", "Player");
    public static GameObject Enemy() => LoadFirst("Characters/Enemy", "Characters/Zombie", "Enemy");
    public static GameObject Shotgun() => LoadFirst("Weapons/Shotgun", "Weapons/Shotgun01", "Shotgun");
    public static GameObject Crate() => LoadFirst("Environment/Crate", "Environment/Crate01", "Crate");
    public static GameObject Barrel() => LoadFirst("Environment/Barrel", "Environment/Barrel01", "Barrel");

    static GameObject LoadFirst(params string[] paths)
    {
        foreach(string path in paths)
        {
            GameObject g=Resources.Load<GameObject>(path);
            if(g!=null)return g;
        }
        return null;
    }
}
