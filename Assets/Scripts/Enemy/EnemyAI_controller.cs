using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class EnemyAI_controller : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float nextWaypointDistance = 0.6f;
    public float jumpForce = 6f;

    [Header("Checks")]
    public Transform groundCheck;
    public Transform wallCheck;             // gizmo only
    public Transform ledgeCheck;            // gizmo only
    public LayerMask groundLayer;           // set to your Obstacles layer
    public float wallCheckDistance = 0.5f;  // gizmo only
    public float ledgeCheckDistance = 0.8f; // gizmo only

    [Header("Segment logic")]
    public float segmentUpThreshold = 0.5f;     // dy > this => jump
    public float segmentDownThreshold = 0.5f;   // dy < -this => drop
    public float jumpLeadX = 0.30f;             // act near segment start

    [Header("Stability & Air control")]
    public float repathInterval = 0.35f;
    public float jumpCooldown = 0.40f;
    public float coyoteTime = 0.08f;
    public float maxAirSpeed = 6.0f;            // cap planned vx
    public float airAccel = 60f;                // how fast we steer to planned vx
    public float landingXSlack = 0.25f;         // capture window at landing
    public float landingYSlack = 0.35f;
    public float dropThroughDuration = 0.22f;   // if standing on a PlatformEffector2D

    private Seeker seeker;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool facingRight = true;

    // --- Use TRUE graph nodes (not smoothed) ---
    private List<GraphNode> nodePath = new List<GraphNode>();
    private readonly List<Vector2> nodePoints = new List<Vector2>();
    private int nodeIndex = 0;

    private float nextRepathTime;
    private float lastGroundedAt = -999f;
    private float lastJumpAt = -999f;

    // Airborne lock + plan
    struct AirPlan
    {
        public bool active;
        public bool isJump;          // true: jump, false: drop
        public int landingIndex;
        public Vector2 takeoff;
        public Vector2 landing;
        public float targetVx;       // air-control horizontal speed
        public float startTime;
        public float endTime;        // expected end (time of flight + buffer)
    }
    private AirPlan air;
    private readonly Collider2D[] contactBuf = new Collider2D[8];

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        nextRepathTime = Time.time;
    }

    void Update()
    {
        // Never repath while we’re executing an air plan
        if (air.active) return;

        if (target != null && Time.time >= nextRepathTime && seeker.IsDone())
        {
            nextRepathTime = Time.time + repathInterval;
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (p == null || p.error) return;
        nodePath = p.path;              // TRUE graph nodes
        nodePoints.Clear();
        for (int i = 0; i < nodePath.Count; i++)
            nodePoints.Add((Vector3)nodePath[i].position);

        nodeIndex = FindClosestNodeIndex(nodePoints, rb.position, nodeIndex);
    }

    void FixedUpdate()
    {
        if (target == null || nodePoints.Count == 0 || nodeIndex >= nodePoints.Count) return;

        // Grounded / coyote
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (grounded) lastGroundedAt = Time.time;

        // If we’re in an air plan, steer horizontally toward planned vx and wait to land.
        if (air.active)
        {
            float vx = Mathf.MoveTowards(rb.linearVelocity.x, air.targetVx, airAccel * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

            bool landedNow =
                grounded &&
                Mathf.Abs(rb.position.x - air.landing.x) <= landingXSlack &&
                Mathf.Abs(rb.position.y - air.landing.y) <= landingYSlack &&
                (Time.time - air.startTime) > 0.06f; // avoid instant re-lock

            bool timedOut = Time.time >= air.endTime; // safety

            if (landedNow || timedOut)
            {
                air.active = false;
                nodeIndex = Mathf.Clamp(air.landingIndex, 0, nodePoints.Count - 1);
            }
            return; // do not advance nodes while locked in air
        }

        // Normal steering toward current node (on ground or free)
        Vector2 curr = nodePoints[Mathf.Clamp(nodeIndex, 0, nodePoints.Count - 1)];
        bool hasNext = nodeIndex < nodePoints.Count - 1;
        Vector2 next = hasNext ? nodePoints[nodeIndex + 1] : curr;

        // Face for visuals
        float dirX = Mathf.Sign(curr.x - rb.position.x);
        if (dirX > 0 && !facingRight) Flip();
        else if (dirX < 0 && facingRight) Flip();

        // Decide vertical action from segment only
        if (hasNext)
        {
            float dy = next.y - curr.y;                 // +Y up
            bool segUp = dy >  segmentUpThreshold;
            bool segDown = dy < -segmentDownThreshold;

            // Proximity to segment start (with small forgiveness after passing)
            float moveDir = Mathf.Sign(next.x - curr.x);
            if (moveDir == 0) moveDir = Mathf.Sign(curr.x - rb.position.x);
            float along = (rb.position.x - curr.x) * moveDir;
            bool nearStart = Mathf.Abs(rb.position.x - curr.x) <= jumpLeadX;
            bool justPassed = along >= 0f && along <= jumpLeadX * 1.5f;
            bool inWindow = nearStart || justPassed;

            // JUMP: only if grounded or within coyote window, and we can plan a landing
            if (segUp && inWindow && (grounded || (Time.time - lastGroundedAt) <= coyoteTime) && (Time.time - lastJumpAt) >= jumpCooldown)
            {
                if (PlanJump(curr, next, out air))
                {
                    lastJumpAt = Time.time;
                    // set takeoff instantly with planned vx
                    rb.linearVelocity = new Vector2(air.targetVx, jumpForce);
                    // steer in air toward landing; freeze index until we land
                    nodeIndex = air.landingIndex; // steer to landing node horizontally
                    return;
                }
                // If planning failed (jump too high), do nothing; path is likely invalid for this jumpForce.
            }

            // DROP: step down; if touching PlatformEffector2D, briefly ignore to fall through
            if (segDown && inWindow && grounded)
            {
                PlanDrop(curr, next, out air); // always succeeds
                TryDropThroughEffectors(dropThroughDuration);
                nodeIndex = air.landingIndex;  // steer to landing node horizontally
                return;
            }
        }

        // Grounded horizontal steering
        Vector2 to = curr - rb.position;
        float desiredX = speed * Mathf.Sign(to.x);
        rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, desiredX, 40f * Time.fixedDeltaTime), rb.linearVelocity.y);

        // Advance node (lenient) — but only when not airborne
        float dx = Mathf.Abs(rb.position.x - curr.x);
        float dist = to.magnitude;
        if (dx < Mathf.Max(0.25f, nextWaypointDistance * 0.5f) || dist < nextWaypointDistance)
            nodeIndex++;
    }

    // ---------- Planning ----------

    bool PlanJump(Vector2 takeoff, Vector2 landing, out AirPlan plan)
    {
        plan = default;

        // Physics: y(t) = v0y * t - 0.5 * g * t^2  ; want y(t) = dy
        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale); // positive
        float v0y = jumpForce;
        float dy = landing.y - takeoff.y;

        // Solve 0.5*g*t^2 - v0y*t + dy = 0  => t = (v0y ± sqrt(v0y^2 - 2*g*dy)) / g
        float disc = v0y * v0y - 2f * g * dy;
        if (disc < 0f)
        {
            // Jump force cannot reach the landing height
            // (maxRise = v0y^2 / (2g)). Either raise jumpForce or lower link.
            return false;
        }

        float tAscDesc = (v0y + Mathf.Sqrt(disc)) / g; // descending root (landing after apex)
        tAscDesc = Mathf.Max(tAscDesc, 0.05f);

        float dx = landing.x - takeoff.x;
        float vx = dx / tAscDesc;
        vx = Mathf.Clamp(vx, -maxAirSpeed, maxAirSpeed);

        plan = new AirPlan
        {
            active = true,
            isJump = true,
            landingIndex = nodeIndex + 1,
            takeoff = takeoff,
            landing = landing,
            targetVx = vx,
            startTime = Time.time,
            endTime = Time.time + tAscDesc + 0.08f // small buffer
        };
        return true;
    }

    bool PlanDrop(Vector2 takeoff, Vector2 landing, out AirPlan plan)
    {
        plan = default;
        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float dyDown = Mathf.Max(0.01f, takeoff.y - landing.y); // positive drop distance
        float t = Mathf.Sqrt(2f * dyDown / g);                  // fall time from rest (good approximation)
        float dx = landing.x - takeoff.x;
        float vx = Mathf.Clamp(dx / t, -maxAirSpeed, maxAirSpeed);

        plan = new AirPlan
        {
            active = true,
            isJump = false,
            landingIndex = nodeIndex + 1,
            takeoff = takeoff,
            landing = landing,
            targetVx = vx,
            startTime = Time.time,
            endTime = Time.time + t + 0.06f
        };
        return true;
    }

    // ---------- Utilities ----------

    void TryDropThroughEffectors(float duration)
    {
        int n = col.GetContacts(contactBuf);
        List<Collider2D> effs = null;
        for (int i = 0; i < n; i++)
        {
            var c = contactBuf[i];
            if (c != null && c.GetComponent<PlatformEffector2D>() != null)
            {
                (effs ??= new List<Collider2D>(4)).Add(c);
            }
        }
        if (effs != null) StartCoroutine(DropThroughEffectors(effs, duration));
    }

    System.Collections.IEnumerator DropThroughEffectors(List<Collider2D> effs, float duration)
    {
        foreach (var c in effs) if (c) Physics2D.IgnoreCollision(col, c, true);
        yield return new WaitForSeconds(duration);
        foreach (var c in effs) if (c) Physics2D.IgnoreCollision(col, c, false);
    }

    int FindClosestNodeIndex(List<Vector2> pts, Vector2 pos, int hint)
    {
        if (pts == null || pts.Count == 0) return 0;
        int start = Mathf.Clamp(hint, 0, pts.Count - 1);
        int best = start;
        float bestSqr = (pts[start] - pos).sqrMagnitude;
        for (int i = Mathf.Max(0, start - 3); i <= Mathf.Min(pts.Count - 1, start + 3); i++)
        {
            float d = (pts[i] - pos).sqrMagnitude;
            if (d < bestSqr) { bestSqr = d; best = i; }
        }
        for (int i = 0; i < pts.Count; i++)
        {
            float d = (pts[i] - pos).sqrMagnitude;
            if (d < bestSqr) { bestSqr = d; best = i; }
        }
        return best;
    }

    void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale; s.x *= -1f; transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + Vector3.down * ledgeCheckDistance);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = Vector3.right * (facingRight ? 1f : -1f);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }
    }
}
