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
        if (!inventoryManager)
        {
            Debug.LogWarning("[PlayerAttack] Missing Inventory Manager reference!");
            return;
        }

        if (inventoryManager.equippedWeapon == null)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            Debug.Log("[PlayerAttack] E pressed → Performing attack!");
            StartCoroutine(PerformAttack(inventoryManager.equippedWeapon));
        }
    }

	public void StartAttack() 
	{	
		if (!isAttacking && inventoryManager && inventoryManager.equippedWeapon != null)
		{
			StartCoroutine(PerformAttack(inventoryManager.equippedWeapon));
		}
	}

    IEnumerator PerformAttack(WeaponData w)
    {
        if (!w)
        {
            Debug.LogWarning("[PlayerAttack] No weapon data found.");
            yield break;
        }

        isAttacking = true;
        Debug.Log($"[PlayerAttack] Attack started using {w.name}, pattern = {w.pattern}");

        switch (w.pattern)
        {
            case AttackPattern.Swing:  
                yield return SwingAttack(w);  
                break;

            case AttackPattern.Ranged: 
                yield return RangedAttack(w); 
                break;

            default:                   
                Debug.LogWarning("[PlayerAttack] Unknown attack pattern, defaulting to Swing.");
                yield return SwingAttack(w);  
                break;
        }

        float cooldown = 1f / Mathf.Max(0.01f, w.attackSpeed);
        Debug.Log($"[PlayerAttack] Attack cooldown = {cooldown:F2}s");
        yield return new WaitForSeconds(cooldown);

        isAttacking = false;
        Debug.Log("[PlayerAttack] Attack ready again.");
    }

    // para sa melee
    IEnumerator SwingAttack(WeaponData w)
    {
        if (!attackOrigin)
        {
            attackOrigin = transform;
            Debug.LogWarning("[PlayerAttack] No attackOrigin assigned, using player transform instead.");
        }

        yield return new WaitForSeconds(0.05f);
        Debug.Log("[PlayerAttack] SwingAttack triggered.");
        DoMeleeHit2D(w);
    }

    void DoMeleeHit2D(WeaponData w)
    {
        float reach = w.reach > 0 ? w.reach : attackRadius;
        Debug.Log($"[PlayerAttack] Checking for hits at {attackOrigin.position} with reach {reach}");

        Collider2D[] cols = Physics2D.OverlapCircleAll(
            (Vector2)attackOrigin.position,
            reach,
            hittableLayer
        );

        Debug.Log($"[PlayerAttack] Found {cols.Length} colliders in range.");

        var unique = new HashSet<EnemyHealth>();

        for (int i = 0; i < cols.Length; i++)
        {
            Collider2D col = cols[i];
            Debug.Log($"[PlayerAttack] Hit: {col.name}");

            var node = col.GetComponentInParent<Boss_questionNodes>();
            if (node)
            {
                Debug.Log($"[PlayerAttack] Detected Boss_questionNodes on {col.name} → calling OnHit()");
                node.OnHit();
                continue;
            }

            var eh = col.GetComponentInParent<EnemyHealth>();
            if (eh)
            {
                if (unique.Add(eh))
                {
                    Debug.Log($"[PlayerAttack] Damaging enemy {eh.name} for {w.damage} damage.");
                    eh.TakeDamage(w.damage);

                    var kb = col.GetComponentInParent<KnockBack>();
                    if (kb != null)
                    {
                        Debug.Log($"[PlayerAttack] Applying knockback to {eh.name}");
                        kb.ApplyKnockback(transform.position);
                    }
                }
                else
                {
                    Debug.Log($"[PlayerAttack] Skipped duplicate enemy {eh.name}");
                }
            }
            else
            {
                Debug.Log($"[PlayerAttack] No EnemyHealth or Node component found on {col.name}");
            }
        }

        if (cols.Length == 0)
        {
            Debug.Log("[PlayerAttack] No objects hit. Check hittableLayer and collider positions.");
        }
    }

    // para sa ranged 
    IEnumerator RangedAttack(WeaponData w)
    {
        if (!bulletPrefab)
        {
            Debug.LogWarning("[PlayerAttack] Missing bulletPrefab!");
            yield break;
        }

        if (!attackOrigin)
        {
            attackOrigin = transform;
            Debug.LogWarning("[PlayerAttack] No attackOrigin, using player transform instead.");
        }

        GameObject bullet = Instantiate(bulletPrefab, attackOrigin.position, Quaternion.identity);
        Debug.Log($"[PlayerAttack] Spawned bullet {bullet.name}");

        var rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d)
        {
            Vector2 dir = attackOrigin.right;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right * Mathf.Sign(transform.lossyScale.x);

            rb2d.linearVelocity = dir.normalized * bulletSpeed;
            Debug.Log($"[PlayerAttack] Bullet velocity set to {rb2d.linearVelocity}");
        }

        var b = bullet.GetComponent<Bullet>();  
        if (b)
        {
            b.damage = w.damage;
            b.attacker = transform;             
            Debug.Log($"[PlayerAttack] Bullet damage set to {b.damage}");
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