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

	public float MinJumpPower => minJumpPower;
	public float MaxJumpPower => maxJumpPower;
	public float CurrentJumpPower => currentJumpPower;

    [SerializeField] private float currentJumpPower = 0f;
    bool isGrounded = false;

    [Header("Movement On Platform")] 
    public bool isOnPlatform;
    public Rigidbody2D platformRb;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float keyboardInput = Input.GetAxis("Horizontal");

		if (Mathf.Abs(horizontalInput) < 0.1f) 
		{
			horizontalInput = keyboardInput; 
		}

        animator.SetBool("IsJump", !isGrounded);

        SpriteFlip();

        HandleChargeJump();
    }

    private void SpriteFlip()
    {
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1.3f, transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1.3f, transform.localScale.y, transform.localScale.z);
        }
    }

    private void  HandleChargeJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            currentJumpPower = minJumpPower;
        }

        if (Input.GetButton("Jump") && isGrounded)
        {
            currentJumpPower += jumpChargeRate * Time.deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, maxJumpPower);
        }

        if (Input.GetButtonUp("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpPower);
            currentJumpPower = 0f;
            isGrounded = false;
        }
    
    }


    private void FixedUpdate()
    {
        float targetSpeed = horizontalInput * moveSpeed;
        
        if (isOnPlatform)
        {
            rb.linearVelocity = new Vector2(targetSpeed + platformRb.velocity.x, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat("Runspeed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

   private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
           
            isGrounded = true;
            animator.SetBool("IsJump", !isGrounded);

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("IsJump", !isGrounded); 

        }
    }

	public void SetHorizontalInput(float value) 
	{
		horizontalInput = value; 
		SpriteFlip();

	}

	public void StartJumpCharge() 
	{
		if (isGrounded) currentJumpPower = minJumpPower; 
	}

	public void HoldJumpButton() 
	{
		if (isGrounded) 
		{
			currentJumpPower += jumpChargeRate * Time.deltaTime;
			currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, maxJumpPower);
		}
	} 
	
 	public void ReleaseJump() 
	{
		if (isGrounded) 
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpPower);	
			currentJumpPower = 0f;
			isGrounded = false; 
		}
	} 
}