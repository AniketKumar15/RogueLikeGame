using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Head Bob Settings")]
    public float walkBobSpeed = 8f;  // Speed of bobbing while walking
    public float walkBobAmount = 0.05f; // How much the head moves while walking
    public float sprintBobSpeed = 12f; // Speed of bobbing while sprinting
    public float sprintBobAmount = 0.1f; // How much the head moves while sprinting
    public float crouchBobSpeed = 6f;  // Speed of bobbing while crouching
    public float crouchBobAmount = 0.03f; // How much the head moves while crouching

    [Header("Side Bobbing (Left-Right)")]
    public float sideBobAmount = 0.02f;  // How much the head moves side-to-side
    public float sideBobSpeed = 8f; // Speed of side-to-side movement

    [Header("References")]
    public Transform playerTransform;  // Reference to the player
    private Rigidbody rb;

    private float defaultYPos;
    private float timer = 0;

    void Start()
    {
        defaultYPos = transform.localPosition.y;
        rb = playerTransform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get Player Speed
        float speed = rb.velocity.magnitude;
        float bobSpeed, bobAmount;

        // Adjust bobbing based on speed (walking, sprinting, crouching)
        if (speed > 2.5f) // Sprinting
        {
            bobSpeed = sprintBobSpeed;
            bobAmount = sprintBobAmount;
        }
        else if (speed > 1f) // Walking
        {
            bobSpeed = walkBobSpeed;
            bobAmount = walkBobAmount;
        }
        else if (speed > 0.1f) // Crouching
        {
            bobSpeed = crouchBobSpeed;
            bobAmount = crouchBobAmount;
        }
        else
        {
            // Reset position when idle
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, defaultYPos, 0), Time.deltaTime * 5f);
            return;
        }

        // Calculate vertical bobbing (up & down)
        timer += Time.deltaTime * bobSpeed;
        float verticalBob = Mathf.Sin(timer) * bobAmount;

        // Calculate horizontal bobbing (side to side)
        float horizontalBob = Mathf.Sin(timer * sideBobSpeed) * sideBobAmount;

        // Apply head bobbing movement
        transform.localPosition = new Vector3(horizontalBob, defaultYPos + verticalBob, transform.localPosition.z);
    }
}
