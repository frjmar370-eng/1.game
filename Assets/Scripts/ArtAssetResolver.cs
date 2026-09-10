using UnityEngine;

public static class ArtAssetResolver
{
    public static GameObject Player() => LoadFirst(
        "Characters/Player", "Characters/Soldier", "RealPacks/SciFiEssentials/Character", "Player");

    public static GameObject Enemy() => LoadByKeywords(
        new[] { "enemy", "robot", "zombie", "alien", "character" },
        "RealPacks/SciFiEssentials");

    public static GameObject Shotgun() => LoadByKeywords(
        new[] { "shotgun", "blaster", "gun", "weapon" },
        "RealPacks/SciFiEssentials");

    public static GameObject Crate() => LoadByKeywords(
        new[] { "crate", "box", "container" },
        "RealPacks/SciFiEssentials");

    public static GameObject Barrel() => LoadByKeywords(
        new[] { "barrel", "drum" },
        "RealPacks/SciFiEssentials");

    public static GameObject RandomReal(string folder)
    {
        Object[] assets = Resources.LoadAll(folder, typeof(GameObject));
        if (assets != null && assets.Length > 0)
            return assets[Random.Range(0, assets.Length)] as GameObject;
        return null;
    }

    static GameObject LoadFirst(params string[] paths)
    {
        foreach (string path in paths)
        {
            GameObject g = Resources.Load<GameObject>(path);
            if (g != null) return g;
        }
        return null;
    }

    static GameObject LoadByKeywords(string[] keywords, string folder)
    {
        Object[] assets = Resources.LoadAll(folder, typeof(GameObject));
        if (assets == null || assets.Length == 0) return null;

        foreach (Object asset in assets)
        {
            string n = asset.name.ToLowerInvariant();
            foreach (string keyword in keywords)
            {
                if (n.Contains(keyword)) return asset as GameObject;
            }
        }

        return assets[Random.Range(0, assets.Length)] as GameObject;
    }
}
