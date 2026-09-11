using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Camera playerCamera;
    public Transform muzzle;
    public float damage = 34f;
    public float range = 85f;
    public float shotsPerSecond = 1.15f;
    public int magazineSize = 12;
    public int reserveAmmo = 48;

    int magazine;
    float nextShot;

    void Awake()
    {
        magazine = magazineSize;
        if (!playerCamera) playerCamera = Camera.main;
    }

    void Update()
    {
        if (MobileControls.Fire && Time.time >= nextShot)
        {
            nextShot = Time.time + 1f / Mathf.Max(.1f, shotsPerSecond);
            Fire();
        }
    }

    public void Reload()
    {
        int need = magazineSize - magazine;
        int take = Mathf.Min(need, reserveAmmo);
        magazine += take;
        reserveAmmo -= take;
    }

    void Fire()
    {
        if (magazine <= 0) { Reload(); return; }
        magazine--;
        if (!playerCamera) playerCamera = Camera.main;
        if (!playerCamera) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            var health = hit.collider.GetComponentInParent<Health>();
            if (health) health.Damage(damage);
        }
    }
}
