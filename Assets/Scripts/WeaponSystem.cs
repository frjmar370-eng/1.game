using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance { get; private set; }
    public int currentWeapon;
    public readonly string[] weaponNames={"Shotgun","Rifle","Pistol"};
    public readonly int[] maxAmmo={24,90,60};
    public readonly int[] damage={18,7,12};
    public readonly float[] cooldown={.65f,.12f,.25f};
    int[] ammo={24,90,60};
    float nextShot;

    void Awake(){if(Instance&&Instance!=this){Destroy(gameObject);return;}Instance=this;DontDestroyOnLoad(gameObject);}
    public int Ammo=>ammo[currentWeapon];
    public bool CanFire(){return Time.time>=nextShot&&ammo[currentWeapon]>0;}
    public bool Fire(){if(!CanFire())return false;ammo[currentWeapon]--;nextShot=Time.time+cooldown[currentWeapon];return true;}
    public void Switch(int index){currentWeapon=Mathf.Clamp(index,0,weaponNames.Length-1);}
    public void Next(){Switch((currentWeapon+1)%weaponNames.Length);}
    public void AddAmmo(int amount){ammo[currentWeapon]=Mathf.Min(maxAmmo[currentWeapon],ammo[currentWeapon]+amount);}
    public void Refill(){for(int i=0;i<ammo.Length;i++)ammo[i]=maxAmmo[i];}
} 
