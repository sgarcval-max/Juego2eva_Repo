using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SwordCinematicManager : MonoBehaviour
{
    public static SwordCinematicManager Instance;

    [Header("Video Player & Canvas")]
    public VideoPlayer videoPlayer;  // asignar en inspector
    public CanvasGroup fadeCanvas;   // un panel negro con CanvasGroup

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float fadeOutBeforeEnd = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inicializamos VideoPlayer y Canvas
        if (videoPlayer != null) videoPlayer.gameObject.SetActive(false);
        if (fadeCanvas != null) fadeCanvas.alpha = 0f;
    }

    public void PlaySwordCinematic(Action onComplete)
    {
        if (videoPlayer == null || fadeCanvas == null)
        {
            Debug.LogError("SwordCinematicManager: VideoPlayer o FadeCanvas no asignado.");
            onComplete?.Invoke();
            return;
        }

        // Activamos el video y el canvas
        videoPlayer.gameObject.SetActive(true);
        StartCoroutine(CinematicRoutine(onComplete));
    }

    private IEnumerator CinematicRoutine(Action onComplete)
    {
        // Fade in desde negro
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Reproducir video
        videoPlayer.Play();

        // Esperamos hasta los segundos antes del final
        float waitTime = (float)videoPlayer.clip.length - fadeOutBeforeEnd;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        // Fade out antes de terminar
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Aseguramos que el video se detenga
        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);

        // Restauramos canvas invisible
        fadeCanvas.alpha = 0f;

        onComplete?.Invoke();
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }
        fadeCanvas.alpha = end;
    }
}