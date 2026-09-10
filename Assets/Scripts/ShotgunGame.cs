using UnityEngine;
using UnityEngine.UI;

public class ShotgunGame : MonoBehaviour
{
    public Camera playerCamera; public bool thirdPerson; public float lookSensitivity=2.2f; public float moveSpeed=4.2f;
    public int maxAmmo=6,ammo=6; public float fireCooldown=.55f,range=45f; public int pellets=12; public float spread=.075f;
    public Text ammoText,scoreText,healthText,messageText; public MobileHUD mobileHUD; public bool IsGameOver=>gameOver;
    float nextFire; int score; int health=100; float yaw,pitch; bool reloading; float reloadDone; bool gameOver;
    float verticalVelocity;
    WeaponSystem weapons; ShotgunWeaponView weapon; CharacterController controller; Animator animator; Vector3 cameraVelocity;
    int idleHash,walkHash,runHash;
    void Start(){
        weapon=GetComponent<ShotgunWeaponView>();controller=GetComponent<CharacterController>();animator=GetComponentInChildren<Animator>();
        idleHash=Animator.StringToHash("Idle");walkHash=Animator.StringToHash("Walk");runHash=Animator.StringToHash("Run");
        if(GameSettings.Instance)lookSensitivity=GameSettings.Instance.sensitivity;weapons=WeaponSystem.Instance;
        if(playerCamera){yaw=transform.eulerAngles.y;playerCamera.transform.position=transform.position+new Vector3(0,2.4f,-5f);playerCamera.transform.LookAt(transform.position+Vector3.up*1.2f);}
        if(CombatFeedback.Instance==null)new GameObject("Combat Feedback").AddComponent<CombatFeedback>();
        if(VisualFX.Instance==null)new GameObject("Visual FX").AddComponent<VisualFX>();UpdateUI();
    }
    void Update(){
        if(gameOver)return;
        if(reloading&&Time.time>=reloadDone){reloading=false;if(weapons!=null)weapons.ReloadCurrent();else ammo=maxAmmo;if(messageText)messageText.text="";UpdateUI();}
        Look();Move();if(Input.GetMouseButton(0)||(mobileHUD&&mobileHUD.FireHeld))Fire();
        if(Input.GetKeyDown(KeyCode.R)||(mobileHUD&&mobileHUD.ReloadHeld))Reload();if(Input.GetKeyDown(KeyCode.Q))SwitchWeapon();
    }
    void Look(){
        Vector2 look=mobileHUD?mobileHUD.Look:Vector2.zero;
        if(look.sqrMagnitude>.001f){yaw+=look.x*lookSensitivity*2.2f;pitch-=look.y*lookSensitivity*2.2f;}
        else if(Input.touchCount==1){Touch t=Input.GetTouch(0);if(t.position.x>Screen.width*.35f&&t.phase==TouchPhase.Moved){yaw+=t.deltaPosition.x*lookSensitivity*.08f;pitch-=t.deltaPosition.y*lookSensitivity*.08f;}}
        else{yaw+=Input.GetAxis("Mouse X")*lookSensitivity;pitch-=Input.GetAxis("Mouse Y")*lookSensitivity;}
        pitch=Mathf.Clamp(pitch,-35f,55f);transform.rotation=Quaternion.Euler(0,yaw,0);if(!playerCamera)return;
        if(thirdPerson){
            Vector3 target=transform.position+Vector3.up*1.25f;Quaternion orbit=Quaternion.Euler(pitch,yaw,0);Vector3 desired=target+orbit*new Vector3(0,0,-5.2f);Vector3 dir=desired-target;float dist=dir.magnitude;
            if(dist>.01f&&Physics.SphereCast(target,.22f,dir.normalized,out RaycastHit hit,dist,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore))desired=target+dir.normalized*Mathf.Max(1.1f,hit.distance-.18f);
            playerCamera.transform.position=Vector3.SmoothDamp(playerCamera.transform.position,desired,ref cameraVelocity,.06f);
            Vector3 aimTarget=target+Vector3.up*Mathf.Clamp(pitch*.012f,-.4f,.55f);playerCamera.transform.rotation=Quaternion.LookRotation(aimTarget-playerCamera.transform.position,Vector3.up);
        }else playerCamera.transform.localRotation=Quaternion.Euler(pitch,0,0);
    }
    void Move(){
        Vector2 stick=mobileHUD?mobileHUD.Move:Vector2.zero;float x=stick.sqrMagnitude>.001f?stick.x:Input.GetAxis("Horizontal");float z=stick.sqrMagnitude>.001f?stick.y:Input.GetAxis("Vertical");
        Vector3 move=(transform.right*x+transform.forward*z);if(move.sqrMagnitude>1f)move.Normalize();float inputMagnitude=Mathf.Clamp01(new Vector2(x,z).magnitude);Vector3 horizontal=move*moveSpeed;
        if(controller){if(controller.isGrounded&&verticalVelocity<0f)verticalVelocity=-2f;else verticalVelocity+=Physics.gravity.y*Time.deltaTime;Vector3 velocity=(horizontal+Vector3.up*verticalVelocity)*Time.deltaTime;controller.Move(velocity);}
        else transform.position+=(horizontal+Vector3.up*verticalVelocity)*Time.deltaTime;
        UpdateAnimation(inputMagnitude);
    }
    void UpdateAnimation(float amount){if(!animator)return;int hash=amount<.05f?idleHash:(amount>.7f?runHash:walkHash);if(animator.HasState(0,hash)&&animator.GetCurrentAnimatorStateInfo(0).shortNameHash!=hash)animator.CrossFade(hash,.12f);}
    public void Fire(){
        if(gameOver||reloading)return;if(weapons==null)weapons=WeaponSystem.Instance;int w=weapons!=null?weapons.currentWeapon:0;
        if(weapons!=null){if(!weapons.Fire()){if(weapons.Ammo<=0)Reload();return;}}else{if(Time.time<nextFire)return;if(ammo<=0){Reload();return;}nextFire=Time.time+fireCooldown;ammo--;}
        if(weapon)weapon.FireKick();if(CombatFeedback.Instance)CombatFeedback.Instance.Shot();Vector3 muzzle=transform.position+transform.forward*.65f+Vector3.up*1.15f;if(VisualFX.Instance)VisualFX.Instance.Muzzle(muzzle);if(AudioManager.Instance)AudioManager.Instance.Shoot();
        Vector3 origin=playerCamera?playerCamera.transform.position:transform.position+Vector3.up*1.2f;Vector3 aim=playerCamera?playerCamera.transform.forward:transform.forward;
        if(w==0)for(int i=0;i<pellets;i++)HitScan(origin,(aim+Random.insideUnitSphere*spread),1);else HitScan(origin,aim,w==1?7:12);
        UpdateUI();if(weapons!=null&&weapons.Ammo<=0&&(GameSettings.Instance==null||GameSettings.Instance.autoReload))Reload();else if(weapons==null&&ammo==0)Reload();
    }
    void HitScan(Vector3 origin,Vector3 direction,int damage){if(Physics.Raycast(origin,direction.normalized,out RaycastHit hit,range,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore)){TargetDummy target=hit.collider.GetComponentInParent<TargetDummy>();if(target!=null){bool killed=target.Hit(damage);score+=killed?100:5;if(CombatFeedback.Instance)CombatFeedback.Instance.Hit();if(VisualFX.Instance){VisualFX.Instance.Hit();VisualFX.Instance.Impact(hit.point);}}}}
    public void SwitchWeapon(){if(weapons==null)return;weapons.Next();reloading=false;if(messageText)messageText.text=weapons.weaponNames[weapons.currentWeapon];if(AudioManager.Instance)AudioManager.Instance.Click();UpdateUI();}
    public void Reload(){if(gameOver||reloading)return;if(weapons!=null){if(weapons.Ammo>=weapons.maxAmmo[weapons.currentWeapon])return;}else if(ammo>=maxAmmo)return;reloading=true;reloadDone=Time.time+1.15f;if(messageText)messageText.text="RELOADING...";UpdateUI();}
    public void AddAmmo(int amount){amount=Mathf.Max(1,amount);if(weapons!=null)weapons.AddAmmo(amount);else ammo=Mathf.Min(maxAmmo,ammo+amount);if(messageText)messageText.text="AMMO +"+amount;UpdateUI();}
    public void Heal(int amount){int old=health;health=Mathf.Min(100,health+Mathf.Max(1,amount));if(health>old&&messageText)messageText.text="HEALTH +"+(health-old);UpdateUI();}
    public void TakeDamage(int amount){if(gameOver)return;health=Mathf.Max(0,health-Mathf.Max(0,amount));if(CombatFeedback.Instance)CombatFeedback.Instance.Shake(.12f);if(VisualFX.Instance)VisualFX.Instance.PlayerDamaged();if(AudioManager.Instance)AudioManager.Instance.Damage();UpdateUI();if(health==0){gameOver=true;if(messageText)messageText.text="GAME OVER";if(mobileHUD)mobileHUD.ShowRestart();}}
    void UpdateUI(){int currentAmmo=weapons!=null?weapons.Ammo:ammo;int currentMax=weapons!=null?weapons.maxAmmo[weapons.currentWeapon]:maxAmmo;string name=weapons!=null?weapons.weaponNames[weapons.currentWeapon]:"Shotgun";if(ammoText)ammoText.text=name+"  "+currentAmmo+"/"+currentMax;if(scoreText)scoreText.text="SCORE  "+score;if(healthText)healthText.text="HP  "+health;}
}
