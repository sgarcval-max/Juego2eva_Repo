using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SwordCinematicManager : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoScreen;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;
    public float fadeOutBeforeEnd = 2f;

    [Header("Escena final")]
    public string nextSceneName; // ← PON AQUÍ EL NOMBRE DE LA ESCENA

    public void PlaySwordCinematic(Action onCinematicEnd)
    {
        StartCoroutine(CinematicRoutine(onCinematicEnd));
    }

    private IEnumerator CinematicRoutine(Action onCinematicEnd)
    {
        // 1️⃣ Fade a negro desde el juego
        fadeCanvas.alpha = 0f;
        fadeCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // 2️⃣ Cuando ya está todo negro, preparamos el video
        videoScreen.gameObject.SetActive(true);
        videoPlayer.time = 0;
        videoPlayer.Play();

        // 3️⃣ Fade desde negro al video
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // 4️⃣ Esperar hasta casi el final del video
        float waitTime = (float)videoPlayer.length - fadeOutBeforeEnd;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        // 5️⃣ Fade del video a negro
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        videoPlayer.Stop();
        videoScreen.gameObject.SetActive(false);

        // 6️⃣ Callback y cargar escena
        onCinematicEnd?.Invoke();

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
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