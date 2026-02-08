using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Coroutine shakeRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float timer = 0f;

        while (timer < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;

            // SOLO offset temporal, no guardamos posición fija
            transform.localPosition += randomOffset;

            yield return null;

            transform.localPosition -= randomOffset;

            timer += Time.deltaTime;
        }
    }
}