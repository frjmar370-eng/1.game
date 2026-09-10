using UnityEngine;
using UnityEngine.UI;

public class CrosshairHUD : MonoBehaviour
{
    public ShotgunGame player;
    public Text crosshair;
    public Text status;
    float pulse;

    void Start(){if(!player)player=FindObjectOfType<ShotgunGame>();}
    void Update(){if(!player)return;bool canFire=!player.IsGameOver;float scale=1f+Mathf.Sin(Time.time*7f)*.04f;pulse=Mathf.MoveTowards(pulse,0f,Time.deltaTime*2.5f);if(crosshair){crosshair.text=canFire?"+":"×";crosshair.transform.localScale=Vector3.one*(scale+pulse*.8f);crosshair.color=canFire?Color.white:Color.red;}if(status){status.text=player.IsGameOver?"GAME OVER":"";}}
    public void HitConfirm(){pulse=.22f;}
}
