using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet2D : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;
    [Tooltip("Layers the bullet should die on (e.g., Ground, Default).")]
    public LayerMask destroyOnLayers;

    [HideInInspector] public Transform attacker;
    [HideInInspector] public float direction = 1f; // 🔹 Added — direction explicitly set by spawner

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        col.isTrigger = true;
    }

    void OnEnable()
    {
        Destroy(gameObject, lifetime);

        // 🔹 Start moving in the set direction
        rb.velocity = new Vector2(Mathf.Sign(direction) * Mathf.Abs(rb.velocity.x == 0 ? 5f : rb.velocity.x), 0f);

        // 🔹 Flip sprite visually if needed
        var spr = GetComponentInChildren<SpriteRenderer>();
        if (spr != null)
            spr.flipX = (rb.velocity.x < 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        var hp = other.GetComponentInParent<Player_health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);

            var kb = other.GetComponentInParent<KnockBack>();
            if (kb != null)
                kb.ApplyKnockback(attacker ? attacker.position : transform.position);

            Destroy(gameObject);
            return;
        }

        if ((destroyOnLayers.value & (1 << other.gameObject.layer)) != 0)
            Destroy(gameObject);
    }
}