using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    [Header("Panel Texto")]
    public CanvasGroup panelGroup;
    public TextMeshProUGUI text;

    [Header("Fade a negro")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 2f;
    public string nextSceneName;

    [SerializeField] private float fadeSpeed = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        panelGroup.alpha = 0f;
        panelGroup.gameObject.SetActive(false);

        fadeCanvas.alpha = 0f;
    }

    public void ShowPhaseTextFade(int phase)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowTextRoutine(phase));
    }

    IEnumerator ShowTextRoutine(int phase)
    {
        panelGroup.gameObject.SetActive(true);

        string message = phase switch
        {
            1 => "Boss Dragon\nFase 1",
            2 => "Fase 2",
            3 => "Fase 3",
            _ => ""
        };

        text.text = message;

        // Fade in
        panelGroup.alpha = 0f;
        while (panelGroup.alpha < 1f)
        {
            panelGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(2f);

        // Fade out
        while (panelGroup.alpha > 0f)
        {
            panelGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        panelGroup.alpha = 0f;
        panelGroup.gameObject.SetActive(false);
    }

    public void ShowBossDefeated()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowFinalText());
    }

    IEnumerator ShowFinalText()
    {
        panelGroup.gameObject.SetActive(true);
        text.text = "Dragon Derrotado";

        // Fade in texto
        panelGroup.alpha = 0f;
        while (panelGroup.alpha < 1f)
        {
            panelGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(4f);

        // Fade out texto
        while (panelGroup.alpha > 0f)
        {
            panelGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        panelGroup.alpha = 0f;
        panelGroup.gameObject.SetActive(false);

        // 🔥 Fade a negro y cambio de escena
        yield return StartCoroutine(FadeToBlackAndLoad());
    }

    IEnumerator FadeToBlackAndLoad()
    {
        fadeCanvas.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = 1f;

        SceneManager.LoadScene(nextSceneName);
    }
}