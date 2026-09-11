using UnityEngine;

public static class ArtAssetResolver
{
    static GameObject Load(string name)
    {
        GameObject remote = RuntimeAssetBundleStore.LoadPrefab(name);
        if (remote != null) return remote;
        return null;
    }

    public static GameObject Player()=>Load("Player");
    public static GameObject Enemy()=>Load("Enemy");
    public static GameObject Boss()=>Load("Boss") ?? Load("Enemy");
    public static GameObject Shotgun()=>Load("Shotgun");
    public static GameObject Rifle()=>Load("Rifle");
    public static GameObject Pistol()=>Load("Pistol");
    public static GameObject Crate()=>Load("Crate");
    public static GameObject Barrel()=>Load("Barrel");
    public static GameObject Floor()=>Load("Floor");
    public static GameObject Wall()=>Load("Wall");
    public static GameObject Door()=>Load("Door");
    public static GameObject Cover()=>Load("Cover");
    public static GameObject RandomReal(string folder)=>null;
}
