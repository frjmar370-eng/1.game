using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    public int health = 3;
    public float moveRadius = 2.5f;
    public float moveSpeed = 1.2f;
    public int damageToPlayer = 8;
    public float attackInterval = 2.5f;

    Vector3 origin;
    float phase;
    float nextAttack;
    Renderer body;
    Color baseColor;
    Vector3 baseScale;

    void Start()
    {
        origin = transform.position;
        phase = Random.value * 6.28f;
        nextAttack = Time.time + Random.Range(1f, attackInterval);
        body = GetComponentInChildren<Renderer>();
        if (body) { baseColor = body.material.color; baseScale = transform.localScale; }
    }

    void Update()
    {
        Vector3 p = origin;
        p.x += Mathf.Sin(Time.time * moveSpeed + phase) * moveRadius;
        p.y += Mathf.Sin(Time.time * moveSpeed * 1.7f + phase) * 0.25f;
        transform.position = p;
        transform.Rotate(0, 30f * Time.deltaTime, 0);

        if (Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackInterval + Random.Range(.25f, 1f);
            ShotgunGame player = FindObjectOfType<ShotgunGame>();
            if (player && Vector3.Distance(transform.position, player.transform.position) < 18f)
                player.TakeDamage(damageToPlayer);
        }
    }

    public void Hit(int damage)
    {
        health -= damage;
        if (body) StartCoroutine(HitFlash());
        if (health <= 0)
        {
            GameObject fx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fx.name = "HitFX";
            fx.transform.position = transform.position;
            fx.transform.localScale = Vector3.one * .35f;
            Destroy(fx.GetComponent<Collider>());
            Destroy(fx, .12f);
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        body.material.color = Color.white;
        transform.localScale = baseScale * 1.08f;
        yield return new WaitForSeconds(.07f);
        if (body) body.material.color = baseColor;
        transform.localScale = baseScale;
    }
}
