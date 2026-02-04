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

    [Header("Panel a ocultar durante la cinemática")]
    public GameObject panelToHide;

    [Header("Escena final")]
    public string nextSceneName;

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

        // 👉 Ocultar panel cuando ya está todo negro
        if (panelToHide != null)
            panelToHide.SetActive(false);

        // 2️⃣ Preparar video
        videoScreen.gameObject.SetActive(true);
        videoPlayer.time = 0;
        videoPlayer.Play();

        // 3️⃣ Fade de negro → video
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // 4️⃣ Esperar casi al final
        float waitTime = (float)videoPlayer.length - fadeOutBeforeEnd;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        // 5️⃣ Fade final a negro
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        videoPlayer.Stop();
        videoScreen.gameObject.SetActive(false);

        // 👉 Volver a activar panel antes de cambiar escena
        if (panelToHide != null)
            panelToHide.SetActive(true);

        // 6️⃣ Callback + cambiar escena
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
