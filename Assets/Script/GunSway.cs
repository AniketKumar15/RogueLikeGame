using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{
     [Header("Sway Settings")]
    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothness = 6f;

    [Header("Rotation Sway")]
    public float rotationSwayAmount = 4f;
    public float maxRotationSway = 8f;
    public float rotationSmoothness = 6f;

    [Header("Jump Sway")]
    public float jumpSwayAmount = 0.1f;
    public float fallSwayAmount = 0.15f;
    public float jumpSmoothness = 4f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Find the Rigidbody of the Player
        rb = FindObjectOfType<Rigidbody>();
    }

    void Update()
    {
        // Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate Position Sway
        Vector3 swayPosition = new Vector3(-mouseX, -mouseY, 0) * swayAmount;
        swayPosition.x = Mathf.Clamp(swayPosition.x, -maxSwayAmount, maxSwayAmount);
        swayPosition.y = Mathf.Clamp(swayPosition.y, -maxSwayAmount, maxSwayAmount);

        // Check if the player is in the air (jumping/falling)
        if (rb.velocity.y > 0.1f) // Jumping Up
        {
            swayPosition.y += jumpSwayAmount;
        }
        else if (rb.velocity.y < -0.1f) // Falling Down
        {
            swayPosition.y -= fallSwayAmount;
        }

        // Smooth Position Movement
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + swayPosition, Time.deltaTime * swaySmoothness);

        // Calculate Rotation Sway
        Quaternion swayRotationX = Quaternion.AngleAxis(-mouseY * rotationSwayAmount, Vector3.right);
        Quaternion swayRotationY = Quaternion.AngleAxis(mouseX * rotationSwayAmount, Vector3.up);
        Quaternion targetRotation = initialRotation * swayRotationX * swayRotationY;

        // Smooth Rotation Movement
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSmoothness);
    }
}
