using UnityEngine;
using UnityEngine.UI;

public class ShotgunGame : MonoBehaviour
{
    public Camera playerCamera;
    public bool thirdPerson;
    public float lookSensitivity=2.2f;
    public float moveSpeed=4.2f;
    public int maxAmmo=6;
    public int ammo=6;
    public float fireCooldown=.55f;
    public float range=45f;
    public int pellets=12;
    public float spread=.075f;
    public Text ammoText,scoreText,healthText,messageText;
    public MobileHUD mobileHUD;
    public bool IsGameOver=>gameOver;

    float nextFire;int score;int health=100;float yaw,pitch;bool reloading;float reloadDone;bool gameOver;ShotgunWeaponView weapon;CharacterController controller;Animator animator;Vector3 cameraVelocity;
    int idleHash,walkHash,runHash;

    void Start(){weapon=GetComponent<ShotgunWeaponView>();controller=GetComponent<CharacterController>();animator=GetComponentInChildren<Animator>();idleHash=Animator.StringToHash("Idle");walkHash=Animator.StringToHash("Walk");runHash=Animator.StringToHash("Run");if(playerCamera)playerCamera.transform.position=transform.position+new Vector3(0,2.4f,-5f);UpdateUI();}
    void Update(){if(gameOver)return;if(reloading&&Time.time>=reloadDone){reloading=false;ammo=maxAmmo;if(messageText)messageText.text="";UpdateUI();}Look();Move();bool fire=Input.GetMouseButton(0)||(mobileHUD&&mobileHUD.FireHeld);if(fire)Fire();if(Input.GetKeyDown(KeyCode.R)||(mobileHUD&&mobileHUD.ReloadHeld))Reload();}

    void Look(){Vector2 look=mobileHUD?mobileHUD.Look:Vector2.zero;if(look.sqrMagnitude>.001f){yaw+=look.x*lookSensitivity*2.2f;pitch-=look.y*lookSensitivity*2.2f;}else if(Input.touchCount==1){Touch t=Input.GetTouch(0);if(t.position.x>Screen.width*.35f&&t.phase==TouchPhase.Moved){yaw+=t.deltaPosition.x*lookSensitivity*.08f;pitch-=t.deltaPosition.y*lookSensitivity*.08f;}}else{yaw+=Input.GetAxis("Mouse X")*lookSensitivity;pitch-=Input.GetAxis("Mouse Y")*lookSensitivity;}pitch=Mathf.Clamp(pitch,-35f,55f);transform.rotation=Quaternion.Euler(0,yaw,0);
        if(!playerCamera)return;if(thirdPerson){Vector3 target=transform.position+Vector3.up*1.25f;Quaternion orbit=Quaternion.Euler(pitch,yaw,0);Vector3 desired=target+orbit*new Vector3(0,0,-5.2f);Vector3 dir=desired-target;float dist=dir.magnitude;if(Physics.SphereCast(target,.22f,dir.normalized,out RaycastHit hit,dist,~0,QueryTriggerInteraction.Ignore))desired=target+dir.normalized*Mathf.Max(1.1f,hit.distance-.18f);playerCamera.transform.position=Vector3.SmoothDamp(playerCamera.transform.position,desired,ref cameraVelocity,.06f);playerCamera.transform.rotation=Quaternion.LookRotation((target+Vector3.up*(pitch*.012f))-playerCamera.transform.position,Vector3.up);}else playerCamera.transform.localRotation=Quaternion.Euler(pitch,0,0);}

    void Move(){Vector2 stick=mobileHUD?mobileHUD.Move:Vector2.zero;float x=stick.sqrMagnitude>.001f?stick.x:Input.GetAxis("Horizontal");float z=stick.sqrMagnitude>.001f?stick.y:Input.GetAxis("Vertical");Vector3 move=(transform.right*x+transform.forward*z);if(move.sqrMagnitude>1f)move.Normalize();float inputMagnitude=Mathf.Clamp01(move.magnitude);move*=moveSpeed*Time.deltaTime;if(controller){if(!controller.isGrounded)move.y-=9.81f*Time.deltaTime;controller.Move(move);}else transform.position+=move;UpdateAnimation(inputMagnitude);}
    void UpdateAnimation(float amount){if(!animator)return;if(amount<.05f){if(animator.HasState(0,idleHash))animator.CrossFade(idleHash,.15f);return;}int hash=amount>.7f?runHash:walkHash;if(animator.HasState(0,hash))animator.CrossFade(hash,.12f);}

    public void Fire(){if(gameOver||reloading||Time.time<nextFire||ammo<=0||playerCamera==null)return;nextFire=Time.time+fireCooldown;ammo--;if(weapon)weapon.FireKick();Vector3 aimOrigin=playerCamera.transform.position;Vector3 aimDir=playerCamera.transform.forward;for(int i=0;i<pellets;i++){Vector3 direction=aimDir+Random.insideUnitSphere*spread;if(Physics.Raycast(aimOrigin,direction.normalized,out RaycastHit hit,range,~0,QueryTriggerInteraction.Ignore)){TargetDummy target=hit.collider.GetComponentInParent<TargetDummy>();if(target!=null){bool killed=target.Hit(1);score+=killed?100:5;}}}UpdateUI();}
    public void Reload(){if(gameOver||reloading||ammo==maxAmmo)return;reloading=true;reloadDone=Time.time+1.15f;if(messageText)messageText.text="RELOADING...";}
    public void TakeDamage(int amount){if(gameOver)return;health=Mathf.Max(0,health-amount);UpdateUI();if(health==0){gameOver=true;if(messageText)messageText.text="GAME OVER";if(mobileHUD)mobileHUD.ShowRestart();}}
    void UpdateUI(){if(ammoText)ammoText.text=reloading?"RELOADING":"AMMO  "+ammo+"/"+maxAmmo;if(scoreText)scoreText.text="SCORE  "+score;if(healthText)healthText.text="HP  "+health;}
}
