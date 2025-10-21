using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;

    public Transform attacker;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null) return;
        eh.TakeDamage(damage);

        var kb = other.GetComponentInParent<KnockBack>();
        if (kb != null)
        {
            var source = attacker ? attacker.position : transform.position;
            kb.ApplyKnockback(source);
        }

        Destroy(gameObject);
    }
}
