using UnityEngine;
using System.Collections;

public class KnockBack : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float strength;
    public float duration;

    private Rigidbody2D rb;
    private bool isKnocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 sourcePosition)
    {
        if (isKnocked) return;

        // Calculate direction from source to player
        Vector2 direction = (rb.position - sourcePosition).normalized;

        // Optional tweak: Reduce vertical push so it’s not only upwards
        direction.y = Mathf.Clamp(direction.y, -0.2f, 0.5f);

        rb.linearVelocity = Vector2.zero; // reset current velocity
        rb.AddForce(direction * strength, ForceMode2D.Impulse);

        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        
        var move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        yield return new WaitForSeconds(duration);

        if (move != null) move.enabled = true;
        isKnocked = false;
    }
}