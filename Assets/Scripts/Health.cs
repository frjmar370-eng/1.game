using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float Current { get; private set; }
    public bool IsDead => Current <= 0f;

    void Awake() => Current = maxHealth;

    public void Damage(float amount)
    {
        if (IsDead) return;
        Current = Mathf.Max(0f, Current - Mathf.Max(0f, amount));
        if (Current <= 0f) gameObject.SetActive(false);
    }
}
