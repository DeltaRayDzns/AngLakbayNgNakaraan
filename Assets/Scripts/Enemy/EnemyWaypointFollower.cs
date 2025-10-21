using UnityEngine;

public class EnemyWaypointFollower : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints;
    public float speed = 3f;
    public float jumpForce = 7f;
    public float detectRange = 6f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Transform targetWaypoint;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindClosestWaypoint();
    }

    void FixedUpdate()
    {
        if (player == null || waypoints.Length == 0)
            return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // If the player moves, update target waypoint
        if (Vector2.Distance(transform.position, player.position) < detectRange)
        {
            FindClosestWaypointToPlayer();
        }

        if (targetWaypoint == null)
            return;

        Vector2 direction = (targetWaypoint.position - transform.position).normalized;

        // Move horizontally
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        // Jump if waypoint is above
        if (isGrounded && targetWaypoint.position.y - transform.position.y > 0.8f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Check if reached waypoint
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            FindClosestWaypointToPlayer();
        }
    }

    void FindClosestWaypoint()
    {
        float minDist = Mathf.Infinity;
        foreach (Transform wp in waypoints)
        {
            float dist = Vector2.Distance(transform.position, wp.position);
            if (dist < minDist)
            {
                minDist = dist;
                targetWaypoint = wp;
            }
        }
    }

    void FindClosestWaypointToPlayer()
    {
        float minDist = Mathf.Infinity;
        foreach (Transform wp in waypoints)
        {
            float playerDist = Vector2.Distance(player.position, wp.position);
            if (playerDist < minDist)
            {
                minDist = playerDist;
                targetWaypoint = wp;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (Transform wp in waypoints)
        {
            Gizmos.DrawWireSphere(wp.position, 0.2f);
        }

        if (targetWaypoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetWaypoint.position);
        }
    }
}
