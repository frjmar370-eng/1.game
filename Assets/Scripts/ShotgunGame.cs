using UnityEngine;
using UnityEngine.UI;

public class ShotgunGame : MonoBehaviour
{
    public Camera playerCamera;
    public float lookSensitivity = 2.2f;
    public float moveSpeed = 4f;
    public int maxAmmo = 6;
    public int ammo = 6;
    public float fireCooldown = .55f;
    public float range = 45f;
    public int pellets = 12;
    public float spread = .075f;
    public Text ammoText, scoreText, healthText, messageText;
    public MobileHUD mobileHUD;
    public bool IsGameOver => gameOver;

    float nextFire;
    int score;
    int health = 100;
    float yaw, pitch;
    bool reloading;
    float reloadDone;
    bool gameOver;
    ShotgunWeaponView weapon;

    void Start() { weapon = GetComponent<ShotgunWeaponView>(); UpdateUI(); }

    void Update()
    {
        if (gameOver) return;
        if (reloading && Time.time >= reloadDone) { reloading = false; ammo = maxAmmo; if (messageText) messageText.text = ""; UpdateUI(); }
        Look(); Move();
        bool fire = Input.GetMouseButton(0) || (mobileHUD && mobileHUD.FireHeld);
        if (fire) Fire();
        if (Input.GetKeyDown(KeyCode.R) || (mobileHUD && mobileHUD.ReloadHeld)) Reload();
    }

    void Look()
    {
        Vector2 look = mobileHUD ? mobileHUD.Look : Vector2.zero;
        if (look.sqrMagnitude > .001f) { yaw += look.x * lookSensitivity * 2.2f; pitch -= look.y * lookSensitivity * 2.2f; }
        else if (Input.touchCount == 1) { Touch t = Input.GetTouch(0); if (t.position.x > Screen.width * .35f && t.phase == TouchPhase.Moved) { yaw += t.deltaPosition.x * lookSensitivity * .08f; pitch -= t.deltaPosition.y * lookSensitivity * .08f; } }
        else { yaw += Input.GetAxis("Mouse X") * lookSensitivity; pitch -= Input.GetAxis("Mouse Y") * lookSensitivity; }
        pitch = Mathf.Clamp(pitch, -75f, 75f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        if (playerCamera) playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void Move()
    {
        Vector2 stick = mobileHUD ? mobileHUD.Move : Vector2.zero;
        float x = stick.sqrMagnitude > .001f ? stick.x : Input.GetAxis("Horizontal");
        float z = stick.sqrMagnitude > .001f ? stick.y : Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z) * moveSpeed * Time.deltaTime;
        CharacterController cc = GetComponent<CharacterController>();
        if (cc) cc.Move(move); else transform.position += move;
    }

    public void Fire()
    {
        if (gameOver || reloading || Time.time < nextFire || ammo <= 0 || playerCamera == null) return;
        nextFire = Time.time + fireCooldown; ammo--;
        if (weapon) weapon.FireKick();
        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = playerCamera.transform.forward + Random.insideUnitSphere * spread;
            if (Physics.Raycast(playerCamera.transform.position, direction.normalized, out RaycastHit hit, range))
            {
                TargetDummy target = hit.collider.GetComponentInParent<TargetDummy>();
                if (target != null) { bool killed = target.Hit(1); score += killed ? 100 : 5; }
            }
        }
        UpdateUI();
    }

    public void Reload()
    {
        if (gameOver || reloading || ammo == maxAmmo) return;
        reloading = true;
        reloadDone = Time.time + 1.15f;
        if (messageText) messageText.text = "RELOADING...";
    }

    public void TakeDamage(int amount)
    {
        if (gameOver) return;
        health = Mathf.Max(0, health - amount); UpdateUI();
        if (health == 0)
        {
            gameOver = true;
            if (messageText) messageText.text = "GAME OVER";
            if (mobileHUD) mobileHUD.ShowRestart();
        }
    }

    void UpdateUI()
    {
        if (ammoText) ammoText.text = reloading ? "RELOADING" : "AMMO  " + ammo + "/" + maxAmmo;
        if (scoreText) scoreText.text = "SCORE  " + score;
        if (healthText) healthText.text = "HP  " + health;
    }
}
