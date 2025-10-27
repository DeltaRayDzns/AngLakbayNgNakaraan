using UnityEngine;
using System.Collections;
using System.Collections.Generic;   // NEW

public class MovingPlatform : MonoBehaviour
{
    public float Speed; 
    Vector3 targetPos;
    
    private PlayerMovement playerMovement;
    private Rigidbody2D rb; 
    Vector3 moveDirection;
    private Rigidbody2D playerRb;

    [Header("Ways")] 
    public GameObject ways;
    public Transform[] wayPoints; 
    int pointIndex;
    int pointCount;
    int direction = 1;

    [Header("Coroutine: wait before go")] 
    public float waitDuration;

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color waypointColor = new Color(1f, 0.7f, 0f, 0.9f); 
    public Color lineColor     = new Color(0f, 1f, 1f, 0.9f);    
    public float waypointRadius = 0.15f;
    public bool showTarget = true;
    public Color targetColor = Color.green;

    bool isWaiting = false;

    // -------- NEW: carry riders by delta, no parenting ----------
    private readonly HashSet<Rigidbody2D> riders = new HashSet<Rigidbody2D>();
    private Vector2 lastPos;        // platform pos last physics step
    private Vector2 frameDelta;     // how far platform moved this step
    // ------------------------------------------------------------

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        wayPoints = new Transform[ways.transform.childCount];
        for (int i = 0; i < ways.gameObject.transform.childCount; i++)
        {
            wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
        }
    }

    private void Start()
    {
        pointIndex = 1;
        pointCount = wayPoints.Length;

        if (pointCount < 2)
        {
            Debug.LogWarning("[MovingPlatform] Need at least 2 waypoints under 'ways'.");
            enabled = false;
            return;
        }

        targetPos = wayPoints[1].transform.position;
        DirectionCalculate();

        // NEW: init lastPos so first delta is clean
        lastPos = rb.position;
    }

    private void Update()
    {
        if (!isWaiting && Vector2.SqrMagnitude((Vector2)transform.position - (Vector2)targetPos) < (0.05f * 0.05f))
        {
            NextPoint(); 
        }
    }

    private void FixedUpdate()
    {
        Vector2 curr = rb.position;
        Vector2 step = (Vector2)(moveDirection * Speed * Time.fixedDeltaTime);
        Vector2 next = curr + step;

        if (!isWaiting && step.sqrMagnitude > 0f)
        {
            Vector2 toTargetCurr = (Vector2)targetPos - curr;
            Vector2 toTargetNext = (Vector2)targetPos - next;

            if (Vector2.Dot(toTargetCurr, toTargetNext) <= 0f)
            {
                rb.MovePosition((Vector2)targetPos);
                frameDelta = (Vector2)targetPos - lastPos;   // NEW: delta this step
                CarryRiders(frameDelta);                      // NEW
                lastPos = rb.position;                        // NEW
                NextPoint();
                return;
            }
        }

        rb.MovePosition(next);

        // NEW: compute delta and carry riders every physics step
        frameDelta = next - lastPos;
        CarryRiders(frameDelta);
        lastPos = next;
    }

    // NEW: Move any rider by the exact platform delta (keeps contact, no parenting)
    private void CarryRiders(Vector2 delta)
    {
        if (delta == Vector2.zero || riders.Count == 0) return;

        foreach (var r in riders)
        {
            if (!r) continue;
            // Only carry if still roughly on top (optional safety)
            r.MovePosition(r.position + delta);
        }
    }

    void NextPoint()
    {
        if (isWaiting) return;
        isWaiting = true;

        transform.position = targetPos;
        moveDirection = Vector3.zero;

        if (pointIndex ==  pointCount - 1) direction = -1;
        if (pointIndex == 0)               direction =  1;
        
        pointIndex += direction;
        targetPos = wayPoints[pointIndex].transform.position;

        StartCoroutine(WaitNextPoint());
    }

    IEnumerator WaitNextPoint()
    {
        yield return new WaitForSeconds(waitDuration);
        DirectionCalculate();
        isWaiting = false;
    }

    void DirectionCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    // ----- CHANGED: no hard-parenting, no gravity hacks; just mark as rider -----
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        riders.Add(collision.attachedRigidbody);

        playerMovement.isOnPlatform = true;
        playerMovement.platformRb = rb;
        // removed parenting and gravityScale edits
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        riders.Remove(collision.attachedRigidbody);

        playerMovement.isOnPlatform = false;
        // removed parenting and gravityScale edits
    }
    // ----------------------------------------------------------------------------

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Transform[] pts = wayPoints;
        if ((pts == null || pts.Length == 0) && ways != null)
        {
            int childCount = ways.transform.childCount;
            if (childCount > 0)
            {
                pts = new Transform[childCount];
                for (int i = 0; i < childCount; i++)
                    pts[i] = ways.transform.GetChild(i);
            }
        }

        if (pts == null || pts.Length == 0) return;

        Gizmos.color = waypointColor;
        for (int i = 0; i < pts.Length; i++)
        {
            if (!pts[i]) continue;
            Gizmos.DrawSphere(pts[i].position, waypointRadius);
        }

        Gizmos.color = lineColor;
        for (int i = 0; i < pts.Length - 1; i++)
        {
            if (!pts[i] || !pts[i + 1]) continue;
            Gizmos.DrawLine(pts[i].position, pts[i + 1].position);
        }

        if (showTarget && Application.isPlaying)
        {
            Gizmos.color = targetColor;
            Gizmos.DrawWireSphere(targetPos, waypointRadius * 1.6f);
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}
