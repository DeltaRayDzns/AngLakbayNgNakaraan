using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float horizontalInput;
    Rigidbody2D rb;
    public float minJumpPower = 5f;
    public float maxJumpPower = 15f;
    public float jumpChargeRate = 20f;
    private float currentJumpPower = 0f;
    bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

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