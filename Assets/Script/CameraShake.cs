using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    //public float duration = 0.2f; // Duration of the shake
    //public float magnitude = 0.2f; // Magnitude of the shake

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalLocalPosition = transform.localPosition; // Use local position instead of global position
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalLocalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition; // Reset position
    }

    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
}
