using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    //public float duration = 0.2f; // Duration of the shake
    //public float magnitude = 0.2f; // Magnitude of the shake

    private bool isShaking = false; // Tracks if the shake is currently active

    public IEnumerator Shake(float duration, float magnitude)
    {
        if (isShaking) yield break; // Prevent overlapping shakes

        isShaking = true;

        Vector3 originalPosition = transform.position; // Store the original global position
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Generate random shake offsets
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Apply the shake relative to the original position
            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reset to the original position after the shake
        transform.position = originalPosition;

        isShaking = false;
    }
}
