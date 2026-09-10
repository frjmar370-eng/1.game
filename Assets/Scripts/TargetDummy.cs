using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    public int health = 3;
    public float moveRadius = 2.5f;
    public float moveSpeed = 1.2f;
    private Vector3 origin;
    private float phase;

    void Start()
    {
        origin = transform.position;
        phase = Random.value * 6.28f;
    }

    void Update()
    {
        Vector3 p = origin;
        p.x += Mathf.Sin(Time.time * moveSpeed + phase) * moveRadius;
        p.y += Mathf.Sin(Time.time * moveSpeed * 1.7f + phase) * 0.25f;
        transform.position = p;
        transform.Rotate(0, 30f * Time.deltaTime, 0);
    }

    public void Hit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
