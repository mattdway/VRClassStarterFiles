using UnityEngine;
using System.Collections;

public class ShakeEffect : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.1f;
    public bool shakeOnX = true;
    public bool shakeOnY = true;
    public bool shakeOnZ = false; // Set to true if you want shaking on the Z-axis as well

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = shakeOnX ? Random.Range(-1f, 1f) * shakeIntensity : 0f;
            float offsetY = shakeOnY ? Random.Range(-1f, 1f) * shakeIntensity : 0f;
            float offsetZ = shakeOnZ ? Random.Range(-1f, 1f) * shakeIntensity : 0f;

            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
