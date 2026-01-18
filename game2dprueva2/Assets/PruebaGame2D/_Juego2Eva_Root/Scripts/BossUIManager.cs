using UnityEngine;
using TMPro;
using System.Collections;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    public CanvasGroup panelGroup;
    public TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        panelGroup.alpha = 0;
    }

    public void ShowPhaseTextFade(int phase)
    {
        StartCoroutine(ShowTextRoutine(phase));
    }

    IEnumerator ShowTextRoutine(int phase)
    {
        string message = "";

        if (phase == 1)
            message = "Boss Dragon\nFase 1";
        else if (phase == 2)
            message = "Fase 2";
        else if (phase == 3)
            message = "Fase 3";

        text.text = message;

        // Fade In
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            panelGroup.alpha = i;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Fade Out
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            panelGroup.alpha = i;
            yield return null;
        }
    }

    public void ShowBossDefeated()
    {
        StartCoroutine(ShowFinalText());
    }

    IEnumerator ShowFinalText()
    {
        text.text = "Dragon Derrotado";

        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            panelGroup.alpha = i;
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            panelGroup.alpha = i;
            yield return null;
        }
    }
}