using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuControllerRuntime : MonoBehaviour
{
    public static MenuControllerRuntime Instance;

    [Header("Scene Main Menu (se asigna desde MenuController)")]
    public CanvasGroup mainMenuGroup;

    [Header("Options UI Prefab Panels")]
    public CanvasGroup controlsPanelGroup;
    public CanvasGroup soundPanelGroup;
    public CanvasGroup keyboardPanelGroup;
    public CanvasGroup gamepadPanelGroup;
    public CanvasGroup keyboardReassignPanel;
    public CanvasGroup gamepadReassignPanel;

    public GameObject controlsFirstButton; // Primer botón seleccionado del panel de opciones
    public float transitionDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        HideAllPanels();
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

    public void ShowControlsPanel()
    {
        StartCoroutine(FadeOutAndIn(mainMenuGroup, controlsPanelGroup, controlsFirstButton));
    }

    public void ReturnToMainMenuFromOptions()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, mainMenuGroup, null));
    }

    public void ShowSoundPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, soundPanelGroup, null)); }
    public void ShowKeyboardPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, keyboardPanelGroup, null)); }
    public void ShowGamepadPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, gamepadPanelGroup, null)); }
    public void ReturnToControlsPanel(CanvasGroup fromPanel) { StartCoroutine(FadeOutAndIn(fromPanel, controlsPanelGroup, controlsFirstButton)); }

    public void OpenKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardPanelGroup, keyboardReassignPanel, null)); }
    public void CloseKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardReassignPanel, keyboardPanelGroup, null)); }
    public void OpenGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadPanelGroup, gamepadReassignPanel, null)); }
    public void CloseGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadReassignPanel, gamepadPanelGroup, null)); }

    private IEnumerator FadeOutAndIn(CanvasGroup fadeOut, CanvasGroup fadeIn, GameObject firstSelected)
    {
        if (fadeOut != null)
        {
            float t = 0f;
            fadeOut.interactable = false; fadeOut.blocksRaycasts = false;
            while (t < transitionDuration)
            {
                t += Time.unscaledDeltaTime;
                fadeOut.alpha = 1f - (t / transitionDuration);
                yield return null;
            }
            fadeOut.alpha = 0f;
        }

        if (fadeIn != null)
        {
            float t = 0f;
            fadeIn.alpha = 0f; fadeIn.interactable = true; fadeIn.blocksRaycasts = true;
            while (t < transitionDuration)
            {
                t += Time.unscaledDeltaTime;
                fadeIn.alpha = t / transitionDuration;
                yield return null;
            }
            fadeIn.alpha = 1f;
            if (firstSelected != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}