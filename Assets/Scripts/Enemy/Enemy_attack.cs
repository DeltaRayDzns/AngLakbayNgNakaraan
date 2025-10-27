using UnityEngine;
using System.Collections;

public class Enemy_attack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 1f; 
    public int damage = 1;
    public float moveStopDuration = 0.5f;

    private float lastAttackTime;
    private EnemyAI_controller enemyAI;
    private bool isAttacking = false;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI_controller>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            Player_health playerHealth = other.GetComponent<Player_health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            KnockBack kb = other.GetComponent<KnockBack>();
            if (kb != null)
            {
                kb.ApplyKnockback(transform.position);
            }

            lastAttackTime = Time.time;

            if (!isAttacking)
                StartCoroutine(StopMovementTemporarily());
        }
    }

    private IEnumerator StopMovementTemporarily()
    {
        isAttacking = true;

        if (enemyAI != null)
        {
            float originalSpeed = enemyAI.speed;
            enemyAI.speed = 0f; 

            yield return new WaitForSeconds(moveStopDuration);

            enemyAI.speed = originalSpeed;
        }

        isAttacking = false;
    }
}
