using UnityEngine;
using TMPro;
using System.Collections;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    public CanvasGroup panelGroup;
    public TextMeshProUGUI text;

    [SerializeField] private float fadeSpeed = 2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
        panelGroup.alpha = 0f;
        panelGroup.gameObject.SetActive(false);
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

        // ---------- FADE IN ----------
        panelGroup.alpha = 0f;
        while (panelGroup.alpha < 1f)
        {
            panelGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        panelGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(2f);

        // ---------- FADE OUT ----------
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

        // Fade In
        panelGroup.alpha = 0f;
        while (panelGroup.alpha < 1f)
        {
            panelGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        panelGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(4f);

        // Fade Out
        while (panelGroup.alpha > 0f)
        {
            panelGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        panelGroup.alpha = 0f;
        panelGroup.gameObject.SetActive(false);
    }
}
