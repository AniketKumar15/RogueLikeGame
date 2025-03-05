using UnityEngine;

public class RigidbodyMouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float sensitivityX = 100f;
    public float sensitivityY = 100f;
    public Transform orientation;

    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Head Bobbing Settings")]
    public float bobFrequency = 2f;
    public float bobAmount = 0.05f;
    private float bobTimer;
    private Vector3 startPos;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startPos = transform.localPosition;
    }

    private void Update()
    {
        HandleMouseLook();
       // HandleHeadBobbing();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    //private void HandleHeadBobbing()
    //{
    //    if (rb.velocity.magnitude > 0.1f && rb.velocity.y == 0)
    //    {
    //        bobTimer += Time.deltaTime * bobFrequency;
    //        float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
    //        transform.localPosition = startPos + new Vector3(0, bobOffset, 0);
    //    }
    //    else
    //    {
    //        bobTimer = 0;
    //        transform.localPosition = startPos;
    //    }
    //}
}
