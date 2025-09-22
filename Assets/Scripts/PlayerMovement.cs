using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float horizontalInput;
    Rigidbody2D rb;
    [SerializeField] private float minJumpPower = 5f;
    [SerializeField] private float maxJumpPower = 15f;
    [SerializeField] private float jumpChargeRate = 20f;
    [SerializeField] private float currentJumpPower = 0f;
    bool isJumping = false;


    [Header("Animation")]
    [SerializeField] private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        animator.SetBool("isRunning", horizontalInput != 0);

        if (horizontalInput > 0)
        {
            sr.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            sr.flipX = true;
        }

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            currentJumpPower = minJumpPower;
        }

        if (Input.GetButton("Jump") && !isJumping)
        {
            currentJumpPower += jumpChargeRate * Time.deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, maxJumpPower);
        }

        if (Input.GetButtonUp("Jump") && !isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpPower);
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            currentJumpPower = 0f;
        }
    }
}