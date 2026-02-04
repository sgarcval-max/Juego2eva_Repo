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

    [Header("Canvas Panel que se oculta")]
    public GameObject gameplayPanel;

    [Header("Escena final")]
    public string nextSceneName;

    public void PlaySwordCinematic(Action onCinematicEnd)
    {
        StartCoroutine(CinematicRoutine(onCinematicEnd));
    }

    private IEnumerator CinematicRoutine(Action onCinematicEnd)
    {
        fadeCanvas.alpha = 0f;
        fadeCanvas.gameObject.SetActive(true);

        // 1️⃣ Fade a negro + bajar música
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutMusic(fadeDuration);

        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        // 2️⃣ Video
        videoScreen.gameObject.SetActive(true);
        videoPlayer.time = 0;
        videoPlayer.Play();

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // 3️⃣ Esperar casi final
        float waitTime = (float)videoPlayer.length - fadeOutBeforeEnd;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        // 4️⃣ Fade final + subir música
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeInMusic(fadeDuration);

        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        videoPlayer.Stop();
        videoScreen.gameObject.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        onCinematicEnd?.Invoke();

        // 5️⃣ cargar escena
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

