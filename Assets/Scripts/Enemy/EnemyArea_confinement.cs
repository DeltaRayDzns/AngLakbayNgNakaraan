using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyArea_confinement : MonoBehaviour
{
    [Tooltip("Only objects with this tag are confined. Leave empty to accept any with EnemyAI_controller.")]
    public string requiredTag = "Enemy";
    [Tooltip("Confine along X axis.")]
    public bool confineX = true;
    [Tooltip("Confine along Y axis.")]
    public bool confineY = true;
    [Tooltip("Small inset so objects are kept slightly inside the box to avoid flicker on the boundary.")]
    [Range(0f, 0.5f)] public float skin = 0.05f;

    BoxCollider2D box;

    void Reset()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        if (box) box.isTrigger = true;
    }

    void OnTriggerStay2D(Collider2D other)  => Confine(other);
    void OnTriggerExit2D(Collider2D other)  => Confine(other); // if it tries to exit, snap back in

    void Confine(Collider2D other)
    {
        // Only affect enemies (by tag and/or by component)
        if (!Accept(other)) return;

        var rb = other.attachedRigidbody;
        if (!rb) return;

        // world-space bounds, slightly shrunk by skin
        var b = box.bounds;
        b.Expand(-skin * 2f);

        var p = rb.position;
        var v = rb.linearVelocity;
        bool changed = false;

        if (confineX)
        {
            if (p.x < b.min.x) { p.x = b.min.x; if (v.x < 0) v.x = 0; changed = true; }
            else if (p.x > b.max.x) { p.x = b.max.x; if (v.x > 0) v.x = 0; changed = true; }
        }

        if (confineY)
        {
            if (p.y < b.min.y) { p.y = b.min.y; if (v.y < 0) v.y = 0; changed = true; }
            else if (p.y > b.max.y) { p.y = b.max.y; if (v.y > 0) v.y = 0; changed = true; }
        }

        if (changed)
        {
            rb.position = p;
            rb.linearVelocity = v;
        }
    }

    bool Accept(Collider2D other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return false;
        return other.GetComponent<EnemyAI_controller>() != null;
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var bc = GetComponent<BoxCollider2D>();
        if (!bc) return;
        var outer = bc.bounds;
        var inner = outer; inner.Expand(-skin * 2f);

        Gizmos.color = new Color(1f, 0.6f, 0f, 0.12f);
        Gizmos.DrawCube(outer.center, outer.size);
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.9f);
        Gizmos.DrawWireCube(inner.center, inner.size);
    }
#endif
}
