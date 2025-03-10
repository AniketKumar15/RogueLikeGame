using UnityEngine;
using UnityEngine.UI;

public class DynamicCrosshair : MonoBehaviour
{
    public RectTransform top, bottom, left, right; // UI elements
    public float defaultGap = 10f;
    public float moveSpread = 10f; // How much the crosshair expands
    public float runSpread = 30f; // How much the crosshair expands
    public float aimSpread = 5f;   // Crosshair size when aiming
    public float shrinkSpeed = 10f; // Speed of transition

    private float currentSpread = 0f;
    private FirstPersonMovement playerMovement; // Reference to movement script
    private GunSystem gunSystem; // Reference to aiming

    private void Start()
    {
        playerMovement = FindObjectOfType<FirstPersonMovement>();
        gunSystem = FindObjectOfType<GunSystem>();
        currentSpread = defaultGap; // Set initial gap
    }

    private void Update()
    {
        float targetSpread = defaultGap; // Start with the default gap;

        // Expand when moving or shooting
        if (playerMovement != null && playerMovement.IsMoving() && !playerMovement.IsRunningPlayer())
        {
            targetSpread = moveSpread;
            gunSystem.spread = 0.05f;
        }
        else if (playerMovement != null && playerMovement.IsMoving() && playerMovement.IsRunningPlayer())
        {
            targetSpread = runSpread;
            gunSystem.spread = 0.1f;
        }
        else
        {
            gunSystem.spread = 0.02f;
        }

        if (gunSystem != null && gunSystem.IsShooting())
        {
            targetSpread = runSpread;
        }

        // Shrink when aiming
        if (gunSystem != null && gunSystem.IsAiming())
            targetSpread = aimSpread;

        // Smooth transition
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * shrinkSpeed);

        // Apply spread to crosshair UI elements
        top.anchoredPosition = new Vector2(0, currentSpread);
        bottom.anchoredPosition = new Vector2(0, -currentSpread);
        left.anchoredPosition = new Vector2(-currentSpread, 0);
        right.anchoredPosition = new Vector2(currentSpread, 0);
    }
}
