using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public Inventory_manager inventoryManager; 
    public Transform attackOrigin; 
    public LayerMask hittableLayer; 
    public GameObject bulletPrefab;      

    [Header("Stats")]
    public float bulletSpeed = 15f;
    public float attackRadius = 1.5f;

    [Header("Fire Control")]
    [Tooltip("Seconds per attack = attackSpeed * this. Example: 4 -> 0.4s when set to 0.1")]
    public float attackSpeedUnitSeconds = 0.1f;

    private float nextAttackTime = 0f; 

    private bool isAttacking = false;

    void Update()
    {
        if (!inventoryManager || inventoryManager.equippedWeapon == null) return;

        
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartAttack(); 
        }
    }

    public void StartAttack() 
    {	
        if (!inventoryManager || inventoryManager.equippedWeapon == null) return;

        var w = inventoryManager.equippedWeapon;
        float cooldown = GetCooldownSeconds(w);

        if (Time.time < nextAttackTime || isAttacking) return;

        nextAttackTime = Time.time + cooldown;

        StartCoroutine(PerformAttack(w));
    }

    float GetCooldownSeconds(WeaponData w)
    {
        return Mathf.Max(0.01f, w.attackSpeed * attackSpeedUnitSeconds);
    }

    IEnumerator PerformAttack(WeaponData w)
    {
        if (!w) yield break;

        isAttacking = true;

        switch (w.pattern)
        {
            case AttackPattern.Swing:  yield return SwingAttack(w);  break;
            case AttackPattern.Ranged: yield return RangedAttack(w); break;
            default:                   yield return SwingAttack(w);  break;
        }

        yield return new WaitForSeconds(GetCooldownSeconds(w));
        isAttacking = false;
    }

    // para sa melle
    IEnumerator SwingAttack(WeaponData w)
    {
        if (!attackOrigin) attackOrigin = transform;
        yield return new WaitForSeconds(0.05f);
        DoMeleeHit2D(w);
    }

    void DoMeleeHit2D(WeaponData w)
    {
        float reach = w.reach > 0 ? w.reach : attackRadius;
        Collider2D[] cols = Physics2D.OverlapCircleAll((Vector2)attackOrigin.position, reach, hittableLayer);

        var unique = new HashSet<EnemyHealth>();
        for (int i = 0; i < cols.Length; i++)
        {
            var node = cols[i].GetComponentInParent<Boss_questionNodes>();
            if (node) { node.OnHit(); continue; }

            var eh = cols[i].GetComponentInParent<EnemyHealth>();
            if (eh && unique.Add(eh))
            {
                eh.TakeDamage(w.damage);
                var kb = cols[i].GetComponentInParent<KnockBack>();
                if (kb) kb.ApplyKnockback(transform.position);
            }
        }
    }

    // para sa ranged
    IEnumerator RangedAttack(WeaponData w)
    {
        if (!bulletPrefab) yield break;
        if (!attackOrigin) attackOrigin = transform;

        GameObject bullet = Instantiate(bulletPrefab, attackOrigin.position, Quaternion.identity);
        var rb2d = EnsureBulletSetup(bullet);

        float facing = Mathf.Sign(transform.lossyScale.x);
        if (Mathf.Approximately(facing, 0f)) facing = 1f;
        Vector2 dir = new Vector2(facing, 0f).normalized;
        rb2d.velocity = dir * bulletSpeed;

        float lifetime = Mathf.Max(0.05f, (w.reach > 0 ? w.reach : attackRadius * 3f) / Mathf.Max(0.01f, bulletSpeed));
        Destroy(bullet, lifetime);

        var b1 = bullet.GetComponent<Bullet>();
        if (b1) { b1.damage = w.damage; b1.attacker = transform; }
        else
        {
            var b2 = bullet.GetComponent<EnemyBullet2D>();
            if (b2) { b2.damage = w.damage; b2.attacker = transform; b2.lifetime = lifetime; }
        }

        IgnorePlayerCollisionWithBullet(bullet);
        yield return null;
    }

    Rigidbody2D EnsureBulletSetup(GameObject bullet)
    {
        var rb2d = bullet.GetComponent<Rigidbody2D>();
        if (!rb2d) rb2d = bullet.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.gravityScale = 0f;
        rb2d.freezeRotation = true;
        rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = bullet.GetComponent<Collider2D>();
        if (!col) col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        return rb2d;
    }

    void IgnorePlayerCollisionWithBullet(GameObject bullet)
    {	
        var bulletCols = bullet.GetComponentsInChildren<Collider2D>();
        var playerCols = GetComponentsInChildren<Collider2D>();
        foreach (var bc in bulletCols)
            foreach (var pc in playerCols)
                if (bc && pc) Physics2D.IgnoreCollision(bc, pc, true);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!attackOrigin) return;
        Gizmos.color = Color.red;
        float r = inventoryManager?.equippedWeapon?.reach ?? attackRadius;
        Gizmos.DrawWireSphere(attackOrigin.position, r);
    }
#endif
}
