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

    [Header("Animation")]
    [SerializeField] Animator anim;              
    [SerializeField] string pSpeed = "Speed";
    [SerializeField] string pGrounded = "IsGrounded";
    [SerializeField] string tJump = "Jump";

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
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
            PathFollow();

        if (anim)
        {
            anim.SetFloat(pSpeed, Mathf.Abs(rb.velocity.x));
            anim.SetBool(pGrounded, isGrounded);
        }
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

                if (anim) anim.SetTrigger(tJump);
            }
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance) currentWaypoint++;

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
