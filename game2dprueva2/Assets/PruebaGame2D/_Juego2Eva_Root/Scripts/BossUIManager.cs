using UnityEngine;
using TMPro;
using System.Collections;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    [Header("UI References")]
    public CanvasGroup phaseCanvasGroup;
    public TMP_Text phaseText;

    [Header("Settings")]
    public float fadeDuration = 0.8f;
    public float phaseTextVisibleTime = 2f;
    public float defeatedTextTime = 4f;

    private void Awake()
    {
        Instance = this;

        phaseCanvasGroup.alpha = 0;
        phaseCanvasGroup.gameObject.SetActive(false);
    }

    // -------- INICIO DE PELEA --------
    public void ShowBossIntro()
    {
        StopAllCoroutines();
        StartCoroutine(BossIntroRoutine());
    }

    IEnumerator BossIntroRoutine()
    {
        // PRIMERO: "BOSS DRAGON"
        yield return StartCoroutine(ShowTextRoutine("BOSS DRAGON", phaseTextVisibleTime));

        // DESPUÉS: "FASE 1"
        yield return StartCoroutine(ShowTextRoutine("FASE 1", phaseTextVisibleTime));
    }

    // -------- CAMBIO DE FASE --------
    public void ShowPhaseText(int phase)
    {
        StopAllCoroutines();
        StartCoroutine(ShowTextRoutine("FASE " + phase, phaseTextVisibleTime));
    }

    // -------- FINAL DEL BOSS --------
    public void ShowBossDefeated()
    {
        StopAllCoroutines();
        StartCoroutine(ShowTextRoutine("DRAGÓN DERROTADO", defeatedTextTime));
    }

    // -------- RUTINA GENERAL --------
    IEnumerator ShowTextRoutine(string text, float visibleTime)
    {
        phaseText.text = text;
        phaseCanvasGroup.gameObject.SetActive(true);

        // FADE IN
        yield return StartCoroutine(FadeCanvasGroup(0, 1));

        // TIEMPO EN PANTALLA
        yield return new WaitForSeconds(visibleTime);

        // FADE OUT
        yield return StartCoroutine(FadeCanvasGroup(1, 0));

        phaseCanvasGroup.gameObject.SetActive(false);
    }

    IEnumerator FadeCanvasGroup(float from, float to)
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            phaseCanvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }

        phaseCanvasGroup.alpha = to;
    }
}
