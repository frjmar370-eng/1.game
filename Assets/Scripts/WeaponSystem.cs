using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance { get; private set; }
    public int currentWeapon;
    public readonly string[] weaponNames={"Shotgun","Rifle","Pistol"};
    public readonly int[] magazineSize={6,30,12};
    public readonly int[] maxAmmo={48,180,72};
    public readonly int[] damage={18,7,12};
    public readonly float[] cooldown={.65f,.12f,.25f};
    int[] ammo={6,30,12};
    int[] reserve={42,150,60};
    float nextShot;

    void Awake(){if(Instance&&Instance!=this){Destroy(gameObject);return;}Instance=this;DontDestroyOnLoad(gameObject);}
    public int Ammo=>ammo[currentWeapon];
    public int Reserve=>reserve[currentWeapon];
    public bool CanFire(){return Time.time>=nextShot&&ammo[currentWeapon]>0;}
    public bool Fire(){if(!CanFire())return false;ammo[currentWeapon]--;nextShot=Time.time+cooldown[currentWeapon];return true;}
    public void Switch(int index){currentWeapon=Mathf.Clamp(index,0,weaponNames.Length-1);nextShot=0f;}
    public void Next(){Switch((currentWeapon+1)%weaponNames.Length);}
    public void AddAmmo(int amount){reserve[currentWeapon]=Mathf.Min(maxAmmo[currentWeapon]-ammo[currentWeapon],reserve[currentWeapon]+Mathf.Max(0,amount));}
    public void ReloadCurrent(){int need=magazineSize[currentWeapon]-ammo[currentWeapon];if(need<=0||reserve[currentWeapon]<=0)return;int loaded=Mathf.Min(need,reserve[currentWeapon]);ammo[currentWeapon]+=loaded;reserve[currentWeapon]-=loaded;}
    public void Refill(){for(int i=0;i<ammo.Length;i++){ammo[i]=magazineSize[i];reserve[i]=maxAmmo[i]-magazineSize[i];}nextShot=0f;}
}
