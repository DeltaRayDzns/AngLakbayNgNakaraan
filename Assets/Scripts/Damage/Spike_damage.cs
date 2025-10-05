using UnityEngine;

public class Spike_damage : MonoBehaviour
{
    public Player_health playerHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage
            if (playerHealth != null)
                playerHealth.TakeDamage(1);

            // Apply knockback
            KnockBack kb = collision.gameObject.GetComponent<KnockBack>();
            if (kb != null)
                kb.ApplyKnockback(transform.position);
        }
    }
}
