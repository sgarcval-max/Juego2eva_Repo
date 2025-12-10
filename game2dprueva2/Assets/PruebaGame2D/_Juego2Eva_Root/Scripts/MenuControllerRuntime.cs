using System.Collections;
using UnityEngine;

public class MenuControllerRuntime : MonoBehaviour
{
    public static MenuControllerRuntime Instance;

    [Header("Prefab Options UI")]
    public GameObject optionsUIPrefab;

    [Header("Scene Main Menu")]
    public CanvasGroup mainMenuGroup; // Se oculta al abrir Options

    [Header("Options UI Panels")]
    public CanvasGroup controlsPanelGroup;
    public CanvasGroup soundPanelGroup;
    public CanvasGroup keyboardPanelGroup;
    public CanvasGroup gamepadPanelGroup;
    public CanvasGroup keyboardReassignPanel;
    public CanvasGroup gamepadReassignPanel;

    [Header("Transition")]
    public float transitionDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Instanciar OptionsUI si no está en la escena
            if (controlsPanelGroup == null && optionsUIPrefab != null)
            {
                GameObject go = Instantiate(optionsUIPrefab);
                CanvasGroup cg = go.GetComponentInChildren<CanvasGroup>(true);
                if (cg != null) controlsPanelGroup = cg;
            }

            HideAllPanels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HideAllPanels()
    {
        CanvasGroup[] all = { controlsPanelGroup, soundPanelGroup, keyboardPanelGroup, gamepadPanelGroup, keyboardReassignPanel, gamepadReassignPanel };
        foreach (var cg in all)
        {
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    // -------------------- Panels --------------------
    public void ShowControlsPanel()
    {
        StartCoroutine(FadeOutAndIn(mainMenuGroup, controlsPanelGroup));
    }

    public void ReturnToMainMenuFromOptions()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, mainMenuGroup));
    }

    public void ShowSoundPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, soundPanelGroup)); }
    public void ShowKeyboardPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, keyboardPanelGroup)); }
    public void ShowGamepadPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, gamepadPanelGroup)); }
    public void ReturnToControlsPanel(CanvasGroup fromPanel) { StartCoroutine(FadeOutAndIn(fromPanel, controlsPanelGroup)); }

    public void OpenKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardPanelGroup, keyboardReassignPanel)); }
    public void CloseKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardReassignPanel, keyboardPanelGroup)); }
    public void OpenGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadPanelGroup, gamepadReassignPanel)); }
    public void CloseGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadReassignPanel, gamepadPanelGroup)); }

    // -------------------- Corutinas --------------------
    private IEnumerator FadeInPanel(CanvasGroup cg)
    {
        if (cg == null) yield break;
        cg.alpha = 0f; cg.interactable = true; cg.blocksRaycasts = true;
        float t = 0f;
        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / transitionDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    private IEnumerator FadeOutPanel(CanvasGroup cg)
    {
        if (cg == null) yield break;
        float t = 0f; cg.interactable = false; cg.blocksRaycasts = false;
        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / transitionDuration);
            yield return null;
        }
        cg.alpha = 0f;
    }

    private IEnumerator FadeOutAndIn(CanvasGroup fadeOutGroup, CanvasGroup fadeInGroup)
    {
        yield return FadeOutPanel(fadeOutGroup);
        yield return FadeInPanel(fadeInGroup);
    }
}