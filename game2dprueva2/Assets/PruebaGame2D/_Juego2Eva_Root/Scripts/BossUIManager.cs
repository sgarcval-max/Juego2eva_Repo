using UnityEngine;
using TMPro;
using System.Collections;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    public TMP_Text phaseText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPhaseText(int phase)
    {
        StartCoroutine(ShowPhaseRoutine(phase));
    }

    IEnumerator ShowPhaseRoutine(int phase)
    {
        phaseText.text = "DRAGON - FASE " + phase;
        phaseText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        phaseText.gameObject.SetActive(false);
    }

    public void ShowBossDefeated()
    {
        phaseText.text = "¡DRAGON DERROTADO!";
        phaseText.gameObject.SetActive(true);
    }
}
