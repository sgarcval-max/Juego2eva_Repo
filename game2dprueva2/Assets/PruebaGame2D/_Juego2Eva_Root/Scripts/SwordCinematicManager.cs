using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using System.Collections;

public class SwordCinematicManager : MonoBehaviour
{
    public static SwordCinematicManager instance;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoScreen; // Canvas RawImage para mostrar el video

    [Header("Fade")]
    public CanvasGroup fadeCanvas; // CanvasGroup para hacer fade
    public float fadeDuration = 1f;
    public float fadeOutBeforeEnd = 2f; // segundos antes de terminar para hacer fade out

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySwordCinematic(Action onCinematicEnd)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("SwordCinematicManager está desactivado!");
            return;
        }

        StartCoroutine(CinematicRoutine(onCinematicEnd));
    }

    private IEnumerator CinematicRoutine(Action onCinematicEnd)
    {
        // --- 1. Fade in a negro ---
        fadeCanvas.alpha = 0f;
        fadeCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // --- 2. Fade out mientras empieza el video ---
        videoScreen.gameObject.SetActive(true);
        videoPlayer.time = 0;
        videoPlayer.Play();
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration)); // Fade out negro para mostrar el video

        // --- 3. Esperar mientras se reproduce el video, menos los segundos del fade final ---
        float waitTime = (float)videoPlayer.length - fadeOutBeforeEnd;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        // --- 4. Fade out final del video ---
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // --- 5. Terminar video ---
        videoPlayer.Stop();
        videoScreen.gameObject.SetActive(false);

        // --- 6. Fade in al juego ---
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        fadeCanvas.gameObject.SetActive(false);

        // --- 7. Llamar callback ---
        onCinematicEnd?.Invoke();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        fadeCanvas.alpha = startAlpha;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            yield return null;
        }
        fadeCanvas.alpha = endAlpha;
    }
}