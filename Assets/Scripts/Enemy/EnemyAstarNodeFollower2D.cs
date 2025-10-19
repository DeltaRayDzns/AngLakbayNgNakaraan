using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D), typeof(Seeker), typeof(Collider2D))]
public class EnemyAstarNodeFollower2D : MonoBehaviour
{
    [Header("Target / A*")]
    public Transform player;           // who to chase
    public float repathInterval = 0.30f;

    [Header("Movement")]
    public float moveSpeed = 4f;       // horizontal speed
    public float accel = 40f;          // horizontal acceleration
    public float jumpSpeed = 12f;      // vertical impulse
    public float gravityScale = 3f;

    [Header("Node Logic")]
    public float nodeReachRadius = 0.18f;  // how close to "consume" a node
    public float upStepThreshold = 0.50f;  // segment ΔY > this => jump
    public float downStepThreshold = 0.50f;// segment ΔY < -this => drop
    public float jumpLeadX = 0.25f;        // must be this close (in X) to the segment start to act
    public float jumpCooldown = 0.15f;     // avoid double-jumps

    [Header("Collision Layers")]
    public LayerMask groundMask;       // what counts as grounded
    public LayerMask oneWayMask;       // one-way platforms (for drop-through)
    public string droppingLayerName = "Dropping"; // layer that ignores one-way
    public float dropDuration = 0.22f; // time to ignore one-way for a drop

    Seeker seeker;
    Rigidbody2D rb;
    Collider2D col;

    List<GraphNode> nodePath = new List<GraphNode>(); // true graph nodes
    List<Vector3> nodePoints = new List<Vector3>();   // world positions of nodes
    int index = 0;

    float nextRepathTime = 0f;
    float lastJumpTime = -999f;
    bool dropping = false;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        // Repath at a fixed cadence so we don't "twitch"
        if (player != null && Time.time >= nextRepathTime)
        {
            nextRepathTime = Time.time + repathInterval;
            seeker.StartPath(rb.position, player.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (p == null || p.error) return;

        // Stick to the actual graph nodes (not smoothed vectors)
        nodePath = p.path;
        nodePoints.Clear();
        for (int i = 0; i < nodePath.Count; i++)
            nodePoints.Add((Vector3)nodePath[i].position);

        index = 0;

        // Optional: remove tiny steps
        const float minStep = 0.02f;
        for (int i = nodePoints.Count - 2; i >= 0; i--)
        {
            if ((nodePoints[i + 1] - nodePoints[i]).sqrMagnitude < minStep * minStep)
            {
                nodePoints.RemoveAt(i + 1);
                nodePath.RemoveAt(i + 1);
            }
        }
    }

    void FixedUpdate()
    {
        if (nodePoints == null || index >= nodePoints.Count)
        {
            // No path: gently brake horizontal motion
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0f, accel * Time.fixedDeltaTime), rb.linearVelocity.y);
            return;
        }

        Vector2 pos = rb.position;

        // --- SEGMENT-DRIVEN VERTICAL DECISIONS (purely from nodes) ---
        // Look at the segment from node[index] -> node[index+1].
        if (index < nodePoints.Count - 1)
        {
            Vector2 segStart = (Vector2)nodePoints[index];
            Vector2 segEnd   = (Vector2)nodePoints[index + 1];

            float dxToStart = Mathf.Abs(pos.x - segStart.x);
            float dySeg     = segEnd.y - segStart.y; // Unity: +Y is up

            bool needUp   = dySeg >  upStepThreshold;
            bool needDown = dySeg < -downStepThreshold;
            bool atStart  = dxToStart <= jumpLeadX;

            if (atStart && IsGrounded())
            {
                if (needUp && Time.time - lastJumpTime > jumpCooldown)
                {
                    // Jump to reach the higher node
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
                    lastJumpTime = Time.time;
                    index++; // immediately begin steering toward the landing node
                }
                else if (needDown && !dropping)
                {
                    // Drop to reach the lower node
                    if (IsOnOneWay()) StartCoroutine(DropThroughOneWay());
                    index++; // steer toward the lower node
                }
            }
        }

        // --- HORIZONTAL STEERING toward the current target node ---
        if (index < nodePoints.Count)
        {
            Vector2 target = (Vector2)nodePoints[index];
            float xDir = Mathf.Sign(target.x - pos.x);
            float desiredX = moveSpeed * xDir;

            rb.linearVelocity = new Vector2(
                Mathf.MoveTowards(rb.linearVelocity.x, desiredX, accel * Time.fixedDeltaTime),
                rb.linearVelocity.y
            );

            // Advance when we are close enough to the node
            float sqrDist = (target - pos).sqrMagnitude;
            if (sqrDist <= nodeReachRadius * nodeReachRadius)
                index++;
        }
    }

    bool IsGrounded() => col.IsTouchingLayers(groundMask);

    bool IsOnOneWay()
    {
        var filter = new ContactFilter2D { useLayerMask = true, layerMask = oneWayMask };
        var hits = new Collider2D[2];
        return col.Overlap(filter, hits) > 0;
    }

    System.Collections.IEnumerator DropThroughOneWay()
    {
        dropping = true;
        int dropLayer = LayerMask.NameToLayer(droppingLayerName);
        int originalLayer = gameObject.layer;
        if (dropLayer != -1) gameObject.layer = dropLayer; // make sure Dropping layer ignores one-way in physics matrix
        yield return new WaitForSeconds(dropDuration);
        gameObject.layer = originalLayer;
        dropping = false;
    }
}
