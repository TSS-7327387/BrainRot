using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;
    private float magnitude = 0.1f;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }

    public void StartShake(float magnitude = 0.15f)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        this.magnitude = magnitude;
        originalPosition = transform.localPosition;
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
            transform.localPosition = originalPosition;
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);
            yield return null;
        }
    }
}
