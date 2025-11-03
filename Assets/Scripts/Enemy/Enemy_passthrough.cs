using UnityEngine;


[AddComponentMenu("AI/Enemy Passthrough 2D")]
public class Enemy_passthrough : MonoBehaviour
{
    [Header("Who counts as an Enemy?")]
    [Tooltip("If set, only objects with this tag are ignored (e.g., \"Enemy\"). Leave empty to accept any that has EnemyAI_controller.")]
    public string requiredTag = "Enemy";

    [Tooltip("Only ignore objects on these layers (set to Everything to ignore regardless of layer).")]
    public LayerMask enemyLayers = ~0;

    [Tooltip("Also treat objects that have EnemyAI_controller (even if tag is empty).")]
    public bool acceptByComponent = true;

    [Header("Behaviour")]
    [Tooltip("Re-enable collision when the enemy leaves the area. If false, the pair stays ignored (typical for hazards).")]
    public bool reenableOnExit = false;

    [Tooltip("Scan for overlapping enemies on Start so pairs are ignored immediately.")]
    public bool warmStartScan = true;

    [Tooltip("Include all colliders on this object and its children.")]
    public bool includeChildrenColliders = true;

    private Collider2D[] myColliders;
    private readonly Collider2D[] overlapBuf = new Collider2D[32];

    void Awake()
    {
        myColliders = includeChildrenColliders
            ? GetComponentsInChildren<Collider2D>(true)
            : new[] { GetComponent<Collider2D>() };

        if (myColliders == null || myColliders.Length == 0 || myColliders[0] == null)
            Debug.LogWarning($"{name}: Enemy_passthrough needs a Collider2D on this object (or children).");
    }

    void Start()
    {
        if (!warmStartScan || myColliders == null || myColliders.Length == 0 || myColliders[0] == null) return;

        var filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayers);
        filter.useTriggers = true;

        foreach (var mine in myColliders)
        {
            if (!mine) continue;
            int n = mine.Overlap(filter, overlapBuf);
            for (int i = 0; i < n; i++)
            {
                var other = overlapBuf[i];
                if (other && IsEnemy(other))
                    SetIgnorePair(mine, other, true);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D c) => TryIgnore(c.collider, true);
    void OnCollisionExit2D (Collision2D c) => TryIgnore(c.collider, false);

    void OnTriggerEnter2D(Collider2D other) => TryIgnore(other, true);
    void OnTriggerExit2D (Collider2D other) => TryIgnore(other, false);

    void TryIgnore(Collider2D other, bool entering)
    {
        if (!other) return;
        if (!IsEnemy(other)) return;

        bool ignore = entering || !reenableOnExit; 
        foreach (var mine in myColliders)
        {
            if (!mine || !other || mine == other) continue;
            SetIgnorePair(mine, other, ignore);
        }
    }

    bool IsEnemy(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayers) == 0) return false;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return false;

        if (!string.IsNullOrEmpty(requiredTag)) return true;
        if (acceptByComponent && other.GetComponentInParent<EnemyAI_controller>() != null) return true;

        return string.IsNullOrEmpty(requiredTag) && !acceptByComponent;
    }

    static void SetIgnorePair(Collider2D a, Collider2D b, bool ignore)
    {
        var enemyColliders = b.GetComponentsInChildren<Collider2D>(true);
        foreach (var ec in enemyColliders)
        {
            if (ec && a) Physics2D.IgnoreCollision(a, ec, ignore);
        }
    }
}
