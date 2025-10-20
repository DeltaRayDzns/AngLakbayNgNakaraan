using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Pathfinding;

public class EnemyAI_astarPath : MonoBehaviour
{
    [Header("pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    // NEW: jump gating (prevents “jumping in air”)
    [Header("Jump Gating")]
    [Tooltip("How close (in X) to the ledge/node we must be before we jump")]
    public float jumpLeadX = 0.35f;
    [Tooltip("Min time between jumps")]
    public float jumpCooldown = 0.30f;
    [Tooltip("Allow jump this long after leaving ground")]
    public float coyoteTime = 0.08f;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    [SerializeField] private string groundTag = "Ground";
    private int groundContacts = 0;

    // NEW: timers
    private float lastGroundedAt = -999f;
    private float lastJumpAt = -999f;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled) PathFollow();
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void PathFollow()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        // Ground check: collisions OR raycast
        bool rayGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset
        );
        isGrounded = (groundContacts > 0) || rayGrounded;
        if (isGrounded) lastGroundedAt = Time.time;

        // Horizontal steering only (no vertical force each frame)
        Vector2 toWp = (Vector2)path.vectorPath[currentWaypoint] - rb.position;
        float dirX = Mathf.Sign(toWp.x);
        Vector2 hForce = new Vector2(dirX, 0f) * speed * Time.deltaTime;
        rb.AddForce(hForce);

        // ---- Jump gating (prevents mid-air retriggers) ----
        if (jumpEnabled)
        {
            bool needJumpUp = toWp.y > jumpNodeHeightRequirement;        // next node is above us
            bool nearLedge  = Mathf.Abs(toWp.x) <= jumpLeadX;            // only jump when close in X
            bool cooled     = (Time.time - lastJumpAt) >= jumpCooldown;
            bool groundedOrCoyote = isGrounded || (Time.time - lastGroundedAt) <= coyoteTime;

            if (needJumpUp && nearLedge && cooled && groundedOrCoyote)
            {
                // consistent takeoff
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * speed * jumpModifier, ForceMode2D.Impulse);
                lastJumpAt = Time.time;
            }
        }

        // Advance waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;

        // Face move direction
        if (directionLookEnabled)
        {
            if (rb.linearVelocity.x > 0.05f)
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (rb.linearVelocity.x < -0.05f)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // --- Ground tag contacts ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            groundContacts++;
            isGrounded = true;
            lastGroundedAt = Time.time;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            isGrounded = true;
            lastGroundedAt = Time.time;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            groundContacts = Mathf.Max(groundContacts - 1, 0);
            if (groundContacts == 0) isGrounded = false;
        }
    }
}
