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

    Animator anim;
    [SerializeField] private string stAttack = "Attack";
    private int attackID;

    private void Awake()
    {
        attackID = Animator.StringToHash(stAttack);
    }

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI_controller>();
        anim = GetComponentInParent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            var playerHealth = other.GetComponent<Player_health>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);

            var kb = other.GetComponent<KnockBack>();
            if (kb != null) kb.ApplyKnockback(transform.position);

            lastAttackTime = Time.time;

            if (anim) anim.CrossFade(attackID, 0.03f); 

            if (!isAttacking) StartCoroutine(StopMovementTemporarily());
        }
    }

    private IEnumerator StopMovementTemporarily()
    {
        isAttacking = true;
        if (enemyAI != null)
        {
            float original = enemyAI.speed;
            enemyAI.speed = 0f;
            yield return new WaitForSeconds(moveStopDuration);
            enemyAI.speed = original;
        }
        isAttacking = false;
    }
}