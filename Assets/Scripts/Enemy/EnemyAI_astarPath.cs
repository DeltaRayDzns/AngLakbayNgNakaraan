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

    [Header("Jump Gating")]
    public float jumpLeadX = 0.35f;
    public float jumpCooldown = 0.30f;
    public float coyoteTime = 0.08f;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;

    [Header("Animation (by state name)")]
    [SerializeField] private Animator anim;
    [SerializeField] private string stIdle   = "Idle";
    [SerializeField] private string stRun    = "Run";
    [SerializeField] private string stJump   = "Jump";
    [SerializeField] private string stAttack = "Attack";
    [SerializeField] private float fade = 0.05f;
    [SerializeField] private float moveThreshold = 0.05f;

    private int idleID, runID, jumpID, attackID;

    private Seeker seeker;
    private Rigidbody2D rb;

    [SerializeField] private string groundTag = "Ground";
    private int groundContacts = 0;

    private float lastGroundedAt = -999f;
    private float lastJumpAt = -999f;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateSeconds);

        if (!anim) anim = GetComponentInChildren<Animator>();

        idleID   = Animator.StringToHash(stIdle);
        runID    = Animator.StringToHash(stRun);
        jumpID   = Animator.StringToHash(stJump);
        attackID = Animator.StringToHash(stAttack);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
            PathFollow();

        UpdateAnimDirect();
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void PathFollow()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        bool rayGrounded = Physics2D.Raycast(
            transform.position, Vector2.down,
            GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset
        );
        isGrounded = (groundContacts > 0) || rayGrounded;
        if (isGrounded) lastGroundedAt = Time.time;

        Vector2 toWp = (Vector2)path.vectorPath[currentWaypoint] - rb.position;
        float dirX = Mathf.Sign(toWp.x);
        Vector2 hForce = new Vector2(dirX, 0f) * speed * Time.deltaTime;
        rb.AddForce(hForce);

        if (jumpEnabled)
        {
            bool needJumpUp = toWp.y > jumpNodeHeightRequirement;
            bool nearLedge  = Mathf.Abs(toWp.x) <= jumpLeadX;
            bool cooled     = (Time.time - lastJumpAt) >= jumpCooldown;
            bool groundedOrCoyote = isGrounded || (Time.time - lastGroundedAt) <= coyoteTime;

            if (needJumpUp && nearLedge && cooled && groundedOrCoyote)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * speed * jumpModifier, ForceMode2D.Impulse);
                lastJumpAt = Time.time;
            }
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;

        if (directionLookEnabled)
        {
            if (rb.linearVelocity.x > moveThreshold)
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (rb.linearVelocity.x < -moveThreshold)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void UpdateAnimDirect()
    {
        if (!anim || rb == null) return;

        var st = anim.GetCurrentAnimatorStateInfo(0);

        if (st.shortNameHash == attackID && st.normalizedTime < 0.95f) return;

        if (!isGrounded)
        {
            if (st.shortNameHash != jumpID)
                anim.CrossFade(jumpID, fade);
            return;
        }

        float absX = Mathf.Abs(rb.linearVelocity.x);

        if (absX > moveThreshold)
        {
            if (st.shortNameHash != runID)
                anim.CrossFade(runID, fade);
        }
        else
        {
            if (st.shortNameHash != idleID)
                anim.CrossFade(idleID, fade);
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
