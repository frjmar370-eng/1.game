using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum Kind { Ammo, Health }
    public Kind kind=Kind.Ammo;
    public int amount=2;
    public float spinSpeed=90f,bobHeight=.18f,bobSpeed=2.2f;
    Vector3 basePos;
    void Start(){basePos=transform.position;}
    void Update(){transform.Rotate(0,spinSpeed*Time.deltaTime,0,Space.World);transform.position=basePos+Vector3.up*(Mathf.Sin(Time.time*bobSpeed)*bobHeight);}
    void OnTriggerEnter(Collider other){
        ShotgunGame player=other.GetComponentInParent<ShotgunGame>();if(!player)return;
        if(kind==Kind.Ammo){
            if(WeaponSystem.Instance!=null)WeaponSystem.Instance.AddAmmo(amount);else player.AddAmmo(amount);
        }else player.Heal(amount);
        if(AudioManager.Instance)AudioManager.Instance.Pickup();
        if(VisualFX.Instance)VisualFX.Instance.Pickup(transform.position);
        Destroy(gameObject);
    }
}
