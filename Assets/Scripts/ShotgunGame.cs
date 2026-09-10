using UnityEngine;
using UnityEngine.UI;

public class ShotgunGame : MonoBehaviour
{
    [Header("Player")]
    public Camera playerCamera;
    public Transform weapon;
    public float lookSensitivity = 2.2f;
    public float moveSpeed = 4f;

    [Header("Combat")]
    public int maxAmmo = 6;
    public int ammo = 6;
    public float fireCooldown = 0.55f;
    public float range = 45f;
    public int pellets = 12;
    public float spread = 0.075f;

    [Header("UI")]
    public Text ammoText;
    public Text scoreText;
    public Text healthText;
    public Text messageText;

    private float nextFire;
    private int score;
    private int health = 100;
    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        UpdateUI();
    }

    void Update()
    {
        Look();
        Move();
        if (Input.GetMouseButton(0)) Fire();
        if (Input.GetKeyDown(KeyCode.R)) Reload();
    }

    void Look()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.position.x > Screen.width * 0.35f && t.phase == TouchPhase.Moved)
            {
                yaw += t.deltaPosition.x * lookSensitivity * 0.08f;
                pitch -= t.deltaPosition.y * lookSensitivity * 0.08f;
            }
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * lookSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
        }
        pitch = Mathf.Clamp(pitch, -75f, 75f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        if (playerCamera) playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    public void Fire()
    {
        if (Time.time < nextFire || ammo <= 0 || playerCamera == null) return;
        nextFire = Time.time + fireCooldown;
        ammo--;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = playerCamera.transform.forward;
            direction += Random.insideUnitSphere * spread;
            if (Physics.Raycast(playerCamera.transform.position, direction.normalized, out RaycastHit hit, range))
            {
                TargetDummy target = hit.collider.GetComponentInParent<TargetDummy>();
                if (target != null) { target.Hit(1); score += 10; }
            }
        }
        UpdateUI();
    }

    public void Reload()
    {
        ammo = maxAmmo;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        health = Mathf.Max(0, health - amount);
        UpdateUI();
        if (health == 0 && messageText) messageText.text = "انتهت اللعبة";
    }

    void UpdateUI()
    {
        if (ammoText) ammoText.text = "AMMO  " + ammo + "/" + maxAmmo;
        if (scoreText) scoreText.text = "SCORE  " + score;
        if (healthText) healthText.text = "HP  " + health;
    }
}
