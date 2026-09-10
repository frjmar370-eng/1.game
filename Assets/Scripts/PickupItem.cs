using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum Kind { Ammo, Health }
    public Kind kind = Kind.Ammo;
    public int amount = 2;
    public float spinSpeed = 90f;
    public float bobHeight = .18f;
    public float bobSpeed = 2.2f;
    Vector3 basePos;

    void Start(){basePos=transform.position;}
    void Update(){transform.Rotate(0,spinSpeed*Time.deltaTime,0,Space.World); transform.position=basePos+Vector3.up*(Mathf.Sin(Time.time*bobSpeed)*bobHeight);}
    void OnTriggerEnter(Collider other){ShotgunGame player=other.GetComponentInParent<ShotgunGame>();if(!player)return;if(kind==Kind.Ammo)player.AddAmmo(amount);else player.Heal(amount);Destroy(gameObject);}
}
