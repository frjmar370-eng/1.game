using UnityEngine;

public static class ArtAssetResolver
{
    static GameObject Load(string path)=>Resources.Load<GameObject>(path);

    public static GameObject Player()=>First("RealPacks/Characters/Player","RealPacks/Characters/PlayerCharacter","Characters/Player");
    public static GameObject Enemy()=>First("RealPacks/Characters/Enemy","RealPacks/Characters/EnemyCharacter","Characters/Enemy");
    public static GameObject Shotgun()=>First("RealPacks/Weapons/Shotgun","Weapons/Shotgun");
    public static GameObject Rifle()=>First("RealPacks/Weapons/Rifle","Weapons/Rifle");
    public static GameObject Pistol()=>First("RealPacks/Weapons/Pistol","Weapons/Pistol");
    public static GameObject Crate()=>First("RealPacks/Environment/Crate","RealPacks/Environment/Container","Environment/Crate");
    public static GameObject Barrel()=>First("RealPacks/Environment/Barrel","RealPacks/Environment/Drum","Environment/Barrel");

    // Compatibility API: deliberately deterministic; never selects an arbitrary asset.
    public static GameObject RandomReal(string folder)=>null;

    static GameObject First(params string[] paths){foreach(string path in paths){GameObject g=Load(path);if(g!=null)return g;}return null;}
}
