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

    private bool isAttacking = false;

    void Update()
    {
        if (!inventoryManager || inventoryManager.equippedWeapon == null) return;

        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
            StartCoroutine(PerformAttack(inventoryManager.equippedWeapon));
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

        float cooldown = 1f / Mathf.Max(0.01f, w.attackSpeed);
        yield return new WaitForSeconds(cooldown);
        isAttacking = false;
    }

    // melee attack para sa mga swords, etc.
    IEnumerator SwingAttack(WeaponData w)
    {
        if (!attackOrigin) attackOrigin = transform;
        yield return new WaitForSeconds(0.05f);     
        DoMeleeHit2D(w);
    }

    void DoMeleeHit2D(WeaponData w)
    {
        float reach = w.reach > 0 ? w.reach : attackRadius;

        Collider2D[] cols = Physics2D.OverlapCircleAll(
            (Vector2)attackOrigin.position,
            reach,
            hittableLayer
        );

        var unique = new HashSet<EnemyHealth>();
        for (int i = 0; i < cols.Length; i++)
        {
            var eh = cols[i].GetComponentInParent<EnemyHealth>();
            if (eh && unique.Add(eh))
            {
                eh.TakeDamage(w.damage);

                var kb = cols[i].GetComponentInParent<KnockBack>();
                if (kb != null)
                    kb.ApplyKnockback(transform.position);
            }
        }
    }

    //  ranged para sa baril
    IEnumerator RangedAttack(WeaponData w)
    {
        if (!bulletPrefab) yield break;
        if (!attackOrigin) attackOrigin = transform;

        GameObject bullet = Instantiate(bulletPrefab, attackOrigin.position, Quaternion.identity);

        var rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d)
        {
            Vector2 dir = attackOrigin.right;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right * Mathf.Sign(transform.lossyScale.x);
            rb2d.linearVelocity = dir.normalized * bulletSpeed;
        }

        var b = bullet.GetComponent<Bullet>();  
        if (b)
        {
            b.damage  = w.damage;
            b.attacker = transform;             
        }

        yield return null;
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
