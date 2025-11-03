using UnityEngine;
using System.Collections;

public class Enemy_attack_ranged : MonoBehaviour
{
    [Header("Who to shoot")]
    public string playerTag = "Player";

    [Header("Projectile")]
    public GameObject bulletPrefab;     
    public Transform firePoint;         
    public float bulletSpeed = 12f;

    [Header("Stats")]
    public float range = 8f;
    public int damage = 1;
    public float reload = 1.0f;

    [Header("Animation")]
    [SerializeField] private Animator anim;           // gets from parent at Start
    [SerializeField] private string stAttack = "Attack";
    [SerializeField] private float attackFade = 0.03f;
    private int attackID;

    private Rigidbody2D enemyRb; 
    private RigidbodyConstraints2D originalCons;
    private bool playerInLOS = false;
    private bool firingLoopRunning = false;
    private Transform targetPlayer;

    void Awake()
    {
        enemyRb = GetComponentInParent<Rigidbody2D>();
        if (!firePoint) firePoint = transform;
        if (enemyRb) originalCons = enemyRb.constraints;

        attackID = Animator.StringToHash(stAttack);   // cache hash like melee script
    }

    void Start()
    {
        if (!anim) anim = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        targetPlayer = other.transform;
        playerInLOS = true;

        if (enemyRb)
            enemyRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        if (!firingLoopRunning)
            StartCoroutine(FireLoop());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInLOS = false;
        targetPlayer = null;

        if (enemyRb)
            enemyRb.constraints = originalCons;
    }

    IEnumerator FireLoop()
    {
        firingLoopRunning = true;

        while (playerInLOS)
        {
            Shoot();
            yield return new WaitForSeconds(reload);
        }

        firingLoopRunning = false;
    }

    void Shoot()
    {
        if (!bulletPrefab) return;

        // Trigger Attack animation (same behavior as melee)
        if (anim) anim.CrossFade(attackID, attackFade);

        Vector2 dir;
        if (targetPlayer != null)
            dir = (targetPlayer.position.x < transform.position.x) ? Vector2.left : Vector2.right;
        else
        {
            float face = Mathf.Sign(transform.localScale.x);
            dir = (face >= 0) ? Vector2.right : Vector2.left;
        }
        dir.Normalize();

        GameObject b = Instantiate(bulletPrefab,
                                   firePoint != null ? firePoint.position : transform.position,
                                   Quaternion.identity);

        var rb2d = b.GetComponent<Rigidbody2D>();
        if (rb2d)
            rb2d.velocity = dir * bulletSpeed;
        else
            Debug.LogError("Spawned bullet has no Rigidbody2D!");

        var eb = b.GetComponent<EnemyBullet2D>();
        if (eb)
        {
            eb.attacker = transform;
            eb.damage = damage;
            eb.lifetime = Mathf.Max(0.05f, range / Mathf.Max(0.01f, bulletSpeed));
            eb.direction = dir.x;
        }

        var spr = b.GetComponentInChildren<SpriteRenderer>();
        if (spr != null)
            spr.flipX = (dir.x < 0f);

        Debug.Log($"[Shoot] -> dir={dir}, velocity={dir * bulletSpeed}");
    }

    void OnDrawGizmosSelected()
    {
        if (!firePoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.right * range);
        Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.left * range);
    }
}
