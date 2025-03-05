using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float dashForce = 10f;
    public float jumpForce = 5f;
    public int maxJumps = 2;
    public float slideForce = 8f;
    public float slideDuration = 0.5f;

    [Header("References")]
    public Transform orientation;
    public Transform camHolder;
    private Rigidbody rb;
    private bool isGrounded;
    private int jumpsLeft;
    private bool isSliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jumpsLeft = maxJumps;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleDash();
        HandleSliding();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = orientation.forward * z + orientation.right * x;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        if (!isSliding)
            rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
    }

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpsLeft--;
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Vector3 dashDir = orientation.forward;
            rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
        }
    }

    private void HandleSliding()
    {
        if (Input.GetKeyDown(KeyCode.C) && isGrounded)
        {
            isSliding = true;
            rb.AddForce(orientation.forward * slideForce, ForceMode.Impulse);
            Invoke("StopSlide", slideDuration);
        }
    }

    private void StopSlide()
    {
        isSliding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpsLeft = maxJumps;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            rb.useGravity = false;
            rb.velocity = new Vector3(rb.velocity.x, 5f, rb.velocity.z);
        }
        else if (other.CompareTag("JumpPad"))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * 2, rb.velocity.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            rb.useGravity = true;
        }
    }
}
