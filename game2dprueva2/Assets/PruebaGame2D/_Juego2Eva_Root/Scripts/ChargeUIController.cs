using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChargeUIController : MonoBehaviour
{
    public static ChargeUIController Instance;

    public GameObject chargePanel;
    public CanvasGroup chargeCanvasGroup;
    public Slider chargeSlider;
    public TMP_Text chargeText;

    public float fadeDuration = 0.3f;
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.1f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        Instance = this;

        chargePanel.SetActive(true);           // Panel activo
        chargeCanvasGroup.alpha = 0f;          // Pero invisible
    }

    public void StartCharging()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(chargeCanvasGroup, 1f, fadeDuration));
    }

    public void UpdateCharge(float normalizedValue)
    {
        chargeSlider.value = normalizedValue;

        if (normalizedValue >= 1f)
            ShakeCamera();
    }

    public void StopCharging()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvas(chargeCanvasGroup, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float target, float duration)
    {
        float start = cg.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        cg.alpha = target;
    }

    private void ShakeCamera()
    {
        // Aquí pones tu lógica de temblor de cámara, ej:
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 originalPos = mainCam.transform.localPosition;
            mainCam.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeIntensity;
            StartCoroutine(ResetCamera(mainCam, originalPos, shakeDuration));
        }
    }

    private IEnumerator ResetCamera(Camera cam, Vector3 originalPos, float duration)
    {
        yield return new WaitForSeconds(duration);
        cam.transform.localPosition = originalPos;
    }
}